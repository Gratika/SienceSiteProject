using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace apiServer.Controllers.Minio
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringController _genericString;
        private readonly HttpClient _httpClient;
        private readonly IMinioClient _minio;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FilesController _filesController;
        private readonly string bucket;
        public ImagesController(ArhivistDbContext context, GenerateRandomStringController genericString, IWebHostEnvironment hostingEnvironment, FilesController filesController)
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
            _filesController = filesController;
            bucket = "images";
        }
        [HttpPost("AddImages")]
        public async Task<ActionResult<string>> AddImages([FromForm] List<IFormFile> upload, string articleId) // обращаемся в minio для взятия url файлов
        {
            //try
            //{
                if (upload.Count == 0)
            {
                return Ok("Файлы пустые");
            }
            //Если бакета не существует - добавляем
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucket);
            bool found = await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucket);
                await _minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }


            IFormFile fileInWebp = _filesController.ConvertToWebp(upload[0]);
            string NewFileName = Path.GetFileNameWithoutExtension(fileInWebp.FileName) + "_" + DateTime.Now.Ticks + Path.GetExtension(fileInWebp.FileName);
            var putObjectArgs = new PutObjectArgs()
                         .WithBucket(bucket)
                         .WithObject(NewFileName)
                         .WithObjectSize(fileInWebp.Length)
                         .WithStreamData(fileInWebp.OpenReadStream());
            await _minio.PutObjectAsync(putObjectArgs);
            var url = await GetUrl(NewFileName, articleId);


            return url;
            //}
            //catch
            //{
            //    throw new Exception();
            //}
        }
        [HttpGet("GetUrl")]
        public async Task<string> GetUrl(string path_files, string articleId) // обращаемся в minio для взятия url файлов
        {
            //try
            //{
            PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                                                 .WithBucket(bucket)
                                                 .WithObject(path_files)
                                                 .WithExpiry(3600);

                IMinioClient minio = new MinioClient()
                .WithEndpoint("localhost:9000") //localhost:9090
                .WithCredentials("ROOTUSER", "CHANGEME123")
                .WithSSL(false)
                .Build();
                string downloadUrl = await minio.PresignedGetObjectAsync(args);

                Articles article = _context.Articles.FirstOrDefault(a => a.Id == articleId);
                article.urls += downloadUrl + ",";
                _context.Articles.Update(article);
                _context.SaveChanges();

                // Создание объекта JSON
                var json = new
                {
                    url = downloadUrl
                };

                // Преобразование объекта JSON в строку
                string jsonResult = JsonConvert.SerializeObject(json);

                return jsonResult;
            //}
            //catch
            //{
            //    throw new Exception();
            //}
        }
        [HttpPost("Delete")]
        public async void Delete(Articles article) // обращаемся в minio для взятия url файлов
        {

            string[] urls = article.urls.Split(',');
            foreach (string url in urls)
            {
                Uri uri = new Uri(url);
                string FileName = uri.Segments[uri.Segments.Length - 1];
                RemoveObjectArgs args = new RemoveObjectArgs()
                                                         .WithBucket(bucket)
                                                         .WithObject(FileName);
                await _minio.RemoveObjectAsync(args);
            }
        }
        
    }
}
