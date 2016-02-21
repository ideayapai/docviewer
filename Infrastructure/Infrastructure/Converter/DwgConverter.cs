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
    public class DwgConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string savePath)
        {
            _logger.DebugFormat("DwgToSwfConverter Transform from {0} to {1}", sourcePath, savePath);

            try
            {
                if (!File.Exists(sourcePath))
                {
                    return ErrorMessages.FileNotExist;
                }

                using (var process = new Process())
                {
                    var swgTosvgexePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Tools\Dwg\DwgConverter.exe");
                    string argements = string.Format("  -t  {0} -s {1}", sourcePath, savePath);
                   
                    //调用新进程 进行转换
                    var psi = new ProcessStartInfo(swgTosvgexePath, argements);
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



    }
}
