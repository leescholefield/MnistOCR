using System;
using System.Collections.Generic;

namespace Core.Math
{

    /// <summary>
    /// </summary>
    public class SectionedHistogram
    {

        public int[][] Sections { get; protected set; }

        public int NumberOfSections { get; protected set; }

        /// <summary>
        /// Generates a Histogram of the values in <paramref name="matrix"/>. Instead of considering all the values together this 
        /// will instead seperate them into sections, of the given width and height, and then generate a histogram of each section.
        /// 
        /// The histogram for each section will be of length 256 (each greyscale pixel can have a value of 0-255), and the returned 
        /// histogram will be all the concatenated sections. Thus, it will have a length of 255 * the number of sections.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="sectionHeight">Height of each section. Must evenly divide into matrix height.</param>
        /// <param name="sectionWidth">Width of each section. Must evenly divide into matrix width.</param>
        /// <returns>A Histrogram instance containing the concatenated histograms of each section.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If either sectionHeight or sectionWidth is not perfectly divisible into 
        /// matrix Height or Length respectivly.</exception>
        public static SectionedHistogram WithSections(int[,] matrix, int sectionWidth, int sectionHeight)
        {
            int mHeight = matrix.GetLength(0);
            int mWidth = matrix.GetLength(1);

            if (mHeight % sectionHeight  != 0 || mWidth % sectionWidth != 0)
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

            return new SectionedHistogram { Sections = completeHistogram, NumberOfSections = numSectionsAcross * numSectionsDown };
        }

        public int[] CombineSections()
        {
            int capacity = NumberOfSections * 256; // 256 possible values for greyscale
            List<int> combined = new List<int>(capacity);
            foreach(var section in Sections)
            {
                combined.AddRange(section);
            }

            return combined.ToArray();
        }
    }
}
