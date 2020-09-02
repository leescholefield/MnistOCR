using Core.Dataset;
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

        public int Label { get; private set; }

        public List<int[]> Histograms { get; private set; }

        public static Task<TrainedData> LoadData(string fileName, int label, int sampleSize = 1000)
        {
            return Task.Run(() => 
            {
                List<int[]> selectedHistograms = new List<int[]>(sampleSize);
                TrainedReader reader = new TrainedReader(fileName);
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
                List<double> result = new List<double>();
                foreach (var t in Histograms)
                {
                    double distance = GetDistance(target, t);
                    result.Add(distance);
                }

                result.Sort();
                return (Label, result);
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
