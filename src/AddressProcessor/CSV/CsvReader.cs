using System.IO;
using System.IO.Abstractions;

namespace AddressProcessing.CSV
{
    public interface ICSVReader
    {
        void Open(IFileSystem fileSystem, string fileName);
        bool Read(out string column1, out string column2);
        void Close();
    }

    public class CSVReader : ICSVReader
    {
        private StreamReader _readerStream;

        public void Open(IFileSystem fileSystem, string fileName)
        {
            _readerStream = fileSystem.File.OpenText(fileName);
        }

        public bool Read(out string column1, out string column2)
        {
            const int firstColumn = 0;
            const int secondColumn = 1;

            char[] separator = { '\t' };

            var line = _readerStream.ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            var columns = line.Split(separator);
            column1 = columns[firstColumn];
            column2 = columns[secondColumn];

            return true;
        }

        public void Close()
        {
            _readerStream?.Close();
        }
    }
}
