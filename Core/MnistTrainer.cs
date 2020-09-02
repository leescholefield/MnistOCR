using Core.Dataset;
using Core.Math;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Core
{
    /// <summary>
    /// Trains the classifier using the Mnist training set. This will then generate a csv file for each label in the training set 
    /// that contains sectioned histograms for each example of that label.
    /// </summary>
    public partial class MnistTrainer
    {

        private readonly LBPCalculator LBPGenerator = new LBPCalculator();

        /// <summary>
        /// Trains the classifier using the dataset at <paramref name="datasetLocation"/>. This will generate csv files for each 
        /// label in the training set, containing a sectioned histogram for each example of that label in the training set.
        /// </summary>
        /// <param name="datasetLocation">Location of the file that contains the training data.</param>
        /// <param name="outputLocation">Location of the directory to save the output files to. These files will be a 
        /// CSV file containing the generated histograms for each label in the training data.</param>
        public void Train(string datasetLocation, string outputLocation)
        {
            var dataSet = LoadDataSet(datasetLocation);
            // key is the label, value is a histogram for that label.
            Dictionary<int, List<int[]>> labeledHistograms = new Dictionary<int, List<int[]>>();
            // add keys to dict
            for (int i = 0; i < 10; i++)
            {
                labeledHistograms[i] = new List<int[]>();
            }

            foreach (var val in dataSet)
            {
                int[,] lbp = LBPGenerator.Calculate(val.Values);
                val.Values = lbp;

                SectionedHistogram histo = SectionedHistogram.WithSections(val.Values, 7, 7);
                int[] combinedHisto = histo.CombineSections();
                labeledHistograms[val.Label].Add(combinedHisto);    
            }

            DumpHistogramsToFile(labeledHistograms, outputLocation);
        }

        private List<MnistDataSet> LoadDataSet(string loc)
        {
            List<MnistDataSet> result = new List<MnistDataSet>();
            MnistReader reader = new MnistReader(loc);
            foreach (MnistDataSet data in reader)
            {
                result.Add(data);
            }

            return result;
        }

        private void DumpHistogramsToFile(Dictionary<int, List<int[]>> histos, string dir)
        {
            // If dir already exists this will do nothing.
            Directory.CreateDirectory(dir);

            foreach (KeyValuePair<int, List<int[]>> pairs in histos)
            {
                string fileName = String.Format("{0}/model_{1}.csv", dir, pairs.Key);
                var stringHistograms = pairs.Value.Select(h => String.Join(",", h)); // gonna be slow

                using var stream = File.Open(fileName, FileMode.Create);
                using var writer = new StreamWriter(stream);
                foreach (var s in stringHistograms)
                {
                    writer.WriteLine(s);
                }
                writer.Flush();
            }
        }
    }
}
