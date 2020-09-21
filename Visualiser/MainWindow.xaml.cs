using Core.Dataset;
using Core.Math;
using Core.Math.Distance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Visualiser.Dataset;

namespace Visualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly string _datasetDir = "Dataset/Data";

        private readonly IDistanceCalculator DistanceCalculator = new EuclideanDistanceCalculator();
        private readonly LBPCalculator LBPCalculator = new LBPCalculator();

        /// <summary>
        /// Contains lists of Datasets by the label they represent.
        /// </summary>
        private Dictionary<int, List<DatasetModelExt>> DatasetDictionary;

        private IEnumerator<int[]> TestDatasetEnumerator;


        public MainWindow()
        {
            InitializeComponent();

            DatasetDictionary = new Dictionary<int, List<DatasetModelExt>>(10);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        /// <summary>
        /// Disallows non-numeric input.
        /// </summary>
        private void num_textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void Test_Button_Click(object sender, RoutedEventArgs e)
        {
            // ensure KNN and sample size boxes have valid input
            if (!int.TryParse(num_neighbours.Text, out int numNeighbours))
                return;

            if (!int.TryParse(sample_size.Text, out int sampleSize))
                return;

            LoadDataset(sampleSize);

            next_button.Visibility = Visibility.Visible;
            start_test_button.Content = "Restart";

            NextTestLabel();
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            // ensure dataset has been loaded
            if (DatasetDictionary.Count == 0 || TestDatasetEnumerator == null)
                return;

            NextTestLabel();

        }

        /// <summary>
        /// Goes through all combined_model files and adds the datasets to <see cref="DatasetDictionary"/>.
        /// </summary>
        /// <param name="sampleSize">How many datasets to return from each label file.</param>
        private void LoadDataset(int sampleSize)
        {
            for(int i = 0; i < 10; i++)
            {
                string fileName = String.Format("{0}/combined_model_{1}.csv", _datasetDir, i);
                DatasetReader reader = new DatasetReader(fileName);

                // assumes doesn't already have a value.
                DatasetDictionary.Add(i, new List<DatasetModelExt>(sampleSize));

                foreach (var result in reader.Take(sampleSize))
                {
                    DatasetDictionary[i].Add(DatasetModelExt.CopyFromDatasetModel(result));
                }
            }

            TestDatasetEnumerator = new CsvReader("Dataset/Mnist/mnist_test.csv").GetEnumerator();
        }

        private void NextTestLabel()
        {
            if (!TestDatasetEnumerator.MoveNext())
            {
                return;
            }

            int[] data = TestDatasetEnumerator.Current;
            NextTestLabel(data, int.Parse(num_neighbours.Text));
        }

        /// <summary>
        /// Advances to the next label in the test set. 
        /// 
        /// This has several side effects throughout its execution. Firstly, it will set the <see cref="current_image"/> to the pixel 
        /// matrix extracted from labelData. Secondly, the nearest K values will be added to <see cref="closest_matches_container"/>. 
        /// </summary>
        /// <param name="labelData">The raw data for this label. This includes the label number in index position 0.</param>
        private void NextTestLabel(int[] labelData, int numNeighbors)
        {
            var (label, matrix) = labelData.CreateMatrixAndExtractLabel();

            // set image_label to current labelData
            Bitmap image = ConvertMatrixIntoImage(matrix);
            current_image.Source = ConvertBitmap(image);
            image_label.Content = label;

            var lbp = LBPCalculator.Calculate(matrix);
            int[] currentHisto = lbp.CreateHistogram().Flatten();

            var distances = GetDistancesForEachLabel(currentHisto);
            distances.Wait();

            AssociateDistancesWithDataset(distances.Result);
            var nearest = GetNearestK(numNeighbors);

            // set closest_matches_container children to nearest
            closest_matches_container.Children.Clear();
            foreach (var n in nearest)
            {
                foreach (var d in n.Value)
                {
                    int[,] imageMatrix = d.PixelMatrix;
                    Bitmap cBitmap = ConvertMatrixIntoImage(imageMatrix);
                    System.Windows.Controls.Image cImage = new System.Windows.Controls.Image
                    {
                        Source = ConvertBitmap(cBitmap)
                    };
                    closest_matches_container.Children.Add(cImage);
                }
            }

            int guess = GuessLabel(nearest);
            // set correct/incorrect label
            if (guess == label)
            {
                result_correct.Visibility = Visibility.Visible;
                result_incorrect.Visibility = Visibility.Hidden;
            }
            else
            {
                result_incorrect.Visibility = Visibility.Visible;
                result_correct.Visibility = Visibility.Hidden;
            }
        }

        #region Bitmap Conversions

        private Bitmap ConvertMatrixIntoImage(int[,] matrix)
        {
            Bitmap image = new Bitmap(28, 28);

            for (int col = 0; col < 28; col++)
            {
                for(int row = 0; row < 28; row++)
                {
                    int val = matrix[col, row];
                    image.SetPixel(row, col, System.Drawing.Color.FromArgb(0, val, val, val));
                }
            }

            return image;
        }

        public BitmapImage ConvertBitmap(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        #endregion

        #region Distance Calculation

        private Task<Dictionary<int, List<double>>> GetDistancesForEachLabel(int[] target)
        {
            return Task.Run(() =>
           {
               Dictionary<int, List<double>> result = new Dictionary<int, List<double>>(10);
               Dictionary<int, Task<List<double>>> taskDict = new Dictionary<int, Task<List<double>>>(10);
               // calculate distance for each task
               foreach(var i in DatasetDictionary)
               {
                   var histos = i.Value.Select(s => s.Histogram).ToList();
                   var task = DistanceCalculator.Calculate(histos, target);
                   taskDict.Add(i.Key, task);
               }

               Task.WaitAll(taskDict.Values.ToArray());

               foreach(var task in taskDict)
               {
                   result.Add(task.Key, task.Value.Result);
               }

               return result;
           });
        }



        /// <summary>
        /// This copies the values in <paramref name="list"/> into a dictionary by their index and then sorts that dictionary in 
        /// ascending order by the value.
        /// </summary>
        /// <remarks>
        /// Although this is terribly ineffecient I haven't been able to come up with a more effective solution to the problem: how 
        /// can we get the original pixel matrix from the list of distances we are given by the IDistanceCaluclator? One solution is 
        /// to wrap 
        /// </remarks>
        private Dictionary<int, double> CopyListIndexAndSortByValue(List<double> list)
        {
            Dictionary<int, double> result = new Dictionary<int, double>(list.Count);

            for(int i = 0; i < list.Count; i++)
            {
                result.Add(i, list[i]);
            }

            return result.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<int, List<DatasetModelExt>> GetNearestK(int k)
        {
            Dictionary<int, List<DatasetModelExt>> result = new Dictionary<int, List<DatasetModelExt>>();
            Dictionary<int, List<DatasetModelExt>> copy = new Dictionary<int, List<DatasetModelExt>>(DatasetDictionary);
            
            // sort each list
            foreach(var l in copy.Values)
            {
                l.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            }

            for(int i = 0; i < k; i++)
            {
                DatasetModelExt candidate = new DatasetModelExt { Distance = double.MaxValue };
                int candidateLabel = -1;

                foreach(KeyValuePair<int, List<DatasetModelExt>> pair in copy)
                {
                    var current = pair.Value.First();

                    if (current.Distance < candidate.Distance)
                    {
                        candidate = current;
                        candidateLabel = pair.Key;
                    }
                }

                if (result.ContainsKey(candidateLabel))
                    result[candidateLabel].Add(candidate);
                else
                    result.Add(candidateLabel, new List<DatasetModelExt>{candidate});
                copy[candidateLabel].RemoveAt(0);
            }

            return result;
        }

        private List<(int Label, int IndexPreSort)> GetNearestK(Dictionary<int, Dictionary<int, double>> dict, int k)
        {
            List<(int Label, int IndexPreSort)> result = new List<(int Label, int IndexPreSort)>(k);

            for(int i = 0; i < k; i++)
            {
                KeyValuePair<int, double> candidate = new KeyValuePair<int, double>(-1, double.MaxValue);
                int candidateLabel = -1;

                foreach(var v in dict)
                {
                    var current = v.Value.First();

                    if (current.Value < candidate.Value)
                    {
                        candidate = current;
                        candidateLabel = v.Key;
                    }
                }

                // add top result
                result.Add((candidateLabel, candidate.Key));

                // remove first val
                dict[candidateLabel].Remove(0);
            }

            return result;
        }

        #endregion

        private int GuessLabel(Dictionary<int, List<DatasetModelExt>> nearest)
        {
            int label = -1;
            int currentTopGuesses = -1;

            foreach(var i in nearest)
            {
                if (i.Value.Count > currentTopGuesses)
                {
                    label = i.Key;
                    currentTopGuesses = i.Value.Count;
                }
            }

            return label;
        }

        private void AssociateDistancesWithDataset(Dictionary<int, List<double>> distances)
        {
            for(int i = 0; i < 10; i++)
            {
                var currentDs = DatasetDictionary[i];
                var currentDistances = distances[i];
                for(int r = 0; r < currentDs.Count; r++)
                {
                    currentDs[r].Distance = currentDistances[r];
                }
            }
        }
    }
}
