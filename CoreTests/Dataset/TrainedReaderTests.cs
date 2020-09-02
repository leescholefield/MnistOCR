using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Dataset.Tests
{
    [TestClass()]
    [DeploymentItem("TestData/trained.csv")]
    public class TrainedReaderTests
    {

        private static TrainedReader Reader;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext _)
        {
            Reader = new TrainedReader("trained.csv");
        }

        [TestMethod()]
        public void Enumerator_Has_Correct_Number_Of_Items()
        {
            int count = 0;
            foreach (var i in Reader)
            {
                count++;
            }

            Assert.AreEqual(5, count);
        }

        [TestMethod()]
        public void MnistDataSet_Values_Has_Correct_Length()
        {
            foreach (var i in Reader)
            {
                Assert.AreEqual(4096, i.Length);
            }
        }
    }
}
