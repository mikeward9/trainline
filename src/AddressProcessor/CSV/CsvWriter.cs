using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace AddressProcessing.CSV
{
    public interface ICSVWriter
    {
        void Open(string fileName);
        void Write(string[] columns);
        void Close();
    }

    public class CSVWriter : ICSVWriter
    {
        private readonly IFileSystem _fileSystem;
        protected TextWriter TextWriter;

        public CSVWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Open(string fileName)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(fileName);
            TextWriter = fileInfo.CreateText();
        }

        public void Write(params string[] columns)
        {
            var csv = columns.Aggregate((current, next) => current + '\t' + next);
            TextWriter.WriteLine(csv);
        }

        public void Close()
        {
            TextWriter?.Close();
        }
    }
}
