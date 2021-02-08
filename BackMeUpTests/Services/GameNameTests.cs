using BackMeUp.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace BackMeUp.UnitTests.Services
{
    [TestFixture]
    public class GameNameTests
    {
        [TestCase]
        public void Resolve_ExistingId_ReturnsName()
        {
            var expectedId = 132;
            var expectedName = "LASTAC";
            var games = new Dictionary<int, string> { { expectedId, expectedName } };
            var sut = new GameName(games);

            var name = sut.FromId(expectedId);

            Assert.AreEqual(expectedName, name);
        }

        [TestCase]
        public void Resolve_NotConfiguredId_ReturnsNameWithUnidentified()
        {
            var expectedId = 132;
            var expectedName = "132_Unidentified";
            var games = new Dictionary<int, string>();
            var sut = new GameName(games);

            var name = sut.FromId(expectedId);

            Assert.AreEqual(expectedName, name);
        }
    }
}