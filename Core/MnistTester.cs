using Core.Dataset;
using Core.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Goes through the 'Dataset/mnist_test.csv' data.
    /// </summary>
    public class MnistTester
    {

        private readonly LBPCalculator LBPGenerator = new LBPCalculator();

        private readonly Configuration TestConfiguration;

        public MnistTester(Configuration testConfiguration)
        {
            TestConfiguration = testConfiguration;
        }

        /// <summary>
        /// Creates a MnistTester instance using a default <see cref="Configuration"/>.
        /// </summary>
        public MnistTester()
        {
            TestConfiguration = new Configuration() { KNumber = 3, TrainedSamples = 1000 };
        }

        public async Task<TestResults> Test(string testDataset, string trainedDir)
        {
            int correctGuesses = 0;
            int incorrectGuesses = 0;
            Dictionary<int, int> incorrectGuessesForLabel = new Dictionary<int, int>();
            
            // time it
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();

            List<TrainedData> trainedData =  await LoadTrainedData(trainedDir);

            IEnumerable<int[]> reader = new CsvReader(testDataset);
            foreach(int[] array in reader)
            {
                var (Label, Matrix) = array.CreateMatrixAndExtractLabel();
                var lpb = LBPGenerator.Calculate(Matrix);

                int[][] sectionedHistogram = lpb.CreateHistogram(7,7);
                var flattenedHistogram = sectionedHistogram.Flatten();

                // compare it to each labeledSamples
                Dictionary<int, List<double>> labeledDistances = await CalculateDistanceForEach(trainedData, flattenedHistogram);

                // get k nearest neighbors
                int guessedLabel = CalculateLabel(labeledDistances);
                if (guessedLabel == Label)
                {
                    correctGuesses++;
                }
                else
                {
                    if (incorrectGuessesForLabel.TryGetValue(Label, out int g))
                    {
                        incorrectGuessesForLabel[Label] = g + 1;
                    }
                    else
                    {
                        incorrectGuessesForLabel.Add(Label, 1);
                    }
                    incorrectGuesses++;
                }
            }

            stopWatch.Stop();

            return new TestResults
            {
                Duration = stopWatch.Elapsed,
                NumIncorrectGuesses = incorrectGuesses,
                NumCorrectGuesses = correctGuesses,
                IncorrectGuessesForLabel = incorrectGuessesForLabel
            };
        }

        private int CalculateLabel(Dictionary<int, List<double>> distances)
        {
            int kTarget = TestConfiguration.KNumber;

            Dictionary<int, int> labelCounts = new Dictionary<int, int>();
            for (int i = 0; i < kTarget; i++)
            {
                double candidateDistance = double.MaxValue;
                int candidateLabel = -1;
                foreach (var v in distances)
                {
                    // looking for lowest distance
                    double c = v.Value.First();
                    if (c < candidateDistance)
                    {
                        candidateDistance = c;
                        candidateLabel = v.Key;
                    }
                }

                if (labelCounts.TryGetValue(candidateLabel, out int val))
                {
                    labelCounts[candidateLabel] += 1;
                }
                else
                {
                    labelCounts.Add(candidateLabel, 1);
                }

                // remove first val in found label distances
                // possible performance gain by just keeping track of latest index
                distances[candidateLabel].RemoveAt(0);
                // reset candidates
                candidateDistance = -1;
                candidateLabel = -1;
            }

            // find label with highest value
            int label = labelCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            return label;
        }

        private async Task<Dictionary<int, List<double>>> CalculateDistanceForEach(List<TrainedData> trainedData, int[] target)
        {
            Dictionary<int, List<double>> result = new Dictionary<int, List<double>>();

            List<Task<(int, List<double>)>> tasks = new List<Task<(int, List<double>)>>();
            foreach (var data in trainedData)
            {
                tasks.Add(data.CalculateDistances(target));
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                result.Add(task.Result.Item1, task.Result.Item2);
            }

            return result;
        }

        private async Task<List<TrainedData>> LoadTrainedData(string dir)
        {
            List<TrainedData> data = new List<TrainedData>();

            // create Task for each file
            List<Task<TrainedData>> tasks = new List<Task<TrainedData>>(10);
            for (int i = 0; i < 10; i++)
            {
                string fileName = String.Format("{0}/model_{1}.csv", dir, i);
                tasks.Add(TrainedData.LoadData(fileName, i, TestConfiguration.TrainedSamples));
            }

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();
        }

        public class Configuration
        {
            public int TrainedSamples { get; set; }

            public int KNumber { get; set; }
        }
    }

    public class TestResults
    {
        public TimeSpan Duration { get; set; }

        public int NumCorrectGuesses { get; set; }

        public int NumIncorrectGuesses { get; set; }

        public Dictionary<int, int> IncorrectGuessesForLabel { get; set; }
    }
}
