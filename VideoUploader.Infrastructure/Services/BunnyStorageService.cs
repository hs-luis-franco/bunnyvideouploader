using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using VideoUploader.Core.DTO;
using VideoUploader.Core.Interfaces;

namespace VideoUploader.Infrastructure.Services
{
    public class BunnyStorageService : IBunnyStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BunnyStorageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            _httpClient.DefaultRequestHeaders.Add("AccessKey", 
                _configuration["BunnyNet:ApiKey"]);
        }

        public async Task<string > InitializeUploadAsync(string title)
        {
            var libraryId = _configuration["BunnyNet:LibraryId"];
            
            var createVideoRequest = new HttpRequestMessage(HttpMethod.Post, 
                $"{_configuration["BunnyNet:ApiUrl"]}/library/{libraryId}/videos")
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    title = title
                }), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(createVideoRequest);
            response.EnsureSuccessStatusCode();

            var videoData = await JsonSerializer.DeserializeAsync<BunnyVideoResponse>(
                await response.Content.ReadAsStreamAsync());

            var uploadUrlRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{_configuration["BunnyNet:ApiUrl"]}/library/{libraryId}/videos/{videoData.guid}/upload");


            return (videoData.guid);
        }

        public async Task<bool> UploadChunkAsync(string guid, byte[] chunk, long startByte, long endByte)
        {
            using var content = new ByteArrayContent(chunk);
            content.Headers.ContentType = new MediaTypeHeaderValue("video/webm");
            content.Headers.ContentRange = new ContentRangeHeaderValue(startByte, endByte);
            var uploadUrl = $"{_configuration["BunnyNet:ApiUrl"]}/library/{_configuration["BunnyNet:LibraryId"]}/videos/{guid}";
            var response = await _httpClient.PutAsync(uploadUrl, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinalizeUploadAsync(string guid)
        {
            // there is no endpoint to finalize the upload
            return true;
        }
        
        public async Task<string> GetUploadUrlAsync(int uploadId)
        {
            var libraryId = _configuration["BunnyNet:LibraryId"];
            
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_configuration["BunnyNet:ApiUrl"]}/videolibrary/{libraryId}/videos/{uploadId}/upload");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
