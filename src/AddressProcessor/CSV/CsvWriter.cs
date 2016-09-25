using System.IO;
using System.IO.Abstractions;
using System.Linq;

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

        public void Open(IFileSystem fileSystem, string fileName)
        {
            var fileInfo = fileSystem.FileInfo.FromFileName(fileName);
            _textWriter = fileInfo.CreateText();
        }

        public void Write(params string[] columns)
        {
            var csv = columns.Aggregate((current, next) => current + '\t' + next);
            _textWriter.WriteLine(csv);
        }

        public void Close()
        {
            _textWriter?.Close();
        }
    }
}
