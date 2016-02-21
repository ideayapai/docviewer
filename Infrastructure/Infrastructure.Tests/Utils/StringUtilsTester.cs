using Infrasturcture.Utils;
using NUnit.Framework;

namespace Infrasturcture.Tests.Utils
{
    [TestFixture]
    public class StringUtilsTester
    {
        [Test]
        public void should_removeallempty()
        {
            string input = " yes, it is empty      now,with blank";
            string result = StringUtils.RemoveAllEmpty(input);
            string expect = "yes,itisemptynow,withblank";
            Assert.AreEqual(result, expect);
        }
    }
}
