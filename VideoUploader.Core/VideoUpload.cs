using System;
using System.Collections.Generic;

namespace VideoUploader.Core.Entities
{
    public class VideoUpload
    {
        public int Id { get; set; }
        public string BunnyGuid { get; set; }
        public string UploadUrl { get; set; }
        public long TotalSize { get; set; }
        public Dictionary<long, long> UploadedChunks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; }
    }
}
