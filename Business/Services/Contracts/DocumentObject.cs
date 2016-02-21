using System;
using System.IO;
using Documents;
using Documents.Converter;
using Documents.Enums;
using Documents.Utils;
using Infrasturcture.Utils;
using Search;
using Services.Documents;

namespace Services.Contracts
{
    [Serializable]
    public class DocumentObject : ContentObject
    {
        [SearchIndex(Tokenized = true, IsDelete = true)]
        public Guid Id { get; set; }

        [SearchIndex(Analyzed = true, Name = "Name")]
        public string FileName { get; set; }

        [SearchIndex]
        public string SpaceSeqNo { get; set; }

        [SearchIndex]
        public string DisplayPath { get; set; }

        [SearchIndex]
        public string DownloadPath
        {
            get
            {
                return DocumentSettings.GetDownloadUrl(Id.ToString());
            }
        }

        [SearchIndex]
        public string StorePath { get; set; }

        [SearchIndex]
        public string DepId { get; set; }

        [SearchIndex]
        public int Visible { get; set; }

        [SearchIndex(Analyzed = true, Date = true)]
        public DateTime CreateTime { get; set; }

        [SearchIndex(FileName = "StorePath", Analyzed = true)]
        public string Content { get; set; }

        public string DisplayFileSize
        {
            get
            {
                return StringUtils.GetDisplayFileSize(FileSize);
            }
        }

        public string ConvertPath 
        {
            get
            {
                string tmpName = Id + DocumentType.ToConvertType().ToSuffix();
                return DocumentSettings.GetConvertPath(tmpName);
            }
        }

        public string PreviewUrl
        {
            get
            {
                return DocumentSettings.GetPreviewUrl(Id.ToString());
            }
        }

        [SearchIndex(Analyzed = true)]
        public DocumentType DocumentType { get; set; }

        public DocumentCategory DocumentCategory
        {
            get
            {
                return DocumentType.ToCategory();
            }
        }

        [SearchIndex(Name = "Size")]
        public double FileSize { get; set; }

        [SearchIndex(Analyzed = true, Date = true)]
        public DateTime UpdateTime { get; set; }

        [SearchIndex]
        public string SpaceId { get; set; }

        [SearchIndex]
        public string SpaceName { get; set; }

        [SearchIndex]
        public string CreateUserId { get; set; }

        [SearchIndex]
        public string CreateUserName { get; set; }

        [SearchIndex]
        public string UpdateUserId { get; set; }

        [SearchIndex]
        public string UpdateUserName { get; set; }

        [SearchIndex]
        public string ThumbUrl { get; set; }

        public bool IsDelete { get; set; }

        public bool IsConvert { get; set; }
    }
}
