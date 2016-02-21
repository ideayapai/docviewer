using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
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
    public class DocumentControllerTester
    {
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        private readonly SpaceService _spaceService = ServiceActivator.Get<SpaceService>();
        private readonly SpaceTreeService _spaceTreeService = ServiceActivator.Get<SpaceTreeService>();
        private readonly Mock<IHttpRequestProvider> _requestProvider = new Mock<IHttpRequestProvider>();
        private readonly Mock<ContextService> _contextService = new Mock<ContextService>();
        private readonly IStorePolicy _storePolicy = ServiceActivator.Get<IStorePolicy>();
        private DocumentController _documentController;

        [SetUp]
        public void SetUp()
        {
            _documentController = new DocumentController(_documentService, _spaceService, _spaceTreeService, _storePolicy, _requestProvider.Object, _contextService.Object);
        }

        [Test]
        public void should_return_recent_view()
        {
            var viewResult = _documentController.Recent() as ViewResult;

            var model = viewResult.Model as DocumentIndexViewModel;
            //Assert.(model.DocumentModels.Count, 0);
            Assert.AreEqual(model.ActiveMenuType, MenuType.Recent);
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }

        [Test]
        public void should_return_cad_view()
        {
            var viewResult = _documentController.CAD(3) as ViewResult;

            var model = viewResult.Model as DocumentIndexViewModel;
            //Assert.GreaterOrEqual(model.DocumentModels.Count, 0);
            Assert.AreEqual(model.ActiveMenuType, MenuType.Cad);
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }

        [Test]
        public void should_return_office_view()
        {
            var viewResult = _documentController.Office(3) as ViewResult;

            var model = viewResult.Model as DocumentIndexViewModel;
            //Assert.GreaterOrEqual(model.DocumentModels.Count, 0);
            Assert.AreEqual(model.ActiveMenuType, MenuType.Office);
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }

        [Test]
        public void should_return_image_view()
        {
            var viewResult = _documentController.Image(3) as ViewResult;

            var model = viewResult.Model as DocumentIndexViewModel;
            //Assert.GreaterOrEqual(model.DocumentModels.Count, 0);
            Assert.AreEqual(model.ActiveMenuType, MenuType.Image);
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }

        [Test]
        public void should_return_trash_view()
        {
            var viewResult = _documentController.Trash(3) as ViewResult;

            var model = viewResult.Model as TrashViewModel;
            //Assert.GreaterOrEqual(model.DocumentModels.Count, 0);
            Assert.AreEqual(model.ActiveMenuType, MenuType.Trash);
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }

        [Test]
        public void should_use_mock_http_file_to_upload()
        {
            SetupHttpMock();

            var jsonResult = _documentController.Add() as JsonResult;
 
            var uploadViewModel = jsonResult.Data as UploadViewModel;
            Assert.AreEqual(uploadViewModel.Status, ErrorMessages.Success);
            Assert.AreEqual(uploadViewModel.ErrorMessage, ErrorMessages.GetErrorMessages(ErrorMessages.Success));
        }

        //[Test]
        //public void should_use_mock_local_file_to_upload_and_clear()
        //{
        //    SetupMockLocal();

        //    var documents = TestUpload();
   
        //    TestClear(documents);
        //}

        //[Test]
        //public void should_use_mock_local_file_to_upload_and_clear_with_rename()
        //{
        //    SetupMockLocal();

        //    var documents = TestUpload();
        //    foreach (var document in documents)
        //    {
        //        var result = _documentController.ReName(document.Id.ToString(), "file", "newfile") as JsonResult;
        //        var viewModel = result.Data as UpdateViewModel;
        //        Assert.AreEqual(viewModel.Status, ErrorMessages.Success);
        //        Assert.AreEqual(viewModel.FileType, "file");
        //    }
            
        //    TestClear(documents);
        //}

        //[Test]
        //public void should_use_mock_local_file_to_upload_and_trash_and_recovery()
        //{
        //    SetupMockLocal();

        //    var documents = TestUpload();
        //    string ids;
        //    string types;
        //    GetIdTypes(documents, out ids, out types);

        //    _requestProvider.SetupGet(f => f["ids"]).Returns(ids);
        //    _requestProvider.SetupGet(f => f["types"]).Returns(types);

        //    var jsonResult = _documentController.DeleteList() as JsonResult;
        //    var data = jsonResult.Data as string;

        //    Assert.AreEqual(data, ids);

        //    TestClear(documents);
        //}

        //[Test]
        //public void should_upload_and_download_file()
        //{
        //    SetupMockLocal();

        //    var documents = TestUpload();


        //    var fileResult = _documentController.Download(documents[0].Id.ToString());
            

        //    TestClear(documents);
        //}

        private void TestClear(List<DocumentObject> documents)
        {
            string ids;
            string types;
            GetIdTypes(documents,out ids, out types);

            _requestProvider.SetupGet(f => f["ids"]).Returns(ids);
            _requestProvider.SetupGet(f => f["types"]).Returns(types);

            var jsonResult = _documentController.Clear() as JsonResult;
            Assert.IsTrue((bool) jsonResult.Data);
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
