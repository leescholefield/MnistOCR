using System;

namespace Core.Math
{
    /// <summary>
    /// Calculates the Local Binary Pattern (<see cref="https://en.wikipedia.org/wiki/Local_binary_patterns"/>) value for each pixel 
    /// in a greyscale image matrix.
    /// 
    /// Summary of LBP:
    /// LBP generates a binary value for each pixel in a matrix. For the target pixel, compare it to each of its 
    /// 8 neighbors. If the greyscale value of a neighbour is greater than or equal to the target value append a "1" to the binary 
    /// value; if it lesser append a "0". When you have compared all of the neighbors you'll have an 8-digit binary number. Convert 
    /// that to decimal and that is the value for that target pixel. If one of the target pixel's neighbors is outside of the 
    /// array it's value will be 0.
    /// </summary>
    public class LBPCalculator
    {

        /// <summary>
        /// Co-ordinates for the 8 neighbors of a center pixel (1,1).
        /// </summary>
        private static readonly Tuple<int, int>[] NeighborCoords3x3 = new Tuple<int, int>[8]
        {
            new Tuple<int, int>(0,0),
            new Tuple<int, int>(0,1),
            new Tuple<int, int>(0,2),
            new Tuple<int, int>(1,0),
            new Tuple<int, int>(1,2),
            new Tuple<int, int>(2,0),
            new Tuple<int, int>(2,1),
            new Tuple<int, int>(2,2),
        };

        /// <summary>
        /// Calculates the LBP for each position in <paramref name="image"/>. See the class documentation for a summary of the 
        /// LBP algorithm.
        /// </summary>
        public int[,] Calculate(int[,] image)
        {
            int numCols = image.GetLength(0);
            int numRows = image.GetLength(1);

            int[,] result = new int[numCols, numRows];
            for (int c = 0; c < numCols; c++)
            {
                for (int r = 0; r < numRows; r++)
                {
                    int[,] neighbors = image.GetSubMatrix(new Tuple<int, int>(c, r), 3, 3);
                    int centerPixelValue = neighbors[1, 1];
                    // get binary value for center pixel.
                    string binaryString = "";
                    foreach(var coord in NeighborCoords3x3)
                    {
                        binaryString += neighbors[coord.Item1, coord.Item2] >= centerPixelValue ? "1" : "0";
                    }
                    int binaryInt = Convert.ToByte(binaryString, 2);
                    result[c, r] = binaryInt;
                }
            }
            return result;
        }

    }
}
