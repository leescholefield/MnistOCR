using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Dataset.Tests
{
    [TestClass()]
    [DeploymentItem("TestData/mnist.csv")]
    public class MnistReaderTests
    {
        private static MnistReader Reader;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext _)
        {
            Reader = new MnistReader("mnist.csv");
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
        public void MnistDataSet_Has_Correct_Label()
        {
            int[] expectedLabels = new int[] { 7, 2, 1, 0, 4 };
            int counter = 0;
            foreach (var i in Reader)
            {
                Assert.AreEqual(expectedLabels[counter], i.Label);
                counter++;
            }
        }

        [TestMethod()]
        public void MnistDataSet_Values_Has_Correct_Width_And_Length()
        {
            foreach (var i in Reader)
            {
                int col = i.Values.GetLength(0);
                int row = i.Values.GetLength(1);

                Assert.AreEqual(28, col);
                Assert.AreEqual(28, row);
            }
        }
    }
}