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
    public class PPTToPdfConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string @from, string @to)
        {
            _logger.DebugFormat("PPT 转换为Pdf, @from:{0},@to:{1}", @from, @to);

            try
            {
                Presentation ppt = new Presentation(@from);
                ppt.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PPTToPdfFailed, ex);
            }
           
        }

        public int Convert(Stream @from, Stream to)
        {
            try
            {
                Presentation ppt = new Presentation(@from);
                ppt.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PPTToPdfFailed, ex);
            }
        }
    }
}