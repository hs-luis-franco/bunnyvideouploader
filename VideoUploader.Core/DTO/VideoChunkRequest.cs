using Microsoft.AspNetCore.Http;
namespace VideoUploader.Core.DTO;

public class VideoChunkRequest
{
    public IFormFile File { get; set; }
    public string VideoId { get; set; }
    public long StartByte { get; set; }
}