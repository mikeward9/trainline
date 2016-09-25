using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;
namespace AddressProcessing.Tests.CSV
{
    [TestFixture]
    public class CSVWriterTests
    {
        private string filename = "C:\\filename.txt";

        private MockFileSystem _fileSystem;
        private MockFileData _fileData;

        private CSVWriter _csvWriter;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _csvWriter = new CSVWriter();
        }

        [Test]
        public void Should_write_columns_to_file()
        {
            // Arrange
            _fileData = new MockFileData("");
            _fileSystem.AddFile(filename, _fileData);

            // Act (grr - see notes)
            _csvWriter.Open(_fileSystem, filename);
            _csvWriter.Write("column1", "column2");
            _csvWriter.Close();

            // Assert
            var result = _fileSystem.FileInfo.FromFileName(filename).OpenText().ReadToEnd();
            Assert.That(result, Is.EqualTo("column1\tcolumn2\r\n"), "It should format and write the given values to the output file");
        }
    }
}