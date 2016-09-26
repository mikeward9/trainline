using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV.Integration
{
    [TestFixture]
    public class CSVWriterTests
    {
        private const string Filename = "C:\\filename.txt";

        private MockFileSystem _fileSystem;

        private CSVWriter _csvWriter;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Filename, new MockFileData("") }
            });

            _csvWriter = new CSVWriter(_fileSystem);
        }

        [Test]
        public void Should_write_columns_to_file()
        {
            // Arrange

            // Act
            _csvWriter.Open(Filename);
            _csvWriter.Write("column1", "column2");
            _csvWriter.Close();

            // Assert
            var result = _fileSystem.FileInfo.FromFileName(Filename).OpenText().ReadToEnd();
            Assert.That(result, Is.EqualTo("column1\tcolumn2\r\n"), "It should format and write the given values to the output file");
        }

        [Test]
        public void Should_write_multiple_lines_to_file()
        {
            // Arrange

            // Act
            _csvWriter.Open(Filename);
            _csvWriter.Write("column1a", "column2a");
            _csvWriter.Write("column1b", "column2b");
            _csvWriter.Close();

            // Assert
            var result = _fileSystem.FileInfo.FromFileName(Filename).OpenText().ReadToEnd();
            Assert.That(result, Is.EqualTo("column1a\tcolumn2a\r\ncolumn1b\tcolumn2b\r\n"), "It should format and write the given values to the output file");
        }
    }
}