using Core.Dataset;
using Core.Math.Distance;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{

    /// <summary>
    /// TODO: Instead of creating a new thread for every call of CalculateDistance we should have a queue where the main thread can 
    /// add histograms. The CalculateDistance method will then take from the queue when items are available
    /// </summary>
    public class TrainedData
    {

        private IDistanceCalculator DistanceCalculator = new EuclideanDistanceCalculator();

        public int Label { get; private set; }

        public List<int[]> Histograms { get; private set; }

        public static Task<TrainedData> LoadData(string fileName, int label, int sampleSize = 1000)
        {
            return Task.Run(() => 
            {
                List<int[]> selectedHistograms = new List<int[]>(sampleSize);
                IEnumerable<int[]> reader = new CsvReader(fileName);
                foreach (int[] histo in reader.Take(sampleSize))
                {
                    selectedHistograms.Add(histo);
                }

                return new TrainedData
                {
                    Histograms = selectedHistograms,
                    Label = label
                };
            });
        }

        public Task<(int, List<double>)> CalculateDistances(int[] target)
        {
            return Task.Run(() =>
            {
                var result = DistanceCalculator.Calculate(Histograms, target);
                result.Wait();

                List<double> distances = result.Result;
                // sort the distances in ascending order
                distances.Sort();
                return (Label, distances);
            });
        }
    }
}
