using BackMeUp.Utils;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Utils
{
    [TestFixture]
    public class Crc16Tests
    {
        [Test]
        public void ComputeChecksum_ByteArray_Checksum()
        {
            var bytes = new byte[] {19, 128, 255};
            Crc16 crc = new Crc16();

            var checkSum = crc.ComputeChecksum(bytes);

            Assert.AreEqual(17872, checkSum);
        }

        [Test]
        public void ComputeChecksumBytes_ByteArray_Checksum()
        {
            var bytes = new byte[] { 23, 42, 02, 76, 223 };
            Crc16 crc = new Crc16();

            var checkSum = crc.ComputeChecksumBytes(bytes);

            Assert.AreEqual(new byte[]{168, 131}, checkSum);
        }


        //ushort ComputeChecksum(byte[] bytes);
        //byte[] ComputeChecksumBytes(byte[] bytes);
    }
}
