using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Dataset
{
    /// <summary>
    /// Reads a CSV file containing a comma-seperated list of ints and converts them to an array
    /// </summary>
    public class CsvReader : IEnumerable<int[]>
    {

        private readonly string _filePath;

        public CsvReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<int[]> GetEnumerator()
        {
            return new CsvEnumerator(_filePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CsvEnumerator : IEnumerator<int[]>
    {

        private readonly StreamReader _reader;

        public CsvEnumerator(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        #region Current

        private int[] _current;
        public int[] Current
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

        public bool MoveNext()
        {
            string line = _reader.ReadLine();
            if (line == null)
                return false;

            _current = ReadLine(line);
            return true;
        }

        private int[] ReadLine(string line)
        {
            var data = line.Split(",").Select(int.Parse).ToArray();
            return data;
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

                // un managed state
                _current = null;
                if (_reader != null)
                {
                    _reader.Close();
                    _reader.Dispose();
                }

                disposedValue = true;
            }
        }

        ~CsvEnumerator()
        {
           Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
