using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Core.Dataset
{
    /// <summary>
    /// Reads MNIST data files and converts them to instances of <see cref="MnistDataSet"/>.
    /// </summary>
    public class MnistReader : IEnumerable<MnistDataSet>
    {

        private readonly string _filePath;

        public MnistReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<MnistDataSet> GetEnumerator()
        {
            return new MnistEnumerator(_filePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MnistEnumerator : IEnumerator<MnistDataSet>
    {

        private readonly StreamReader _reader;

        #region Current

        private MnistDataSet _current;

        public MnistDataSet Current
        {
            get
            {
                if (_reader == null || _current == null)
                    throw new InvalidOperationException();

                return _current;
            }
        }

        private object Current1
        {
            get
            {
                return Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current1;
            }
        }

        #endregion

        public MnistEnumerator(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        public bool MoveNext()
        {
            string line = _reader.ReadLine();
            if (line == null)
                return false;

            _current = ReadLine(line);
            return true;
        }

        private MnistDataSet ReadLine(string line)
        {
            string[] data = line.Split(",");
            int label = int.Parse(data[0]);
            int[,] vals = new int[28, 28];

            // turn 1d array into a matrix
            for (int h = 0; h < 28; h++)
            {
                for (int w = 0; w < 28; w++)
                {
                    // calculate the position in the 1d array from current height and width
                    // +1 at the end to account for the fact first value in the csv file is the label
                    int pos = (h * 28 + w) + 1;
                    int val = int.Parse(data[pos]);
                    vals[h, w] = val;
                }
            }

           return new MnistDataSet { Label = label, Values = vals };

        }

        public void Reset()
        {
            _reader.DiscardBufferedData();
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            _current = null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // no managed state to dispose
                }
                _current = null;
                if (_reader != null)
                {
                    _reader.Close();
                    _reader.Dispose();
                }

                disposedValue = true;
            }
        }

         ~MnistEnumerator()
         {
           Dispose(false);
         }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
