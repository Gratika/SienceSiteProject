﻿    using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel.Args;
using Minio;
using System.Net.Http;
using System.IO.Compression;
using Minio.DataModel.Result;
using System.ComponentModel;
using System.Data;
using Microsoft.AspNetCore.WebUtilities;
using static System.Net.Mime.MediaTypeNames;
using Org.BouncyCastle.Utilities;
using ImageMagick;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace apiServer.Controllers.Minio
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringController _genericString;
        private readonly HttpClient _httpClient;
        private readonly IMinioClient _minio;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FilesController(ArhivistDbContext context, GenerateRandomStringController genericString, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minio = new MinioClient()
                .WithEndpoint("minio1:9000") //localhost:9090
                .WithCredentials("ROOTUSER", "CHANGEME123")
            .WithSSL(false)
                .Build();
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("AddFiles")]
        public async Task<ActionResult<List<string>>> AddFiles([FromForm] string id, [FromForm] List<IFormFile> files)
        {
            try
            {
                Articles article = await _context.Articles.Where(a => a.Id == id).Include(a => a.author_).Include(a => a.theory_).FirstOrDefaultAsync();
                string bucketName;
                string[] prefixForArticle = article.path_file.Split(',');
                if (string.IsNullOrEmpty(article.author_.path_bucket) == true)
                {
                    bucketName = _genericString.GenerateRandomString(15);
                    article.author_.path_bucket = bucketName;
                }
                else
                {
                    bucketName = article.author_.path_bucket;
                }
                //Если бакета не существует - добавляем
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                if (string.IsNullOrEmpty(article.path_file) == true)
                {
                    article.path_file = _genericString.GenerateRandomString(20); // папка для статьи
                    prefixForArticle[0] = article.path_file;
                }

                foreach (var file in files)
                {
                    if (file != null)
                    {
                        IFormFile fileInWebp;
                        if (file.ContentType == "image/jpeg")
                        {
                            fileInWebp = ConvertToWebp(file);
                        }
                        else
                        {
                            fileInWebp = file;
                        }    
                        
                        long currentUtcTime = DateTime.Now.Ticks;
                        string NewFileName = Path.GetFileNameWithoutExtension(fileInWebp.FileName) + "_" + currentUtcTime + Path.GetExtension(fileInWebp.FileName);

                        var putObjectArgs = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(prefixForArticle[0] + "/" + NewFileName)
                            .WithObjectSize(fileInWebp.Length)
                            .WithStreamData(fileInWebp.OpenReadStream());
                        //.WithContentType(file.ContentType);

                        // Выполняем операцию загрузки объекта в <link>MinIO</link>
                        await _minio.PutObjectAsync(putObjectArgs);


                        article.path_file += "," + NewFileName;
                    }
                }

                _context.Articles.Update(article);
                _context.people.Update(article.author_);
                _context.SaveChanges();

                return Ok("Файлы успешно добавленны");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("ConvertToWebP")]
        public FormFile ConvertToWebp(IFormFile file)
        {
            // Создаем объект MagickImage из загруженного файла
            using (var image = new MagickImage(file.OpenReadStream()))
            {
                // Устанавливаем формат конвертирования в WebP
                image.Format = MagickFormat.WebP;

                // Создаем поток для сохранения конвертированного файла
                using (var outputStream = new MemoryStream())
                {
                    // Сохраняем изображение в формате WebP в поток вывода
                    image.Write(outputStream);

                    // Получаем массив байтов из MemoryStream
                    var outputBytes = outputStream.ToArray();

                    // Возвращаем объект FormFile с конвертированным файлом
                    var webpFile = new FormFile(new MemoryStream(outputBytes), 0, outputBytes.Length, null, Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                    {
                        Headers = file.Headers,
                        ContentType = "image/webp"
                    };

                    return webpFile;
                }
            }
        }

        [HttpGet("GetUrlFromMinio")]
        public async Task<List<string>> GetUrlFromMinio(string path_files, string path_bucket) // обращаемся в minio для взятия url файлов
        {
            try
            {
               // IMinioClient minio = new MinioClient()
               //.WithEndpoint("localhost:9000") //localhost:9090
               //.WithCredentials("ROOTUSER", "CHANGEME123")
               //.WithSSL(false)
               //.Build();
                List<string> downloadUrl = new List<string>();
                string[] path_to_file = path_files.Split(',');

                for (int i = 1; i <= path_to_file.Length - 1; i++)
                {
                    PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                                                     .WithBucket(path_bucket)
                                                     .WithObject(path_to_file[0] + "/" + path_to_file[i])
                                                     .WithExpiry(3600);

                    downloadUrl.Add(await _minio.PresignedGetObjectAsync(args));
                }
                return downloadUrl;
            }
            catch
            {
                throw new Exception();
            }
        }
        [HttpGet("GetArchivWithFiles")]
        public async Task<ActionResult> GetArchivWithFiles(string id) // создаем файлы из url и записываем в архив
        {
            try
            {
                Articles article = await _context.Articles.Where(a => a.Id == id).Include(a => a.author_).Include(a => a.theory_).FirstOrDefaultAsync();
                List<string> downloadUrl = new List<string>();
                downloadUrl = await GetUrlFromMinio(article.path_file, article.author_.path_bucket);

                List<byte[]> fileContents = new List<byte[]>();
                List<string> fileTypes = new List<string>();
                foreach (string url in downloadUrl)
                {
                    // Загрузить содержимое файла по URL-адресу в виде массива байтов
                    byte[] fileContent = await _httpClient.GetByteArrayAsync(url);
                    fileContents.Add(fileContent);

                    // Берем тип файла
                    string fileType = Path.GetExtension(url);
                    fileTypes.Add(fileType);
                }

                HttpResponseMessage response = await _httpClient.GetAsync(downloadUrl[0]);
                string zipFileName = "Archiv.zip";

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        for (int i = 0; i < fileContents.Count; i++)
                        {

                            string[] TypeFile = fileTypes[i].Split('?');
                            string fileName = _genericString.GenerateRandomString(8);
                            // Создать новую запись в архиве и записать содержимое файла
                            ZipArchiveEntry zipEntry = zipArchive.CreateEntry(fileName + TypeFile[0]);
                            using (Stream entryStream = zipEntry.Open())
                            {
                                entryStream.Write(fileContents[i], 0, fileContents[i].Length);
                            }
                        }
                    }

                    // Вернуть архивный файл в виде <link>FileContentResult</link>
                    return new FileContentResult(memoryStream.ToArray(), "application/zip")
                    {
                        FileDownloadName = zipFileName
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        [HttpPost("RedactFiles")]
        public async Task<ActionResult<List<string>>> RedactFiles(string id, List<IFormFile>? files) // создаем файлы из url и записываем в архив
        {
            try
            {
                await DeleteFiles(id);
            return await AddFiles(id, files);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        [HttpPost("DeleteFiles")]
        public async Task<ActionResult> DeleteFiles(string id) // создаем файлы из url и записываем в архив
        {
            try
            {
                Articles article = await _context.Articles.Where(a => a.Id == id).Include(a => a.author_).Include(a => a.theory_).FirstOrDefaultAsync();
                string[] path_to_file = article.path_file.Split(',');
                for (int i = 1; i < path_to_file.Length; i++)
                {
                    RemoveObjectArgs args = new RemoveObjectArgs()
                                                     .WithBucket(article.author_.path_bucket)
                                                     .WithObject(path_to_file[0] + "/" + path_to_file[i]);
                    await _minio.RemoveObjectAsync(args);

                }
                article.path_file = "";
                _context.Articles.Update(article);
                _context.SaveChanges();
                return Ok("Файлы удачно удаленны");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
