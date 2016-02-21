using System;
using System.IO;
using System.Runtime.InteropServices;
using Common.Logging;
using Infrasturcture.Errors;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// PPT转换为PDF
    /// </summary>
    public class PPTToPdfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
            {
                return ErrorMessages.FileNotExist;
            }

            ApplicationClass application = null;
            Presentation presentation = null;
            try
            {
                application = new ApplicationClass();
                presentation = application.Presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse,
                    MsoTriState.msoFalse);
                presentation.SaveAs(targetPath, PpSaveAsFileType.ppSaveAsPDF, MsoTriState.msoTrue);
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
                if (presentation != null)
                {
                    presentation.Close();
                }
                if (application != null)
                {
                    application.Quit();
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}