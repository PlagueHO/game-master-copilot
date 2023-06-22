using Microsoft.VisualStudio.TestTools.UnitTesting;
using dungeon_master_copilot_server.Data.Character;

namespace dungeon_master_copilot_server_test.Data.Character
{
    [TestClass]
    public class WeightTests
    {
        [TestMethod]
        public void GetOuncesTest()
        {
            var weight = new Weight { Value = 81600 };
            var expectedOunces = 2878.355136;

            Assert.AreEqual(expectedOunces, weight.GetOunces(), 0.0001);
        }

        [TestMethod]
        public void GetPoundsTest()
        {
            var weight = new Weight { Value = 81600 };
            var expectedPounds = 179.896992;

            Assert.AreEqual(expectedPounds, weight.GetPounds(), 0.0001);
        }

        [TestMethod]
        public void GetKilogramsTest()
        {
            var weight = new Weight { Value = 81600 };
            var expectedKilograms = 81.6;

            Assert.AreEqual(expectedKilograms, weight.GetKilograms(), 0.0001);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var weight = new Weight { Value = 81600 };
            var expectedString = "81600g";

            Assert.AreEqual(expectedString, weight.ToString());
        }
    }
}