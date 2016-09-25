using System;
using System.IO.Abstractions;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    [Obsolete("Obsolete - use CSVReader and CSVWriter instead")]
    public class CSVReaderWriter
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICSVWriter _csvWriter;
        private readonly ICSVReader _csvReader;

        public CSVReaderWriter() : this(new FileSystem(), new CSVWriter(), new CSVReader())
        {
        }

        public CSVReaderWriter(IFileSystem fileSystem, ICSVWriter csvWriter, ICSVReader csvReader)
        {
            _csvWriter = csvWriter;
            _csvReader = csvReader;
            _fileSystem = fileSystem;
        }

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    _csvReader.Open(_fileSystem, fileName);
                    break;
                case Mode.Write:
                    _csvWriter.Open(_fileSystem, fileName);
                    break;
                default:
                    throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            _csvWriter.Write(columns);
        }

        [Obsolete("Obsolete - method achieves nothing, see notes")]
        public bool Read(string column1, string column2)
        {
            return true;
        }

        public bool Read(out string column1, out string column2)
        {
            return _csvReader.Read(out column1, out column2);
        }

        public void Close()
        {
            _csvWriter.Close();
            _csvReader.Close();
        }
    }
}
