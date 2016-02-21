using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// 转换PDF为SWF
    /// </summary>
    public class PdfToSwfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string savePath)
        {
            _logger.DebugFormat("PdfToSwfConverter Transform from {0} to {1}", sourcePath, savePath);

            try
            {
                if (!File.Exists(sourcePath))
                {
                    return ErrorMessages.FileNotExist;
                }

                using (var process = new Process())
                {
                    var pdf2SwfexePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Tools\pdf2swf.exe");

                    var argsStr = GetFlash9Arguments(sourcePath, savePath); 

                    //调用新进程 进行转换
                    var psi = new ProcessStartInfo(pdf2SwfexePath, argsStr);
                    process.StartInfo = psi;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                 
                    process.WaitForExit();
                }
                return ErrorMessages.ConvertSuccess;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return ErrorMessages.ConvertFailed;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private string GetFlash9Arguments(string fileName, string savePath)
        {
            return "  -t " + fileName + " -s flashversion=9 -o " + StringUtils.RemoveAllEmpty(savePath);
        }

   
    }
}
