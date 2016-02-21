using Documents.Enums;

namespace Documents.Converter
{
    public class ConvertResult
    {
        public int ErrorCode { get; set; }

        public string SourcePath { get; set; }

        public string TargetPath { get; set; }
    }
}
