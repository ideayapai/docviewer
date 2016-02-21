﻿//using Microsoft.Office.Interop.Excel;

using System;
using System.IO;
using Aspose.Cells;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换Excel为PDF
    /// </summary>
    public class ExcelToPdfConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("Excel转换为PDF, {0} to {1}", from, to);

            try
            {
                Workbook workBook = new Workbook(from);
                workBook.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }
           
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.ExcelToPdfFailed, ex);
            }
           
        }

        public int Convert(Stream from, Stream to)
        {
            try
            {
                Workbook workBook = new Workbook(from);
                workBook.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }

            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.ExcelToPdfFailed, ex);
            }
        }
    }
}