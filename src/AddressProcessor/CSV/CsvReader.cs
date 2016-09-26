using System.IO;
using System.IO.Abstractions;

namespace AddressProcessing.CSV
{
    public interface ICSVReader
    {
        void Open(string fileName);
        bool Read(out string column1, out string column2);
        void Close();
    }

    public class CSVReader : ICSVReader
    {
        private readonly IFileSystem _fileSystem;

        protected TextReader TextReader;

        public CSVReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Open(string fileName)
        {
            TextReader = _fileSystem.File.OpenText(fileName);
        }

        public bool Read(out string column1, out string column2)
        {
            var line = TextReader.ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            var columns = line.Split('\t');
            if (columns.Length == 1)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            column1 = columns[0];
            column2 = columns[1];

            return true;
        }

        public void Close()
        {
            TextReader?.Close();
        }
    }
}
