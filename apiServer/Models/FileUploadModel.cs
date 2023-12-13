namespace apiServer.Models
{
    public class FileUploadModel
    {
        public string? id { get; set; }
        public List<IFormFile>? files { get; set; }
    }
}
