using System;
using System.IO;
using System.Net;
using Common.Logging;
using Documents.Converter.Imp;
using Documents.Enums;
using Documents.Exceptions;
using Documents.Utils;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Documents.Converter
{
   
    /// <summary>
    /// 文档转换器
    /// word,excel,ppt,pdf->swf
    /// dwg->svg等
    /// </summary>
    public class DocumenConverter
    {
        private readonly ConverterContainer _container = new ConverterContainer();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DocumenConverter()
        {
            _container.Register<WordToHtmlConverter>(ConvertFileType.WordToHtml);
            _container.Register<WordToFormatHtmlConverter>(ConvertFileType.WordPDFtoHtml);
            _container.Register<WordToPdfConverter>(ConvertFileType.WordToPdf);
            _container.Register<ExcelToHtmlConverter>(ConvertFileType.ExcelToHtml);
            _container.Register<ExcelToFormatHtmlConverter>(ConvertFileType.ExcelPDFToHtml);
            _container.Register<ExcelToPdfConverter>(ConvertFileType.ExcelToPdf);
            _container.Register<PPTToHtmlConverter>(ConvertFileType.PPTToHtml);
            _container.Register<PPTToSwfConverter>(ConvertFileType.PPTPDFToHtml);
            _container.Register<PPTToPdfConverter>(ConvertFileType.PPTToPdf);
            _container.Register<PdfToHtmlConverter>(ConvertFileType.PdfToHtml);
            _container.Register<TextToHtmlConverter>(ConvertFileType.TextToHtml);
            _container.Register<TextToPdfConverter>(ConvertFileType.TextToPdf);
            _container.Register<TextToSwfConverter>(ConvertFileType.TextPDFToHtml);
            _container.Register<DwgConverter>(ConvertFileType.CadToBmp);
            _container.Register<DwgConverter>(ConvertFileType.CadToSvg);
            _container.Register<DwgConverter>(ConvertFileType.CadToJpg);
            _container.Register<DwgConverter>(ConvertFileType.CadToPng);
            _container.Register<TextToHtmlConverter>(ConvertFileType.None);
        }

        public int Convert(string from, string to, ConvertFileType convertFileType)
        {
            _logger.Debug("转换文件 : " + from + "到: " + to);

            if (!File.Exists(from))
            {
                throw new ConverterException(ErrorMessages.FileNotExist);
            }

            try
            {
                IConverter converter = _container.Resolve<IConverter>(convertFileType);
                return converter.Convert(@from, to);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
                throw;
            }
           
        }

        public int Convert(Stream from, Stream to, ConvertFileType convertFileType)
        {
            _logger.Debug("转换文件 : " + from + "到: " + to);

            try
            {
                IConverter converter = _container.Resolve<IConverter>(convertFileType);
                return converter.Convert(from, to);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
                throw;
            }
        }
    }

    


}
