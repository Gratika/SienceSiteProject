using apiServer.Models;
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

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringController _genericString;
        private readonly HttpClient _httpClient;
        private readonly IMinioClient _minio;

        public FilesController(ArhivistDbContext context, GenerateRandomStringController genericString)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minio = new MinioClient()
                .WithEndpoint("minio1:9000") //localhost:9090
                .WithCredentials("ROOTUSER", "CHANGEME123")
                .WithSSL(false)
                .Build();
        }
        [HttpPost("AddFiles")]
        public async Task<ActionResult<List<string>>> AddFiles(string? path_file, string? BucketNameForDb, List<IFormFile> files)
        {
            try
            {
                string bucketName;              
                if (BucketNameForDb == null)
                {
                    bucketName = _genericString.GenerateRandomString(15);
                    BucketNameForDb = bucketName;
                }
                else
                {
                    bucketName = BucketNameForDb;
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

                
                string prefixForArticle = _genericString.GenerateRandomString(20);
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        IFormFile fileInWebp = ConvertToWebp(file);
                        long currentUtcTime = DateTime.Now.Ticks;
                        string NewFileName = Path.GetFileNameWithoutExtension(fileInWebp.FileName) + "_" + currentUtcTime + Path.GetExtension(fileInWebp.FileName);

                        var putObjectArgs = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(prefixForArticle + "/" + NewFileName)
                            .WithObjectSize(fileInWebp.Length)
                            .WithStreamData(fileInWebp.OpenReadStream());
                        //.WithContentType(file.ContentType);

                        // Выполняем операцию загрузки объекта в <link>MinIO</link>
                        await _minio.PutObjectAsync(putObjectArgs);

                        if (path_file == null)
                        {
                            path_file = prefixForArticle + ",";
                        }
                        path_file += NewFileName + ",";
                    }
                }

                List<string> ForReturn = new List<string>();
                ForReturn.Add(BucketNameForDb);
                ForReturn.Add(path_file);
                return ForReturn;
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось загрузить файла - " + ex.Message);
            }
        }
        [HttpPost("ConvertToWebP")]
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
        [HttpPost("GetUrl")]
        public async Task<List<string>> GetUrl(/*ArticleWithUserTokenModel articlesWithUserTokens*/string path_files,string path_bucket) // обращаемся в minio для взятия url файлов
        {
            try
            {
                List<string> downloadUrl = new List<string>();
                //List<byte[]> Files = new List<byte[]>();
                string[] path_to_file = path_files.Split(',');

                for (int i = 1; i < path_to_file.Length - 1; i++)
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
        [HttpPost("GetArchivWithFiles")]
        public async Task<ActionResult> GetArchivWithFiles(string path_files, string path_bucket) // создаем файлы из url и записываем в архив
        {
            try
            {
                List<string> downloadUrl = new List<string>();
                downloadUrl = await GetUrl(path_files, path_bucket);

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
                return BadRequest("Ошибка, не удалось выгрузить файлы - " + ex.Message);
            }
        }
        [HttpPost("RedactFiles")]
        public async Task<ActionResult<List<string>>> RedactFiles(string? path_files, string? pathBucket, List<IFormFile>? files) // создаем файлы из url и записываем в архив
        {
            try
            {
                DeleteFiles(path_files, pathBucket);
                return await AddFiles(null, pathBucket, files);
            }
            catch 
            {
                throw new Exception();
            }
        }
        [HttpPost("DeleteFiles")]
        public async void DeleteFiles(string path_files, string pathBucket) // создаем файлы из url и записываем в архив
        {
            try
            {
                string[] path_to_file = path_files.Split(',');
                for (int i = 1; i < path_to_file.Length - 1; i++)
                {
                    RemoveObjectArgs args = new RemoveObjectArgs()
                                                     .WithBucket(pathBucket)
                                                     .WithObject(path_to_file[0] + "/" + path_to_file[i]);
                    _minio.RemoveObjectAsync(args);

                }
            }
           catch 
            {
                throw new Exception();
            }
        }
    }
}
