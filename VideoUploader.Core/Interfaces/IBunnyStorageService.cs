namespace VideoUploader.Core.Interfaces
{
    public interface IBunnyStorageService
    {
        Task<string> InitializeUploadAsync(string title);
        Task<bool> UploadChunkAsync(string guid, byte[] chunk, long startByte, long endByte);
        Task<bool> FinalizeUploadAsync(string guid);
        Task<string> GetUploadUrlAsync(int uploadId);
    }
}