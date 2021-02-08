using BackMeUp.Utils;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Utils
{
    [TestFixture]
    public class DirectoryNameFixerTests
    {
        public static DirectoryNameFixer MakeDirectoryNameFixer()
        {
            return new DirectoryNameFixer();
        }

        [Test]
        public void RemoveInvalidCharacters_GetsGoodPath_ReturnsGoodPath()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.RemoveInvalidCharacters(@"What");

            StringAssert.AreEqualIgnoringCase(@"What", result);
        }

        [Test]
        public void RemoveInvalidCharacters_GetsDirectoryWithChar31_ReturnsChar31Removed()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.RemoveInvalidCharacters(string.Format(@"What{0}No!",(char)31));

            StringAssert.AreEqualIgnoringCase(@"WhatNo!", result);
        }

        [Test]
        public void RemoveInvalidCharacters_GetsDirectoryWithQuestionMark_ReturnsMultipleInvalidCharactersRemoved()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.RemoveInvalidCharacters(@"What?\/:No!");

            StringAssert.AreEqualIgnoringCase(@"WhatNo!", result);
        }

        [Test]
        public void RemoveInvalidCharacters_GetsNullDirectory_ReturnsNull()
        {
            var directoryNameFixer = MakeDirectoryNameFixer();

            var result = directoryNameFixer.RemoveInvalidCharacters(null);

            StringAssert.AreEqualIgnoringCase(null, result);
        }
    }
}
