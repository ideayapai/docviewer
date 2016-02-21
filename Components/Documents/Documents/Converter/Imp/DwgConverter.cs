using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// DWG转换器
    /// </summary>
    public class DwgConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("转换DWG格式的文件 {0} 到 {1}", from, to);

            try
            {
             
                using (var process = new Process())
                {
                    var swgTosvgexePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Tools\Dwg\DwgConverter.exe");
                    string argements = string.Format("  -t  {0} -s {1}", from, to);
                   
                    //调用新进程 进行转换
                    var psi = new ProcessStartInfo(swgTosvgexePath, argements);
                    process.StartInfo = psi;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                }
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.DwgConvertFailed, ex);
            }
          
        }

        public int Convert(Stream @from, Stream to)
        {
            throw new NotImplementedException();
        }
    }
}
