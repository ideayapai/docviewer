using System;
using System.Linq;
using System.Web.Http;
using NUnit.Framework;
using Services.Ioc;
using Services.Spaces;
using WebAPI2.Controllers;

namespace WebAPI2UnitTest.Controllers
{
    [TestFixture]
    public class SpaceControllerTester
    {

        private readonly SpaceController controller = new SpaceController(ServiceActivator.Get<SpaceService>());

     
        [Test]
        public void should_get_space_with_null_id()
        {
            var contracts = controller.GetSpaces(null, null, null);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_space_with_empty_id()
        {
            var contracts = controller.GetSpaces(string.Empty, string.Empty, string.Empty);
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        public void should_get_space_with_error_id()
        {
            var contracts = controller.GetSpaces(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.AreEqual(contracts.Count(), 0);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_remove_space_with_null_id()
        {
            controller.Remove(null);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_remove_space_with_empty_id()
        {
            controller.Remove(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_remove_space_with_error_id()
        {
             controller.Remove(Guid.NewGuid().ToString());
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_with_id_null_when_call_getspace()
        {
             controller.Get(null);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_with_id_empty_when_call_getspace()
        {
            controller.Get(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public void should_get_HttpResponseException_with_id_error_when_call_getspace()
        {
             controller.Get(Guid.NewGuid().ToString());
        }

    }
}
