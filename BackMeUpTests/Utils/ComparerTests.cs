using System;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Utils
{
    [TestFixture]
    public class ComparerTests
    {
        [Test]
        public void CompareDirectories_IdenticalDirectories_True()
        {
            ICrc16 crc16 = Substitute.For<ICrc16>();
            IDirectory directory = Substitute.For<IDirectory>();
            IFile file = Substitute.For<IFile>();
            directory.GetFiles(@"C:\saveDir").Returns(new[] {@"C:\saveDir\a.file", @"C:\saveDir\b.file"});
            directory.GetFiles(@"C:\backupDir").Returns(new[] {@"C:\backupDir\a.file", @"C:\saveDir\b.file"});
            file.ReadAllBytes((Arg.Is<string>(x => x.EndsWith("a.file")))).Returns(new byte[] {12});
            file.ReadAllBytes((Arg.Is<string>(x => x.EndsWith("b.file")))).Returns(new byte[] {13});
            crc16.ComputeChecksum(new byte[] {12}).Returns((ushort) 52);
            crc16.ComputeChecksum(new byte[] {13}).Returns((ushort) 53);
            var comparer = new Comparer(crc16, directory, file);

            var directoriesMatch = comparer.CompareDirectories(@"C:\saveDir", @"C:\backupDir");

            Assert.AreEqual(true, directoriesMatch);
        }

        [Test]
        public void CompareDirectories_NonMatchingFileCount_False()
        {
            ICrc16 crc16 = Substitute.For<ICrc16>();
            IDirectory directory = Substitute.For<IDirectory>();
            IFile file = Substitute.For<IFile>();
            directory.GetFiles(@"C:\saveDir").Returns(new[] {@"C:\saveDir\a.file", @"C:\saveDir\b.file"});
            directory.GetFiles(@"C:\backupDir").Returns(new[] {@"C:\backupDir\a.file"});
            var comparer = new Comparer(crc16, directory, file);

            var directoriesMatch = comparer.CompareDirectories(@"C:\saveDir", @"C:\backupDir");

            Assert.AreEqual(false, directoriesMatch);
        }

        [Test]
        public void CompareDirectories_FileNameDiffers_False()
        {
            ICrc16 crc16 = Substitute.For<ICrc16>();
            IDirectory directory = Substitute.For<IDirectory>();
            IFile file = Substitute.For<IFile>();
            directory.GetFiles(@"C:\saveDir").Returns(new[] { @"C:\saveDir\a.file" });
            directory.GetFiles(@"C:\backupDir").Returns(new[] { @"C:\backupDir\b.file" });
            crc16.ComputeChecksum(Arg.Any<byte[]>()).Returns((ushort) 52);
            var comparer = new Comparer(crc16, directory, file);

            var directoriesMatch = comparer.CompareDirectories(@"C:\saveDir", @"C:\backupDir");

            Assert.AreEqual(false, directoriesMatch);
        }

        [Test]
        public void CompareDirectories_FileCrcDiffers_False()
        {
            ICrc16 crc16 = Substitute.For<ICrc16>();
            IDirectory directory = Substitute.For<IDirectory>();
            IFile file = Substitute.For<IFile>();
            directory.GetFiles(@"C:\saveDir").Returns(new[] { @"C:\saveDir\a.file" });
            directory.GetFiles(@"C:\backupDir").Returns(new[] { @"C:\backupDir\a.file" });
            crc16.ComputeChecksum(Arg.Any<byte[]>()).Returns((ushort) 52, new ushort[] {66});
            var comparer = new Comparer(crc16, directory, file);

            var directoriesMatch = comparer.CompareDirectories(@"C:\saveDir", @"C:\backupDir");

            Assert.AreEqual(false, directoriesMatch);
        }

        [TestCase(null, @"C:\backupDir")]
        [TestCase("", @"C:\backupDir")]
        [TestCase(@"C:\saveDir", null)]
        [TestCase(@"C:\saveDir", "")]
        public void CompareDirectories_InvalidSourceOrDestinationDirectories_ThrowsArgumentException(string sourceDir, string destinationDir)
        {
            ICrc16 crc16 = Substitute.For<ICrc16>();
            IDirectory directory = Substitute.For<IDirectory>();
            IFile file = Substitute.For<IFile>();
            var comparer = new Comparer(crc16, directory, file);

            TestDelegate actDelegate = () => comparer.CompareDirectories(sourceDir, destinationDir);

            Assert.Throws<ArgumentException>(actDelegate);
        }
    }
}
