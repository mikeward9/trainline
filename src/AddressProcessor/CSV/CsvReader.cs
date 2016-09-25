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
        private TextReader _textReader;

        public void Open(IFileSystem fileSystem, string fileName)
        {
            _textReader = fileSystem.File.OpenText(fileName);
        }

        public bool Read(out string column1, out string column2)
        {
            var line = _textReader.ReadLine();

            if (line == null || line.Length == 1)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            var columns = line.Split('\t');
            column1 = columns[0];
            column2 = columns[1];

            return true;
        }

        public void Close()
        {
            _textReader?.Close();
        }
    }
}
