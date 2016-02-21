using System;
using System.IO;
using Aspose.Slides;
using Aspose.Slides.Export;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// PPT转换为PDF
    /// </summary>
    public class PPTToHtmlConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("PPT转换为HTML {0} to {1}", from, to);
            
            try
            {
                Presentation ppt = new Presentation(from);
                ppt.Save(to, SaveFormat.Html);
                return ErrorMessages.Success;
            }
           
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PPTToHtmlFailed, ex);
            }
            
        }

        public int Convert(Stream from, Stream to)
        {
            try
            {
                Presentation ppt = new Presentation(from);
                ppt.Save(from, SaveFormat.Html);
                return ErrorMessages.Success;
            }

            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PPTToHtmlFailed, ex);
            }
        }
    }
}