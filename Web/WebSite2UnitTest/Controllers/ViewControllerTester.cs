using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Documents;
using Documents.Converter;
using Documents.Enums;
using Infrasturcture.Errors;
using Infrasturcture.Store;
using Infrasturcture.Store.Files;
using Infrasturcture.Web;
using NUnit.Framework;
using Moq;
using Services.Context;
using Services.Contracts;
using Services.Documents;
using Services.Ioc;
using Services.Spaces;
using WebSite2.Controllers;
using WebSite2.Models;

namespace WebSite2UnitTest.Controllers
{
    [TestFixture]
    public class ViewControllerTester
    {
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        private readonly SpaceService _spaceService = ServiceActivator.Get<SpaceService>();
        private readonly Mock<ContextService> _contextService = new Mock<ContextService>();
        private readonly Mock<IHttpRequestProvider> _requestProvider = new Mock<IHttpRequestProvider>();
        private readonly SpaceTreeService _spaceTreeService = ServiceActivator.Get<SpaceTreeService>();
        private readonly IStorePolicy _storePolicy = ServiceActivator.Get<IStorePolicy>();
        private DocumentController _documentController;
        private ViewController _viewController;

        [SetUp]
        public void SetUp()
        {
            _viewController = new ViewController(_documentService, _spaceService, _storePolicy, _contextService.Object);
            _documentController = new DocumentController(_documentService, _spaceService, _spaceTreeService, _storePolicy, _requestProvider.Object, _contextService.Object);
        }

        [Test]
        public void should_return_not_found_view_if_parameter_is_null()
        {
            var viewResult = _viewController.Index(null) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "NotFound");
        }

        [Test]
        public void should_return_not_found_view_if_parameter_is_empty()
        {
            var viewResult = _viewController.Index(string.Empty) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "NotFound");
        }

        [Test]
        public void should_return_not_found_view_if_parameter_is_not_exist()
        {
            var viewResult = _viewController.Index(Guid.NewGuid().ToString()) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "NotFound");
        }


        [Test]
        public void should_return_not_found_view_if_parameter_is_null_when_call_getview()
        {
            var viewResult = _viewController.Preview(null) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "NotFound");
        }

        [Test]
        public void should_return_not_found_view_if_parameter_is_empty_when_call_getview()
        {
            var viewResult = _viewController.Index(string.Empty) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "NotFound");
        }

        [Test]
        public void should_return_get_index_view()
        {
            SetupMockLocal();

            var documents = TestUpload();

            foreach (var document in documents)
            {
                var viewResult = _viewController.Index(document.Id.ToString()) as ViewResult;
                var model = viewResult.Model as DisplayViewModel;
                Assert.AreEqual(viewResult.ViewName, string.Empty);
                Assert.AreEqual(model.Document.Id, document.Id);
            }
          

            TestClear(documents);
        }

        [Test]
        public void should_return_get_office_view()
        {
            //SetupMockLocal();

            //var documents = TestUpload();

            //foreach (var document in documents)
            //{
            //    var viewResult = _viewController.Preview(document.Id.ToString()) as ViewResult;
            //    var model = viewResult.Model as DisplayViewModel;
            //    if (model.Document.DocumentType.ToConvertType() == ConvertFileType.WordToHtml)
            //    {
            //        Assert.AreEqual(viewResult.ViewName, "Html");
            //    }
             
            //    Assert.AreEqual(model.Document.Id, document.Id);

            //    Console.WriteLine("Id:" + model.Document.Id);
            //    Console.WriteLine("Name:" + model.Document.FileName);
            //    Console.WriteLine("SpaceName:" + model.Document.SpaceName);
            //}


            //TestClear(documents);
        }


        private void TestClear(List<DocumentObject> documents)
        {
            string ids;
            string types;
            GetIdTypes(documents, out ids, out types);

            _requestProvider.SetupGet(f => f["ids"]).Returns(ids);
            _requestProvider.SetupGet(f => f["types"]).Returns(types);

            var jsonResult = _documentController.Clear() as JsonResult;
            Assert.IsTrue((bool)jsonResult.Data);
        }

        private void GetIdTypes(List<DocumentObject> documents, out string ids, out string types)
        {
            ids = string.Empty;
            types = string.Empty;
            foreach (var document in documents)
            {
                ids += document.Id;
                ids += "|";
                types += "file";
                types += "|";
            }
        }

        private List<DocumentObject> TestUpload()
        {
            var jsonResult = _documentController.Add() as JsonResult;
            var uploadViewModel = jsonResult.Data as UploadViewModel;

            Assert.AreEqual(uploadViewModel.Status, ErrorMessages.Success);
            Assert.AreEqual(uploadViewModel.ErrorMessage, ErrorMessages.GetErrorMessages(ErrorMessages.Success));

            return uploadViewModel.Documents;
        }

        private void SetupMockLocal()
        {
            MockRequestParameters();

            var collection = new BaseFileCollection();
            var files = Directory.GetFiles(Environment.CurrentDirectory + @"\TestFiles");
            foreach (var file in files)
            {
                collection.Add(new LocalFile(file));
            }

            _requestProvider.SetupGet(f => f.FileCollection).Returns(collection);
        }

        private void SetupHttpMock()
        {
            MockRequestParameters();

            _requestProvider.SetupGet(f => f.FileCollection).Returns(new Mock<BaseFileCollection>().Object);
        }

        private void MockRequestParameters()
        {
            string userId = Guid.NewGuid().ToString();
            string nickName = "nickName";
            string depId = "001";
            string spaceName = "testspace";
            string parentId = string.Empty;

            _contextService.SetupGet(f => f.UserId).Returns(userId);
            _contextService.SetupGet(f => f.NickName).Returns(nickName);
            _contextService.SetupGet(f => f.DepId).Returns(depId);
            _requestProvider.SetupGet(f => f["SpaceName"]).Returns(spaceName);
            _requestProvider.SetupGet(f => f["ParentId"]).Returns(parentId);
        }
    }
}
