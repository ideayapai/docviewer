using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Documents.Utils;

namespace Documents.Enums
{
    public static class ConvertFileTypeExtension
    {
        private static readonly Dictionary<string, DocumentType> Dictionary = new Dictionary<string, DocumentType>
        {
            { ".doc", DocumentType.Word},
            { ".docx",DocumentType.Word},
            { ".xls", DocumentType.Excel},
            { ".xlsx",DocumentType.Excel},
            { ".ppt", DocumentType.PPT},
            { ".pptx",DocumentType.PPT},
            { ".rtf", DocumentType.RTF},
            { ".dwg", DocumentType.CAD},
            { ".bmp", DocumentType.BMP},
            { ".jpg", DocumentType.JPG},
            { ".png", DocumentType.PNG},
            { ".gif", DocumentType.GIF},
            { ".psd", DocumentType.PSD},
            { ".txt", DocumentType.TXT},
            { ".obj", DocumentType.OBJ},
            { ".pdf", DocumentType.PDF},
            { ".zip", DocumentType.ZIP},
            { ".rar", DocumentType.RAR},
            { ".jar", DocumentType.JAR},
            { ".damage", DocumentType.DAMAGE},
            { ".iso", DocumentType.ISO},
            { ".old", DocumentType.OLD},
            { ".7z",  DocumentType.Z7},
            { ".bak", DocumentType.BAK},
            { ".tmp", DocumentType.TMP},
            { ".html", DocumentType.HTML},
            { ".htm", DocumentType.HTM},
            { ".xml", DocumentType.XML},
            { ".asp", DocumentType.ASP},
            { ".chm", DocumentType.CHM},
            { ".ai", DocumentType.AI},
            { ".hlp", DocumentType.HLP},
            { ".ttf", DocumentType.TTF},
            { ".ttc", DocumentType.TTC},
            { ".otf", DocumentType.OTF},
            { ".fon", DocumentType.FON},
            { ".avi", DocumentType.AVI},
            { ".asf", DocumentType.ASF},
            { ".mp4", DocumentType.MP4},
            { ".mp3", DocumentType.MP3},
            { ".gp3", DocumentType.GP3},
            { ".rmvb",DocumentType.RMVB},
            { ".wmv", DocumentType.WMV},
            { ".swf", DocumentType.SWF},
            { ".wma", DocumentType.WMA},
            { ".wav", DocumentType.WAV},
            { ".ipa", DocumentType.IPA},
        };

        public static DocumentCategory ToCategory(this DocumentType documentType)
        {

            switch (documentType)
            {
                case DocumentType.BMP:
                case DocumentType.JPG:
                case DocumentType.PNG:
                case DocumentType.PSD:
                    return DocumentCategory.Image;

                case DocumentType.CAD:
                    return DocumentCategory.CAD;

                case DocumentType.Word:
                case DocumentType.Excel:
                case DocumentType.PPT:
                case DocumentType.PDF:
                case DocumentType.RTF:
                    return DocumentCategory.Office;

                case DocumentType.TXT:
                    return DocumentCategory.Text;

                case DocumentType.Folder:
                    return DocumentCategory.Folder;
                default:
                    return DocumentCategory.Unknow;
            }
        }


        public static DocumentType ToDocumentType(this string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension != null && Dictionary.ContainsKey(extension.ToLower()))
            {
                return Dictionary[extension.ToLower()];
            }
            return DocumentType.Unknow;
        }

        public static ConvertFileType ToConvertType(this DocumentType documentType)
        {

            switch (documentType)
            {
                case DocumentType.Word:
                    return ConvertFileType.WordToHtml;
                case DocumentType.Excel:
                    return ConvertFileType.ExcelToHtml;
                case DocumentType.PPT:
                    return ConvertFileType.PPTToHtml;
                case DocumentType.TXT:
                    return ConvertFileType.TextToHtml;
                case DocumentType.PDF:
                    return ConvertFileType.PdfToHtml;
                case DocumentType.RTF:
                    return ConvertFileType.WordToHtml;
                case DocumentType.CAD:
                    return ConvertFileType.CadToSvg;

                default:
                    return ConvertFileType.None;
            }
        }

        public static string ToSuffix(this ConvertFileType fileType)
        {
            switch (fileType)
            {
                case ConvertFileType.WordToHtml:
                case ConvertFileType.ExcelToHtml:
                case ConvertFileType.PPTToHtml:
                case ConvertFileType.PdfToHtml:
                case ConvertFileType.TextToHtml:
                case ConvertFileType.WordPDFtoHtml:
                case ConvertFileType.ExcelPDFToHtml:
                case ConvertFileType.PPTPDFToHtml:
                case ConvertFileType.TextPDFToHtml:
                    return ".html";

                case ConvertFileType.CadToBmp:
                    return ".bmp";

                case ConvertFileType.CadToJpg:
                    return ".jpg";

                case ConvertFileType.CadToPng:
                    return ".png";

                case ConvertFileType.CadToSvg:
                    return ".svg";

                default:
                    return string.Empty;
            }

        }
    }
}
