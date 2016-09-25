using System.IO;
using System.IO.Abstractions;

namespace AddressProcessing.CSV
{
    public interface ICSVWriter
    {
        void Open(IFileSystem fileSystem, string fileName);
        void Write(string[] columns);
        void Close();
    }

    public class CSVWriter : ICSVWriter
    {
        private TextWriter _textWriter;
        private IFileSystem _fileSystem;

        public void Open(IFileSystem fileSystem, string fileName)
        {
            _fileSystem = fileSystem;
            var fileInfo = _fileSystem.FileInfo.FromFileName(fileName);
            _textWriter = fileInfo.CreateText();
        }

        public void Write(params string[] columns)
        {
            var output = "";

            for (var i = 0; i < columns.Length; i++)
            {
                output += columns[i];
                if ((columns.Length - 1) != i)
                {
                    output += "\t";
                }
            }

            _textWriter.WriteLine(output);
        }

        public void Close()
        {
            _textWriter?.Close();
        }
    }
}
