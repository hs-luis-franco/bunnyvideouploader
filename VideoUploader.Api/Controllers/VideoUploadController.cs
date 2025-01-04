using Microsoft.AspNetCore.Mvc;
using VideoUploader.Core.Interfaces;
using VideoUploader.Core.DTO;
using System.Text.Json;
using System.Text;

namespace VideoUploader.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoUploadController : ControllerBase
    {
        private readonly IVideoUploadService _videoUploadService;
        private readonly ILogger<VideoUploadController> _logger;

        public VideoUploadController(
            IVideoUploadService videoUploadService,
            ILogger<VideoUploadController> logger)
        {
            _videoUploadService = videoUploadService;
            _logger = logger;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeUpload([FromBody] VideoInitializeRequest request)
        {
            try
            {
                var upload = await _videoUploadService.InitializeUploadAsync(request.Title);
                return Ok(new { VideoId = upload.BunnyGuid});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize video upload");
                return StatusCode(500, "Failed to initialize upload");
            }
        }

        [HttpPost("chunk")]
        public async Task<IActionResult> UploadChunk([FromForm] VideoChunkRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest("No file uploaded");

                using var ms = new MemoryStream();
                await request.File.CopyToAsync(ms);
                var chunk = ms.ToArray();

                var success = await _videoUploadService.UploadChunkAsync(
                    request.VideoId,
                    chunk,
                    request.StartByte
                );

                if (!success)
                    return StatusCode(500, "Failed to upload chunk");

                return Ok(new { Status = "Chunk uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload chunk");
                return StatusCode(500, "Failed to upload chunk");
            }
        }

        [HttpGet("status/{videoId}")]
        public async Task<IActionResult> GetUploadStatus(int videoId)
        {
            try
            {
                var status = "Done";

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get upload status");
                return StatusCode(500, "Failed to get upload status");
            }
        }

        [HttpPost("finalize/{videoId}")]
        public async Task<IActionResult> FinalizeUpload(string videoId)
        {
            try
            {
                var success = await _videoUploadService.FinalizeUploadAsync(videoId);
                if (!success)
                    return StatusCode(500, "Failed to finalize upload");

                return Ok(new { Status = "Upload finalized successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to finalize upload");
                return StatusCode(500, "Failed to finalize upload");
            }
        }
    }
}