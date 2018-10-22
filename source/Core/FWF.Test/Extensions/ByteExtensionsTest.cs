using NUnit.Framework;

namespace FWF.Test.Extensions
{
    [TestFixture]
    public class ByteExtensionsTest
    {


        [Test]
        public void HexFunctions()
        {
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            var dataString = data.ToHex();

            var result = dataString.FromHex();

            Assert.IsTrue(data.IsEqualByte(result));
        }



    }
}


