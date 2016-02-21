using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换PDF为SWF
    /// </summary>
    public class PdfToSwfConverter : IConverter
    {
        readonly string pdf2Swf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                    @"Tools\pdf2swf.exe");
                
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("PDF转换为SWF from {0} to {1}", from, to);

            if (!File.Exists(pdf2Swf))
            {
                throw new ConverterException(ErrorMessages.Pdf2SwfToolNotExist);
            }

            try
            {

                _logger.Debug("开始转换...");
                using (var process = new Process())
                {                    
                    var argsStr = GetFlash9Arguments(from, to);

                    _logger.Debug(argsStr);
                    //调用新进程 进行转换
                    var psi = new ProcessStartInfo(pdf2Swf, argsStr);
                    process.StartInfo = psi;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                 
                    process.WaitForExit();

                    _logger.Debug("转换结束...");

                    if (!File.Exists(to))
                    {
                        _logger.ErrorFormat("转换后的文件{0}后文件不存在", to);
                        throw new ConverterException(ErrorMessages.ConvertedFileNotExist);
                    }
                    return ErrorMessages.Success;
                }
               
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PPTToSwfFailed, ex);
            }
        }


        private string GetFlash9Arguments(string fileName, string savePath)
        {
            return "  -t " + fileName + " -s flashversion=9 -o " + StringUtils.RemoveAllEmpty(savePath);
        }

   
    }
}
