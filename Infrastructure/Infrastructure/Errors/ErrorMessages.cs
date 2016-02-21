using System.Linq;

namespace Infrasturcture.Errors
{ 
    internal class ErrorMapping
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorMapping(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }

    public static class ErrorMessages
    {
        //业务错误
        public const int UndefinedError = -1;
        public const int Success = 0;
        public const int SpaceNotExist = 1;
        public const int DocumentExist = 2;
        public const int ReNameFailed = 3;
        public const int MoveFailed = 4;
        public const int UploadFailed = 5;
        public const int CreateIndexFailed = 6;
        public const int ReadWordFailed = 7;
        public const int ReadPptFailed = 8;
        public const int ReadPdfFailed = 9;
        public const int ReadExcelFailed = 10;
        public const int OfficeToPdfUninstall = 11;
        public const int DwgLoadFailed = 12;
        public const int ReadTextFailed = 13;

        public const int Pdf2SwfToolNotExist = 99;
        public const int PdftoHtmlFailed = 100;
        public const int ConvertedFileNotExist = 101;
        public const int WordToHtmlFailed = 102;
        public const int ExcelToHtmlFailed = 103;
        public const int WordToPdfFailed = 104;
        public const int PPTToHtmlFailed = 105;
        public const int PPTToPdfFailed = 106;
        public const int TextToHtmlFailed = 107;
        public const int ExcelToPdfFailed = 108;
        public const int UnsupportDwgConvert = 110;
        public const int TextToPdfFailed = 111;
        public const int DwgConvertFailed = 112;
        public const int TextConvertFailed = 113;
        public const int ReadContentUnknowFailed = 114;
        public const int PPTToSwfFailed = 115;
        public const int Pdf2HTMLToolNotExist = 116;

        //底层错误
        public const int DbLoadFailed = 1000;
        public const int ServiceError = 1001;
        public const int FileNotExist = 1002;
        public const int DirectoryNotExist = 1003;
        public const int UnsupportFileType = 1004;
       
        internal static ErrorMapping[] ErrorMaps =
        {
            //业务错误
            new ErrorMapping(UndefinedError, "未定义的错误类型"),
            new ErrorMapping(Success, "操作成功"),
            new ErrorMapping(SpaceNotExist, "空间不存在"),
            new ErrorMapping(DocumentExist, "文档不存在"),
            new ErrorMapping(ReNameFailed, "重命名失败"), 
            new ErrorMapping(MoveFailed, "移动文件夹失败"), 
            new ErrorMapping(UploadFailed, "上传文件失败"),
            new ErrorMapping(CreateIndexFailed, "创建索引错误"),
            new ErrorMapping(ReadWordFailed, "读取Word文件内容失败"),
            new ErrorMapping(ReadPptFailed, "读取PPT文件内容失败"),
            new ErrorMapping(ReadPdfFailed, "读取PDF文件内容失败"),
            new ErrorMapping(ReadExcelFailed, "读取Excel文件内容失败"),
            new ErrorMapping(OfficeToPdfUninstall, "Office未安装PDF插件"),  
            new ErrorMapping(DwgLoadFailed, "读取dwg文件出错"), 
            new ErrorMapping(PdftoHtmlFailed, "PDF转换为HTML内部错误"), 
            new ErrorMapping(ConvertedFileNotExist, "转换后的文件不存在"), 
            new ErrorMapping(WordToHtmlFailed, "Word转换为HTML内部错误"),
            new ErrorMapping(WordToPdfFailed, "Word转换为PDF内部错误"), 
            new ErrorMapping(PPTToHtmlFailed, "PPT转换为HTML内部错误"),
            new ErrorMapping(PPTToPdfFailed, "PPT转换为PDF内部错误"), 
            new ErrorMapping(Pdf2SwfToolNotExist, "PDF2Swf工具不存在"),
            new ErrorMapping(Pdf2HTMLToolNotExist, "PDF2HTML工具不存在"), 
            new ErrorMapping(ExcelToHtmlFailed, "Excel转换到HTML内部错误"),
            new ErrorMapping(ExcelToPdfFailed, "Excel转换到PDF内部错误"), 
            new ErrorMapping(TextToPdfFailed, "Text文本转换到PDF内部错误"), 
            new ErrorMapping(DwgConvertFailed, "Dwg格式转换错误"),
            new ErrorMapping(TextConvertFailed, "Text文本拷贝转换失败"), 
            new ErrorMapping(ReadContentUnknowFailed, "读取文本内容失败"),
 
            new ErrorMapping(DbLoadFailed, "数据库读取错误"), 
            new ErrorMapping(ServiceError, "服务获取失败"), 
            new ErrorMapping(FileNotExist, "文件不存在"),
            new ErrorMapping(DirectoryNotExist, "文件夹不存在"),
            new ErrorMapping(UnsupportFileType, "未支持的文件类型错误"),
            new ErrorMapping(UnsupportDwgConvert, "不支持的DWG格式转换"),
        };

        public static string GetErrorMessages(int errorCode)
        {
            var mapping = ErrorMaps.First(p => p.ErrorCode == errorCode);
            if (mapping != null)
            {
                return mapping.ErrorMessage;
            }
            return ErrorMaps.First(p => p.ErrorCode == UndefinedError).ErrorMessage;
        }
    }

    
}
