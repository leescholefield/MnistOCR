using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Math.Distance
{
    /// <summary>
    /// An implementation of <see cref="IDistanceCalculator"/> using the Euclidean method.
    /// </summary>
    public class EuclideanDistanceCalculator : IDistanceCalculator
    {
        public Task<List<double>> Calculate(List<int[]> vals, int[] target)
        {
            return Task.Run(() =>
           {
               List<double> result = new List<double>();
               foreach (var t in vals)
               {
                   double distance = GetDistance(target, t);
                   result.Add(distance);
               }

               return result;
           });
        }
        private double GetDistance(int[] first, int[] second)
        {
            double distance = 0;
            for (int i = 0; i < first.Length; i++)
            {
                int diff = second[i] - first[i];
                distance += diff * diff;
            }
            distance = System.Math.Sqrt(distance);
            return distance;
        }
    }
}
