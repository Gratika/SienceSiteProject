using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

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
        public ImagesController(ArhivistDbContext context, GenerateRandomStringController genericString, IWebHostEnvironment hostingEnvironment)
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
        [HttpPost("AddImages")]
        public async Task<List<string>> AddImages([FromForm] List<IFormFile> files) // обращаемся в minio для взятия url файлов
        {
            try
            {
                 List<string> urls = new List<string>();
                foreach (var file in files)
                {
                    var putObjectArgs = new PutObjectArgs()
                             .WithBucket("images")
                             .WithObject(file.Name)
                             .WithObjectSize(file.Length)
                             .WithStreamData(file.OpenReadStream());
                    await _minio.PutObjectAsync(putObjectArgs);
                    urls.Add(await GetUrl(file.Name));
                }             

                return urls;
            }
            catch
            {
                throw new Exception();
            }
        }
        [HttpGet("GetUrl")]
        public async Task<string> GetUrl(string path_files) // обращаемся в minio для взятия url файлов
        {
            try
            {
                    PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                                                     .WithBucket("images")
                                                     .WithObject(path_files)
                                                     .WithExpiry(3600);

                string downloadUrl = await _minio.PresignedGetObjectAsync(args);

                return downloadUrl;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
