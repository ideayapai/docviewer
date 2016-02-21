using System;
using System.Web.Mvc;
using Infrasturcture.Web;
using NUnit.Framework;
using Moq;
using Services.Context;
using Services.Contracts;
using Services.Ioc;
using Services.Spaces;
using WebSite2.Controllers;
using WebSite2.Models;

namespace WebSite2UnitTest.Controllers
{
    [TestFixture]
    public class SpaceControllerTester
    {
        private readonly SpaceService _spaceService = ServiceActivator.Get<SpaceService>();
        private readonly SpaceTreeService _spaceTreeService = ServiceActivator.Get<SpaceTreeService>();
        private readonly Mock<IHttpRequestProvider> _requestProvider = new Mock<IHttpRequestProvider>();
        private readonly Mock<ContextService> _contextService = new Mock<ContextService>();

        private SpaceController _spaceController;

        [SetUp]
        public void SetUp()
        {
            _spaceController = new SpaceController(_spaceService, _spaceTreeService, _requestProvider.Object, _contextService.Object);
        }

        [Test]
        public void should_return_index_view()
        {
            var viewResult = _spaceController.Index() as ViewResult;
            Assert.AreEqual(viewResult.ViewName, string.Empty);
        }


        [Test]
        public void should_return_add_view()
        {
            string Id = Guid.NewGuid().ToString();
            var viewResult = _spaceController.Add(Id) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, string.Empty);

            var spaceObject = viewResult.Model as SpaceObject;
            Assert.AreEqual(spaceObject.ParentId, Id);
        }

        [Test]
        public void should_go_through_add_delete_process()
        {
            setupTestCondition();

            var jsonResult = _spaceController.Add() as JsonResult;
            var result = jsonResult.Data as SpaceViewModel;
            _spaceController.Delete(result.SpaceObject.ToString());
        }

        private void setupTestCondition()
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
