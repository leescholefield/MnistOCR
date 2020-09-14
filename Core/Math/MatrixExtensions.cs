using System;

namespace Core.Math
{
    /// <summary>
    /// Contains useful extension methods for a matrix (C# MultidimensionalArray type).
    /// </summary>
    public static class MatrixExtensions
    {

        /// <summary>
        /// Returns the value at matrix[col,row] or <paramref name="defaultVal"/> if it is out of range.
        /// </summary>
        public static int GetValueOrDefault(this int[,] matrix, int col, int row, int defaultVal = 0)
        {
            try
            {
                return matrix[col, row];
            }
            catch (IndexOutOfRangeException)
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// Returns a sub-matrix of the original matrix centered on the co-ordinates given in <paramref name="centerPos"/>. If the 
        /// centre pixel is located on the edge of the matrix the values outside of it will be set to 0.
        /// </summary>
        /// <example>
        /// Suppose you want a 3x3 sub-matrix centered on the square 5,4
        ///     <code>
        ///     var sub = matrix.GetSlice( (5,4), 3, 3);
        ///     </code>
        /// This will return a matrix containing the values at indexes 
        ///     (4,3)(4,4)(4,5)
        ///     (5,3)(5,4)(5,5)
        ///     (6,3)(6,4)(6,5)
        /// </example>
        /// <param name="centerPos">The column and row of the centre square.</param>
        /// <param name="height">How many squares high the slice should be. Must be an odd number greater than 1.</param>
        /// <param name="width">How many squares wide the slice should be. Must be an odd number greater than 1.</param>
        /// <returns></returns>
        public static int[,] GetSubMatrix(this int[,] matrix, Tuple<int, int> centerPos, int height = 3, int width = 3)
        {
            if (height < 3 || width < 3)
            {
                throw new Exception(String.Format("Invalid height and width {0,1}. These must be an odd number greater than 1",
                    height, width));
            }
            int[,] result = new int[height, width];

            int startCol = centerPos.Item1 - ((height - 1) / 2);
            int startRow = centerPos.Item2 - ((width - 1) / 2);

            for (int c = 0; c < height; c++)
            {
                int targetCol = startCol + c;
                for(int r = 0; r < width; r++)
                {
                    int targetRow = startRow + r;
                    result[c, r] = matrix.GetValueOrDefault(targetCol, targetRow);
                }
            }

            return result;
        }


    }
}
