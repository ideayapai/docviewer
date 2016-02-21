using System;
using Documents.Enums;
using Ninject;
using NUnit.Framework;
using Services.Contracts;
using Services.Documents;
using Services.Enums;
using Services.Ioc;

namespace Services.Tests
{
    [TestFixture]
    public class DocumentServiceTester
    {
        private readonly DocumentService _documentService = ServiceActivator.Kernel.Get<DocumentService>();

        [Test]
        public void should_get_all_documents()
        {
            var documents = _documentService.GetAllUnDeleteDocuments();
            Assert.IsNotNull(documents);
        }

        [Test]
        public void should_pass_add_document_without_downloadPath()
        {
            int count = _documentService.GetAllDocuments().Count;

            var document = MakeSimpleDocument();
            Assert.IsNotNull(_documentService.Add(document));
            var result = _documentService.GetDocument(document.Id.ToString());


            Assert.AreEqual(result.FileName, "Test.doc");
      
            _documentService.Delete(document.Id.ToString());
            result = _documentService.GetDocument(document.Id.ToString());
            Assert.IsNull(result);

            int count2 = _documentService.GetAllDocuments().Count;
            Assert.AreEqual(count, count2);
        }


        [Test]
        public void should_pass_add_update_get_delete_process()
        {
            int count = _documentService.GetAllDocuments().Count;

            var document = new DocumentObject
                               {
                                   Id = Guid.NewGuid(),
                                   FileName = "Test.doc",
                                   DisplayPath = @"C:\Test.doc",
                                   StorePath = @"C:\Test.swf",
                                   FileSize = 1080,
                             
                                   CreateTime = DateTime.Now,
                                   UpdateTime = DateTime.Now,
                                   DocumentType = DocumentType.Word,
                                   CreateUserId = "5",
                                   CreateUserName = "kane",
                                   DepId = Guid.NewGuid().ToString(),
                                   SpaceId = Guid.NewGuid().ToString(),
                                   SpaceSeqNo = Guid.NewGuid().ToString(),
                                   SpaceName = Guid.NewGuid().ToString(),
                                   UpdateUserId = "5",
                                   UpdateUserName = "kane",
                               };
            Assert.IsNotNull(_documentService.Add(document));
            var result = _documentService.GetDocument(document.Id.ToString());


            Assert.AreEqual(result.FileName, "Test.doc");
            document.StorePath = @"D:\Test.swf";
          

            Assert.IsNotNull(_documentService.Update(document));
            document = _documentService.GetDocument(document.Id.ToString());
            Assert.AreEqual(document.StorePath, @"D:\Test.swf");
         

            document = _documentService.MoveToTrash(document.Id.ToString());
            document = _documentService.GetDocument(document.Id.ToString());
            Assert.IsTrue(document.IsDelete);

            document = _documentService.Recovery(document.Id.ToString());
            document = _documentService.GetDocument(document.Id.ToString());
            Assert.IsFalse(document.IsDelete);

            _documentService.Delete(document.Id.ToString());
            result = _documentService.GetDocument(document.Id.ToString());
            Assert.IsNull(result);

            int count2 = _documentService.GetAllDocuments().Count;
            Assert.AreEqual(count, count2);
            
        }

        [Test]
        public void should_pass_visiblity_public_process()
        {
            int count = _documentService.GetAllDocuments().Count;

            string userId = Guid.NewGuid().ToString();
            string userId2 = Guid.NewGuid().ToString();

            var document1 = MakePublicDocument(userId);
            var document2 = MakePublicDocument(userId2);

            Assert.IsNotNull(_documentService.Add(document1));
            Assert.IsNotNull(_documentService.Add(document2));

            var documents = _documentService.GetPrivateDocuments(userId);
            Console.WriteLine("Private Documents:" + documents.Count);
            Assert.AreEqual(documents.Count, 1);

            documents = _documentService.GetVisibleDocuments(userId);
            Console.WriteLine("Visible Documents:" + documents.Count);
            Assert.IsTrue(documents.Count > 1);


            _documentService.Delete(document1.Id.ToString());
            Assert.IsNull(_documentService.GetDocument(document1.Id.ToString()));

            _documentService.Delete(document2.Id.ToString());
            Assert.IsNull(_documentService.GetDocument(document2.Id.ToString()));

            int count2 = _documentService.GetAllDocuments().Count;
            Assert.AreEqual(count, count2);
        }

        [Test]
        public void should_pass_visiblity_dep_process()
        {
            int count = _documentService.GetAllDocuments().Count;

            string userId = Guid.NewGuid().ToString();
            string userId2 = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var document1 = MakeDepDocument(userId, depId);
            var document2 = MakeDepDocument(userId2, depId);

            Assert.IsNotNull(_documentService.Add(document1));
            Assert.IsNotNull(_documentService.Add(document2));

            
            var documents = _documentService.GetPrivateDocuments(userId);
            Console.WriteLine("Private Documents:" + documents.Count);
            Assert.AreEqual(documents.Count, 1);

            documents = _documentService.GetVisibleDocuments(userId, depId);
            int visibleCount = documents.Count;
            Console.WriteLine("Visible Documents Before Set Private:" + visibleCount);

            //设置私有权限
            _documentService.SetVisiblity(document2.Id.ToString(), Visible.Private);
            documents = _documentService.GetVisibleDocuments(userId, depId);
            int visibleCount2 = documents.Count;
            Console.WriteLine("Visible Documents After Set Private:" + visibleCount2);
            Assert.AreEqual(visibleCount - visibleCount2, 1);

            //设置为部门权限
            _documentService.SetVisiblity(document2.Id.ToString(), Visible.Dep);
            documents = _documentService.GetVisibleDocuments(userId, depId);
            int visibleCount3 = documents.Count;
            Console.WriteLine("Visible Documents After Set Dep:" + visibleCount3);
            Assert.AreEqual(visibleCount - visibleCount3, 0);

            //设置为公共权限
            _documentService.SetVisiblity(document2.Id.ToString(), Visible.Public);
            documents = _documentService.GetVisibleDocuments(userId, depId);
            int visibleCount4 = documents.Count;
            Console.WriteLine("Visible Documents After Set Public:" + visibleCount4);
            Assert.AreEqual(visibleCount - visibleCount4, 0);

            _documentService.Delete(document1.Id.ToString());
            Assert.IsNull(_documentService.GetDocument(document1.Id.ToString()));

            _documentService.Delete(document2.Id.ToString());
            Assert.IsNull(_documentService.GetDocument(document2.Id.ToString()));

            int count2 = _documentService.GetAllDocuments().Count;
            Assert.AreEqual(count, count2);
        }

        private static DocumentObject MakeSimpleDocument()
        {
            var document = new DocumentObject
            {
                Id = Guid.NewGuid(),
                FileName = "Test.doc",
                DisplayPath = @"C:\Test.doc",
                StorePath = @"C:\Test.swf",
                FileSize = 1080,
          
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                DocumentType = DocumentType.Word,
                CreateUserId = "5",
                DepId = Guid.NewGuid().ToString(),
                CreateUserName = "kane",
                SpaceId = Guid.NewGuid().ToString(),
                SpaceSeqNo = Guid.NewGuid().ToString(),
                SpaceName = Guid.NewGuid().ToString(),
                UpdateUserId = "5",
                UpdateUserName = "kane",
            };
            return document;
        }

        private static DocumentObject MakeDepDocument(string userId, string depId)
        {
            var document = new DocumentObject
            {
                Id = Guid.NewGuid(),
                FileName = "Test.doc",
                DisplayPath = @"C:\Test.doc",
                StorePath = @"C:\Test.swf",
                FileSize = 1080,
           
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                DocumentType = DocumentType.Word,
                CreateUserId = userId,
                Visible = (int)Visible.Dep,
                DepId = depId,
                CreateUserName = "kane",
                SpaceId = Guid.NewGuid().ToString(),
                SpaceSeqNo = Guid.NewGuid().ToString(),
                SpaceName = Guid.NewGuid().ToString(),
                UpdateUserId = userId,
                UpdateUserName = "kane",
            };
            return document;
        }

        private static DocumentObject MakePublicDocument(string userId)
        {
            var document = new DocumentObject
            {
                Id = Guid.NewGuid(),
                FileName = "Test.doc",
                DisplayPath = @"C:\Test.doc",
                StorePath = @"C:\Test.swf",
                FileSize = 1080,
          
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                DocumentType = DocumentType.Word,
                CreateUserId = userId,
                Visible = (int)Visible.Public,
                DepId = Guid.NewGuid().ToString(),
                CreateUserName = "kane",
                SpaceId = Guid.NewGuid().ToString(),
                SpaceSeqNo = Guid.NewGuid().ToString(),
                SpaceName = Guid.NewGuid().ToString(),
                UpdateUserId = userId,
                UpdateUserName = "kane",
            };
            return document;
        }
      
    }
}
