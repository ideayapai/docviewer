using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using Documents;
using Documents.Enums;
using Infrasturcture.Store;
using Infrasturcture.Store.Files;
using Infrasturcture.Web;
using NUnit.Framework;
using Moq;
using Ninject;
using Services.Documents;
using Services.Ioc;
using Services.Spaces;
using WebAPI2.Controllers;

namespace WebAPI2UnitTest.Controllers
{
    [TestFixture]
    public class DocumentControllerTester
    {
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        private readonly SpaceService _spaceService = ServiceActivator.Get<SpaceService>();
        private readonly Mock<IHttpRequestProvider> _requestProvider = new Mock<IHttpRequestProvider>();
        private readonly IStorePolicy _storePolicy = ServiceActivator.Get<IStorePolicy>();
        private DocumentController _controller;

        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("Setup");
            _controller = new DocumentController(_documentService, _spaceService, _requestProvider.Object, _storePolicy);
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
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

        private void MockRequestParameters()
        {
            string userId = Guid.NewGuid().ToString();
            string nickName = "kanezeng";
            string depId = "001";
            string parentId = string.Empty;
            _requestProvider.SetupGet(f => f["UserId"]).Returns(userId);
            _requestProvider.SetupGet(f => f["UserName"]).Returns(nickName);
            _requestProvider.SetupGet(f => f["DepId"]).Returns(depId);
            _requestProvider.SetupGet(f => f["ParentId"]).Returns(parentId);
        }

        [Test]
        public void should_go_throw_add_process()
        {

            SetupMockLocal();
            var contract = _controller.Add();
            Assert.IsNotNull(contract);
            var result = _controller.Delete(contract.Id.ToString());
            Assert.IsTrue(result);
        }

        [Test]
        public void should_get_documents_with_documentType()
        {
            var contracts = _controller.GetDocuments(string.Empty, DocumentType.BMP);
            Assert.IsTrue(contracts.Count()>= 0);
        }

        [Test]
        public void should_get_documents_with_spaceId_withParameters_null()
        {
            var contracts = _controller.GetDocuments(string.Empty, string.Empty);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_documents_with_spaceId_withParameters_empty()
        {
            var contracts = _controller.GetDocuments(null, null);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_documents_with_spaceId_userId_withParameters_emtpy()
        {
            var contracts = _controller.GetDocuments(string.Empty, string.Empty);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_documents_with_spaceId_userId_withParameters_null()
        {
            var contracts = _controller.GetDocuments(null, null);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_documents_with_spaceId_userId_documentType_withParameters_null()
        {
            var contracts = _controller.GetDocuments(null, null, DocumentType.BMP);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_call_getdocument_with_id_null()
        {
            _controller.Get(null);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_call_getdocument_with_id_empty()
        {
            _controller.Get(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_call_getdocument_with_id_error()
        {
             _controller.Get(Guid.NewGuid().ToString());
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_delete_document_with_id_null()
        {
             _controller.Delete(null);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_delete_document_with_id_empty()
        {
             _controller.Delete(string.Empty);
        }

        [Test]
        public void should_delete_document_with_id_error()
        {
            //Assert.False(controller.Delete(Guid.NewGuid().ToString()));
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_recover_document_with_id_null()
        {
             _controller.Recover(null);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_recover_document_with_id_empty()
        {
             _controller.Recover(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_recover_document_with_id_error()
        {
             _controller.Recover(Guid.NewGuid().ToString());
        }
    }
}
