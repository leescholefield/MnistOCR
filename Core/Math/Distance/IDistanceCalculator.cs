using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Math.Distance
{
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates the distance of each array in <paramref name="vals"/> from the target and returns those distances as a List.
        /// The index of a value in vals will be the index of its distance in the returned list.
        /// </summary>
        public Task<List<double>> Calculate(List<int[]> vals, int[] target);

    }
}
