using System;
using System.IO;
using System.IO.Abstractions;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter
    {
        private readonly IFileSystem _fileSystem;

        private StreamReader _readerStream;
        private TextWriter _writerStream;

        public CSVReaderWriter() : this(new FileSystem())
        {
        }

        public CSVReaderWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    _readerStream = _fileSystem.File.OpenText(fileName);
                    break;
                case Mode.Write:
                    var fileInfo = _fileSystem.FileInfo.FromFileName(fileName);
                    _writerStream = fileInfo.CreateText();
                    break;
                default:
                    throw new Exception("Unknown file mode for " + fileName);
            }
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
          
            _writerStream.WriteLine(output);
        }

        public bool Read(string column1, string column2)
        {
            const int firstColumn = 0;
            const int secondColumn = 1;

            char[] separator = { '\t' };

            var line = ReadLine();
            var columns = line.Split(separator);

            // I don't think this can be reached so not testing https://dotnetfiddle.net/J5jl6c
            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[firstColumn];
                column2 = columns[secondColumn];

                return true;
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int firstColumn = 0;
            const int secondColumn = 1;

            char[] separator = { '\t' };

            var line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            var columns = line.Split(separator);

            // I don't think this can be reached so not testing https://dotnetfiddle.net/J5jl6c
            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[firstColumn];
                column2 = columns[secondColumn];

                return true;
            }
        }
        
        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            _writerStream?.Close();

            _readerStream?.Close();
        }
    }
}
