//using Microsoft.Office.Interop.Excel;

using System;
using System.IO;
using System.Runtime.InteropServices;
using Common.Logging;
using Infrasturcture.Errors;
using Microsoft.Office.Interop.Excel;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// 转换Excel为PDF
    /// </summary>
    public class ExcelToPdfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
            {
                return ErrorMessages.FileNotExist;
            }
            XlFixedFormatType targetType = XlFixedFormatType.xlTypePDF;

            object missing = Type.Missing;
            ApplicationClass application = new ApplicationClass(); ;
            Workbook workBook = null;
            try
            {
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                                                      missing, missing, missing, missing, missing, missing, missing, missing, missing);
                //设置格式，导出成PDF 
                Worksheet sheet = (Worksheet)workBook.Worksheets[1]; //下载从1开始 
                //把sheet设置成横向 
                sheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                //可以设置sheet页的其他相关设置，不列举                     
                sheet.ExportAsFixedFormat(targetType,
                                            targetPath,
                                            XlFixedFormatQuality.xlQualityStandard,
                                            true,
                                            false,
                                            missing,
                                            missing,
                                            missing,
                                            missing);

                return ErrorMessages.ConvertSuccess;
            }
            catch (COMException ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
                return ErrorMessages.OfficeToPdfUninstall;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return ErrorMessages.ConvertFailed;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                }
                if (application != null)
                {
                    application.Quit();
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}