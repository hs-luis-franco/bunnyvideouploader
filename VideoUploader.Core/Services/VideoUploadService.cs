using VideoUploader.Core.Entities;
using VideoUploader.Core.Interfaces;

namespace VideoUploader.Core.Services
{
    public class VideoUploadService : IVideoUploadService
    {
        private readonly IBunnyStorageService _bunnyStorage;

        public VideoUploadService(IBunnyStorageService bunnyStorage)
        {
            _bunnyStorage = bunnyStorage;
        }

        public async Task<VideoUpload> InitializeUploadAsync(string title)
        {
            var guid = await _bunnyStorage.InitializeUploadAsync(title);

            var upload = new VideoUpload
            {
                BunnyGuid = guid,
                CreatedAt = DateTime.UtcNow,
                Status = "Initialized",
                UploadedChunks = new Dictionary<long, long>()
            };


            return upload;
        }

        public async Task<bool> UploadChunkAsync(string uploadId, byte[] chunk, long startByte)
        {
            // var uploadUrl = await _bunnyStorage.GetUploadUrlAsync(uploadId);
            var endByte = startByte + chunk.Length - 1;
            var success = await _bunnyStorage.UploadChunkAsync(uploadId, chunk, startByte, endByte);


            return success;
        }
        

        public async Task<bool> FinalizeUploadAsync(string guid)
        {
            try
            {
                var success = await _bunnyStorage.FinalizeUploadAsync(guid);
                return success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
