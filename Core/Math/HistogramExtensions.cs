using System;
using System.Collections.Generic;

namespace Core.Math
{
    /// <summary>
    /// Provides extension methods to Multidimensional (a.k.a Matrix) and Jagged Arrays for generating and flattening Histograms.
    /// </summary>
    public static class HistogramExtensions
    {

        /// <summary>
        /// Generates a Histogram from a Matrix. 
        /// </summary>
        /// <param name="matrix">Matrix to generate the histogram from.</param>
        /// <param name="sectionWidth">Width of a single section. This must be perfectly divisible into the matrix width.</param>
        /// <param name="sectionHeight">Height of a single section. This must be perfectly divisible into the matrix height.</param>
        /// <returns>A Jagged array containing histograms of each section.</returns>
        public static int[][] CreateHistogram(this int[,] matrix, int sectionWidth = 7, int sectionHeight = 7)
        {
            int mHeight = matrix.GetLength(0);
            int mWidth = matrix.GetLength(1);

            if (mHeight % sectionHeight != 0 || mWidth % sectionWidth != 0)
                throw new ArgumentOutOfRangeException("Both sectionWidth and sectionHeight must be evenly divisible into the matrix Width and Height.");

            int numSectionsAcross = mWidth / sectionWidth;
            int numSectionsDown = mHeight / sectionHeight;

            int[][] completeHistogram = new int[numSectionsAcross * numSectionsDown][];
            int currentSection = 0;

            for (int h = 0; h < numSectionsDown; h++)
            {
                //where this sections height pos starts and ends
                int startHPos = h * sectionHeight;
                int endHPos = (h + 1) * sectionHeight;

                for (int w = 0; w < numSectionsAcross; w++)
                {
                    // where this sections width pos starts and ends
                    int startWPos = w * sectionWidth;
                    int endWPos = (w + 1) * sectionWidth;

                    int[] sectionHist = new int[256];

                    // start getting the actual histogram here
                    for (int hPos = startHPos; hPos < endHPos; hPos++)
                    {
                        for (int wPos = startWPos; wPos < endWPos; wPos++)
                        {
                            int pixelVal = matrix[hPos, wPos];
                            sectionHist[pixelVal] += 1;
                        }
                    }

                    completeHistogram[currentSection] = sectionHist;
                    currentSection += 1;
                }
            }

            return completeHistogram;
        }

        /// <summary>
        /// Flattens a Jagged Array. Put simply, this appends all the values contained in the inner arrays onto each other.
        /// </summary>
        public static int[] Flatten(this int[][] array)
        {
            int numSections = array.Length;

            int capacity = numSections * 256; // 256 possible values for greyscale
            List<int> combined = new List<int>(capacity);
            foreach (int[] section in array)
            {
                combined.AddRange(section);
            }

            return combined.ToArray();
        }
    }
}
