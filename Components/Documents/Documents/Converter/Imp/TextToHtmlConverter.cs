using System;
using System.IO;
using System.Text;
using Common.Logging;
using Documents.Exceptions;
using Documents.Utils;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// Text转换为HTML
    /// </summary>
    public class TextToHtmlConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        
        public int Convert(string from, string to)
        {
            _logger.DebugFormat("Text转换为HTML, {0},到:{1}", from, to);

            try
            {
                var encoding = EncodingUtils.GetFileEncode(from);
                string result = File.ReadAllText(from, encoding);
                File.WriteAllText(to, result, Encoding.UTF8);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.TextToHtmlFailed, ex);
            }
        }

        public int Convert(Stream from, Stream to)
        {
            try
            {
                //var encoding = EncodingUtils.GetFileEncode(from);
                from.CopyTo(to);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadTextFailed, ex);
            }
        }
    }
}
