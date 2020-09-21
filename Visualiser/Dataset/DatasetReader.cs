using Core.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Visualiser.Dataset
{
    public class DatasetReader : IEnumerable<DatasetModel>
    {

        private readonly string _filePath;

        public DatasetReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<DatasetModel> GetEnumerator()
        {
            return new DatasetEnumerator(_filePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DatasetEnumerator : IEnumerator<DatasetModel>
    {

        private readonly StreamReader _reader;
        public DatasetEnumerator(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        #region Current Imp

        private DatasetModel _current;
        public DatasetModel Current
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
                return _current;
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

        public bool MoveNext()
        {

            string line = _reader.ReadLine();
            if (line == null)
                return false;

            int[] data = line.Split(",").Select(int.Parse).ToArray();
            int[,] imageMatrix = data.CreateMatrix();

            line = _reader.ReadLine();
            // if our combine_script is correct this should never happen
            if (line == null)
            {
                throw new Exception("Pixel image does not have associated histogram");
            }

            int[] histogram = line.Split(",").Select(int.Parse).ToArray();

            _current = new DatasetModel
            {
                PixelMatrix = imageMatrix,
                Histogram = histogram
            };
            return true;
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
                    // no managed state
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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DatasetEnumerator()
        {
          // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
          Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
