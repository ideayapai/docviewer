using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using Infrasturcture.Errors;

namespace SWFConverter
{
    /// <summary>
    /// 转换PDF为SWF
    /// </summary>
    public class SwfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string savePath)
        {
            _logger.DebugFormat("SwfConverter Transform from {0} to {1}", sourcePath, savePath);

            try
            {
                if (!File.Exists(sourcePath))
                {
                    return ErrorMessages.FileNotExist;
                }

                _logger.Debug("Start Upload");

                var pdf2SwfexePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"pdf2swf.exe");
                if (!File.Exists(pdf2SwfexePath))
                {
                    _logger.Error("pdf2swf is not exist");

                    return ErrorMessages.Pdf2SwfToolNotExist;
                }
                using (var process = new Process())
                {
                    var argsStr = GetFlash9Arguments(sourcePath, savePath);

                    _logger.Debug(argsStr);
                    //调用新进程 进行转换
                    var psi = new ProcessStartInfo(pdf2SwfexePath, argsStr);
                    process.StartInfo.UseShellExecute = false;
                    if (Environment.OSVersion.Version.Major >= 6)
                        process.StartInfo.Verb = "runas";
                    process.StartInfo = psi;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    process.WaitForExit();

                    _logger.Debug("End Upload");

                }
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return ErrorMessages.ExcelToHtmlFailed;
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
            return fileName + " -o " + RemoveAllEmpty(savePath);
        }

        public static string RemoveAllEmpty(string savePath)
        {
            int preLength = savePath.Length;
            int nowLength = 0;
            for (; nowLength < preLength; )
            {
                preLength = savePath.Length;
                savePath = savePath.Replace(" ", "");
                nowLength = savePath.Length;
            }
            return savePath;
        }

    }
}
