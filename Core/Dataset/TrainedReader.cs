using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Dataset
{
    public class TrainedReader : IEnumerable<int[]>
    {

        private readonly string _filePath;

        public TrainedReader(string filePath)
        {
            _filePath = filePath;
        }


        public IEnumerator<int[]> GetEnumerator()
        {
            return new TrainedEnumerator(_filePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TrainedEnumerator : IEnumerator<int[]>
    {

        private readonly StreamReader _reader;

        public TrainedEnumerator(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        #region Current Imp

        private int[] _current;
        public int[] Current
        {
            get {
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
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TrainedEnumerator()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
