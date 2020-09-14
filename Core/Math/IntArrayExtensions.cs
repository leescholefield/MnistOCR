namespace Core.Math
{
    /// <summary>
    /// Contains extension methods for int arrays. This class is mainly used to convert the arrays optained via csv files into 
    /// more useful datastructures (Multidimensional arrays for instance).
    /// </summary>
    public static class IntArrayExtensions
    {
        /// <summary>
        /// Converts an int array into a Matrix or Multidimensional Array.
        /// </summary>
        /// <param name="array">Array to convert</param>
        /// <param name="height">Height of the matrix. The default is 28 since this is the height of the Mnist image.</param>
        /// <param name="width">Width of the matrix. The default is 28 since this is the width of the Mnist image.</param>
        /// <returns></returns>
        public static int[,] CreateMatrix(this int[] array, int height = 28, int width = 28)
        {
            int[,] vals = new int[height, width];

            // turn 1d array into a matrix
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    // calculate the position in the 1d array from current height and width
                    int pos = h * 28 + w;
                    int val = array[pos];
                    vals[h, w] = val;
                }
            }

            return vals;
        }

        /// <summary>
        /// In the Mnist csv files the first int in each line will be the label. This method is identical to <see cref="CreateMatrix"/>
        /// except that it will account for this label, returning it along with the image pixel matrix.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns>A tuple containing an int label (what digit the image represents) and the pixel values for that image in a matrix.</returns>
        public static (int Label, int[,] Matrix) CreateMatrixAndExtractLabel(this int[] array, int height = 28, int width = 28)
        {
            int label = array[0];
            int[,] vals = new int[height, width];

            // turn 1d array into a matrix
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    // calculate the position in the 1d array from current height and width
                    // +1 to account for label offset
                    int pos = (h * 28 + w) + 1;
                    int val = array[pos];
                    vals[h, w] = val;
                }
            }

            return (label, vals);
        }

    }
}
