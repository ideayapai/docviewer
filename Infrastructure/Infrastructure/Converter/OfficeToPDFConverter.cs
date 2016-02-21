using System;
using Common.Logging;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// Office转换为PDF
    /// </summary>
    public class OfficeToPdfConverter
    {
        private readonly WordToPdfConverter _wordToPdfConverter = new WordToPdfConverter();
        private readonly ExcelToPdfConverter _excelToPdfConverter = new ExcelToPdfConverter();
        private readonly PPTToPdfConverter _ppttoPdfConverter = new PPTToPdfConverter();
        private readonly TextToPdfConverter _textToPdfConverter = new TextToPdfConverter();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string targetPath)
        {
            _logger.Debug("OfficeToPDF, convert " + sourcePath + " to: " + targetPath);

            if (IsWord(sourcePath))
            {
                return _wordToPdfConverter.Convert(sourcePath, targetPath);
            }

            if (IsExcel(sourcePath))
            {
                return _excelToPdfConverter.Convert(sourcePath, targetPath);
            }

            if (IsPPT(sourcePath))
            {
                return _ppttoPdfConverter.Convert(sourcePath, targetPath);
            }
            
            if (IsText(sourcePath))
            {
                return _textToPdfConverter.Convert(sourcePath, targetPath);
            }

            throw new Exception("Undefined converter");

        }

        private static bool IsWord(string sourcePath)
        {
            return sourcePath.ToLower().EndsWith(".docx") || sourcePath.ToLower().EndsWith(".doc");
        }

        private static bool IsExcel(string sourcePath)
        {
            return sourcePath.ToLower().EndsWith(".xls") || sourcePath.ToLower().EndsWith(".xlsx");
        }

        private static bool IsPPT(string sourcePath)
        {
            return sourcePath.ToLower().EndsWith(".ppt") || sourcePath.ToLower().EndsWith(".pptx");
        }

        private static bool IsText(string sourcePath)
        {
            return sourcePath.ToLower().EndsWith(".txt");
        }
    }
}
