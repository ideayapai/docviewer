using System;
using Documents.Converter;
using Documents.Enums;
using Infrasturcture.Errors;
using Infrasturcture.Store;
using Infrasturcture.Store.Local;
using NUnit.Framework;
using Services.Contracts;
using Services.Documents;
using Services.Ioc;

namespace Services.Tests
{
    [TestFixture]
    public class DocumentConvertServiceTester
    {
        private readonly IStorePolicy _storePolicy = ServiceActivator.Get<LocalPolicy>();
        private readonly DocumenConverter _documenConverter = ServiceActivator.Get<DocumenConverter>();
        private DocumentConvertService _documentConvertService;

        [SetUp]
        public void SetUp()
        {
            _documentConvertService = new DocumentConvertService(_documenConverter, _storePolicy);
        }

        [Test]
        public void should_convert_pdf_file()
        {  
            string sourcePath = Environment.CurrentDirectory + @"\TestFiles\Guidance_User_Manual_v1.2_cn.pdf";
            var document = new DocumentObject
            {
                StorePath = sourcePath,
                DocumentType = DocumentType.PDF,
            };

            var result = _documentConvertService.Convert(ref document, ConvertFileType.PdfToHtml);
            Assert.AreEqual(result.ErrorCode, ErrorMessages.Success);
        }
    }
}
