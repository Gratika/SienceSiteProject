using apiServer.Controllers.Search;
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
        private readonly FilesController _filesController;
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
        }
        /* [HttpPost("AddImages")]
         public async Task<List<string>> AddImages([FromForm] List<IFormFile> files) // обращаемся в minio для взятия url файлов
         {
             try
             {
                 //Если бакета не существует - добавляем
                 var beArgs = new BucketExistsArgs()
                     .WithBucket("images");
                 bool found = await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                 if (!found)
                 {
                     var mbArgs = new MakeBucketArgs()
                         .WithBucket("images");
                     await _minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                 }
                 List<string> urls = new List<string>();

                 foreach (var file in files)
                 {
                     IFormFile fileInWebp = _filesController.ConvertToWebp(file);
                 string NewFileName = Path.GetFileNameWithoutExtension(fileInWebp.FileName) + "_" + DateTime.Now.Ticks + Path.GetExtension(fileInWebp.FileName);
                 var putObjectArgs = new PutObjectArgs()
                              .WithBucket("images")
                              .WithObject(NewFileName)
                              .WithObjectSize(fileInWebp.Length)
                              .WithStreamData(fileInWebp.OpenReadStream());
                     await _minio.PutObjectAsync(putObjectArgs);
                     urls.Add(await GetUrl(NewFileName));
                 }             

                 return urls;
             }
             catch
             {
                 throw new Exception();
             }
 }*/

        [HttpPost("AddImages")]
        public async Task<ActionResult<List<string>>> AddImages([FromForm] List<IFormFile> upload) // обращаемся в minio для взятия url файлов
        {
            try
            {
                if (upload.Count == 0)
                {
                    return Ok("Файлы пустые");
                }
                //Если бакета не существует - добавляем
                var beArgs = new BucketExistsArgs()
                    .WithBucket("images");
                bool found = await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket("images");
                    await _minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                List<string> urls = new List<string>();

                foreach (var file in upload)
                {
                    IFormFile fileInWebp = _filesController.ConvertToWebp(file);
                    string NewFileName = Path.GetFileNameWithoutExtension(fileInWebp.FileName) + "" + DateTime.Now.Ticks + Path.GetExtension(fileInWebp.FileName);
                    var putObjectArgs = new PutObjectArgs()
                                 .WithBucket("images")
                                 .WithObject(NewFileName)
                                 .WithObjectSize(fileInWebp.Length)
                                 .WithStreamData(fileInWebp.OpenReadStream());
                    await _minio.PutObjectAsync(putObjectArgs);
                    urls.Add(await GetUrl(NewFileName));
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
