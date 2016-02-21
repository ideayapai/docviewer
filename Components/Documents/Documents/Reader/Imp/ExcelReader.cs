//using Microsoft.Office.Interop.Excel;

using System;
using System.IO;
using Aspose.Cells;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Reader.Imp
{
    /// <summary>
    /// 转换Excel为PDF
    /// </summary>
    public class ExcelReader : IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string Read(string filePath)
        {
            _logger.DebugFormat("读取Excel文件{0}" + filePath);

            try
            {
                Workbook workbook = new Workbook(filePath);
                return GetContent(workbook);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
        }

        public string Read(Stream inputStream)
        {
            try
            {
                Workbook workbook = new Workbook(inputStream);
                return GetContent(workbook);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
        }

        private static string GetContent(Workbook workbook)
        {
            string content = string.Empty;
            foreach (var sheet in workbook.Worksheets)
            {
                Cells cells = sheet.Cells;
                for (int i = 0; i < cells.MaxDataRow + 1; i++)
                {
                    for (int j = 0; j < cells.MaxDataColumn + 1; j++)
                    {
                        content += cells[i, j].StringValue.Trim();
                    }
                }
            }

            return content;
        }
    }
}