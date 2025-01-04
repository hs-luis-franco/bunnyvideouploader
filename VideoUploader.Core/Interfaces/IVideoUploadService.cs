using VideoUploader.Core.Entities;

namespace VideoUploader.Core.Interfaces
{
    public interface IVideoUploadService
    {
        Task<VideoUpload> InitializeUploadAsync(string title);
        Task<bool> UploadChunkAsync(string uploadId, byte[] chunk, long startByte);
        Task<bool> FinalizeUploadAsync(string guid);
    }
}