using System.IO;
using Common.Logging;
using Documents.Enums;
using Documents.Exceptions;
using Documents.Reader.Imp;
using TextReader = Documents.Reader.Imp.TextReader;

namespace Documents.Reader
{
    /// <summary>
    /// 文档阅读器
    /// 读取文档的概要，内容
    /// </summary>
    public class DocumentReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ReaderContainer _container = new ReaderContainer();

        public DocumentReader()
        {
            _container.Register<ExcelReader>(DocumentType.Excel);
            _container.Register<WordReader>(DocumentType.Word);
            _container.Register<WordReader>(DocumentType.RTF);
            _container.Register<PDFReader>(DocumentType.PDF);
            _container.Register<PPTReader>(DocumentType.PPT);
            _container.Register<TextReader>(DocumentType.TXT);
        }

        public string Read(string filePath)
        {
            _logger.InfoFormat("读取{0}的内容:", filePath);

            if (!File.Exists(filePath))
            {
                _logger.InfoFormat("文件{0}不存在:", filePath);
                throw new FileNotFoundException();
            }

            try
            {
                var documentType = filePath.ToDocumentType();
                IReader reader = _container.Resolve<IReader>(documentType);
                return reader.Read(filePath);
            }
            catch(ReadException ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }  
        }

     

        public string Read(Stream stream, DocumentType documentType)
        {
            try
            {
                IReader reader = _container.Resolve<IReader>(documentType);
                return reader.Read(stream);
            }
            catch (ReadException ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }
    }

    


}
