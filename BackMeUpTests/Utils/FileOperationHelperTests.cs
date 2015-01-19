using System;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Utils
{
    [TestFixture]
    public class FileOperationHelperTests
    {
        [Test]
        public void CreateDirectoryIfNotExists_DirectoryDoesntExist_DirectoryIsCreated()
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            directory.Exists(@"C:\Hugs").Returns(false);
            var fileOperationHelper = new FileOperationHelper(file, directory);

            fileOperationHelper.CreateDirectoryIfNotExists(@"C:\Hugs");

            directory.Received().CreateDirectory(@"C:\Hugs");
        }

        [Test]
        public void CreateDirectoryIfNotExists_DirectoryExists_NothingDone()
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            directory.Exists("").ReturnsForAnyArgs(true);
            var fileOperationHelper = new FileOperationHelper(file, directory);

            fileOperationHelper.CreateDirectoryIfNotExists(@"C:\Hugs");

            directory.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateDirectoryIfNotExists_PathIsNullOrEmpty_ThrowsArgumentException(string path)
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            var fileOperationHelper = new FileOperationHelper(file, directory);

            TestDelegate actDelegate = () => fileOperationHelper.CreateDirectoryIfNotExists(path);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [Test]
        public void CopyDirectory_TwoFilesInDirectory_CopiesFiles()
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            directory.GetFiles(@"C:\Hugs").Returns(new[] {@"C:\Hugs\one.file", @"C:\Hugs\two.file"});
            var fileOperationHelper = new FileOperationHelper(file, directory);

            fileOperationHelper.CopyDirectory(@"C:\Hugs", @"C:\Kisses");

            file.Received().Copy(@"C:\Hugs\one.file", @"C:\Kisses\one.file");
            file.Received().Copy(@"C:\Hugs\two.file", @"C:\Kisses\two.file");
        }

        [Test]
        public void CopyDirectory_FileInSubdirectory_CopiesFile()
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            directory.GetFiles(@"C:\Hugs").Returns(new[] {@"C:\Hugs\Here"});
            directory.GetFiles(@"C:\Hugs\Here").Returns(new[] {@"C:\Hugs\Here\one.file"});
            directory.Exists(@"C:\Hugs\Here").Returns(true);
            var fileOperationHelper = new FileOperationHelper(file, directory);

            fileOperationHelper.CopyDirectory(@"C:\Hugs", @"C:\Kisses");

            file.Received().Copy(@"C:\Hugs\Here\one.file", @"C:\Kisses\Here\one.file");
        }

        [TestCase(null)]
        [TestCase("")]
        public void CopyDirectory_SourcePathIsNullOrEmpty_ThrowsArgumentException(string sourcePath)
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            var fileOperationHelper = new FileOperationHelper(file, directory);

            TestDelegate actDelegate = () => fileOperationHelper.CopyDirectory(sourcePath, @"C:\Kisses");

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [TestCase(null)]
        [TestCase("")]
        public void CopyDirectory_DestinationPathIsNullOrEmpty_ThrowsArgumentException(string destinationPath)
        {
            IFile file = Substitute.For<IFile>();
            IDirectory directory = Substitute.For<IDirectory>();
            var fileOperationHelper = new FileOperationHelper(file, directory);

            TestDelegate actDelegate = () => fileOperationHelper.CopyDirectory(@"C:\Hugs", destinationPath);

            Assert.Throws<ArgumentException>(actDelegate);
        }
    }
}
