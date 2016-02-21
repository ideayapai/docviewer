using System;

namespace Services.Models
{
    public class DocumentContract: BaseContract
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string DisplayPath { get; set; }

        public string DownloadPath { get; set; }

        public DateTime CreateTime { get; set; }

        public string Content { get; set; }
 
        public double FileSize { get; set; }

        public DateTime UpdateTime { get; set; }

        public string SpaceId { get; set; }

        public string SpaceName { get; set; }

        public string CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserId { get; set; }

        public string UpdateUserName { get; set; }

        public string ThumbUrl { get; set; }

        public bool IsDelete { get; set; }
    }
}