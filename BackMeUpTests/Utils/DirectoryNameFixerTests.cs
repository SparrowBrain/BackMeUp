using BackMeUp.Utils;
using NUnit.Framework;

namespace BackMeUpTests.Utils
{
    [TestFixture]
    public class DirectoryNameFixerTests
    {
        public static DirectoryNameFixer MakeDirectoryNameFixer()
        {
            return new DirectoryNameFixer();
        }

        [Test]
        public void ReplaceInvalidCharacters_GetsGoodPath_ReturnsGoodPath()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.ReplaceInvalidCharacters(@"What");

            StringAssert.AreEqualIgnoringCase(@"What", result);
        }

        [Test]
        public void ReplaceInvalidCharacters_GetsDirectoryWithChar31_ReturnsChar31ReplacedWithUnderscore()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.ReplaceInvalidCharacters(string.Format(@"What{0}No!",(char)31));

            StringAssert.AreEqualIgnoringCase(@"What_No!", result);
        }

        [Test]
        public void ReplaceInvalidCharacters_GetsDirectoryWithQuestionMark_ReturnsQuestionMarkReplacedWithUnderscore()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.ReplaceInvalidCharacters(@"What?\/:No!");

            StringAssert.AreEqualIgnoringCase(@"What____No!", result);
        }

        [Test]
        public void ReplaceInvalidCharacters_GetsNullDirectory_ReturnsNull()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.ReplaceInvalidCharacters(null);

            StringAssert.AreEqualIgnoringCase(null, result);
        }
    }
}
