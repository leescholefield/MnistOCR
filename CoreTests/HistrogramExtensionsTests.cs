﻿using Core.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass()]
    public class HistrogramExtensionsTests
    {
        [TestMethod()]
        public void WithSections_Has_Correct_NumberOfSections()
        {
            int[,] testMatrix = new int[28, 28];
            int expectedSections = 16;

            int[][] hist = testMatrix.CreateHistogram(7, 7);
            Assert.AreEqual(expectedSections, hist.Length);
        }

        [TestMethod()]
        public void WithSections_Section_Has_Expected_Values()
        {
            int[,] matrix = new int[28, 28];
            matrix[0, 0] = 12;
            matrix[0, 1] = 12;
            matrix[0, 2] = 12;

            var expectedResult = 3;

            int[][] hist = matrix.CreateHistogram(7, 7);

            Assert.AreEqual(expectedResult, hist[0][12]);
        }

        [TestMethod()]
        public void Has_Expected_Number_Of_Pixels_In_Each_Section()
        {
            int[,] matrix = new int[28, 28];

            var hist = matrix.CreateHistogram(7, 7);
            var expectedResult = 49; // 7 * 7

            // since it counts each sections pixels and each pixel is set to 0 this should be be a short hand to 
            // get total pixel count
            Assert.AreEqual(expectedResult, hist[0][0]);
        }

        [TestMethod()]
        public void Sections_Have_Expected_Combined_Length()
        {
            int[,] matrix = new int[28, 28];
            var hist = matrix.CreateHistogram(7, 7);

            int expected = 16 * 256;

            int actual = 0;
            foreach (var i in hist)
            {
                actual += i.Length;
            }

            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void CombineSections_Produces_Array_Of_Expected_Size()
        {
            int[,] matrix = new int[28, 28];
            var hist = matrix.CreateHistogram(7, 7);
            var result = hist.Flatten();
            
            Assert.AreEqual(16 * 256, result.Length);
        }
    }
}