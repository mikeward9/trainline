using System;
using System.IO.Abstractions.TestingHelpers;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV
{
    [TestFixture]
    public class CSVReaderWriterIntegrationTests
    {
        private string filename = "C:\\filename.txt";

        private MockFileSystem _fileSystem;
        private MockFileData _fileData;

        private CSVReaderWriter _csvReaderWriter;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            var csvWriter = new CSVWriter();
            var csvReader = new CSVReader();
            _csvReaderWriter = new CSVReaderWriter(_fileSystem, csvWriter, csvReader);
        }

        [Test]
        public void Should_write_columns_to_file()
        {
            // Arrange
            _fileData = new MockFileData("");
            _fileSystem.AddFile(filename, _fileData);

            // Act (grr - see notes)
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Write);
            _csvReaderWriter.Write("column1", "column2");
            _csvReaderWriter.Close();

            // Assert
            var result = _fileSystem.FileInfo.FromFileName(filename).OpenText().ReadToEnd();
            Assert.That(result, Is.EqualTo("column1\tcolumn2\r\n"), "It should format and write the given values to the output file");
        }

        [Test]
        public void Should_return_true_when_reading_correctly_formatted_file()
        {
            // Arrange
            _fileData = new MockFileData("column1\tcolumn2\r\n");
            _fileSystem.AddFile(filename, _fileData);

            // Act
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Read);
            var result = _csvReaderWriter.Read("apparentlyUnused1", "apparentlyUnused2");
            _csvReaderWriter.Close();

            // Assert
            Assert.That(result, Is.True, "It should return true");
        }

        [Test]
        public void Should_return_false_when_reading_out_empty_file()
        {
            // Arrange
            _fileData = new MockFileData("");
            _fileSystem.AddFile(filename, _fileData);

            // Act
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Read);
            string column1;
            string column2;
            var result = _csvReaderWriter.Read(out column1, out column2);
            _csvReaderWriter.Close();

            // Assert
            Assert.That(result, Is.False, "It should return false");
        }

        [Test]
        public void Should_throw_when_reading_out_incorrectly_formatted_file()
        {
            // Arrange
            _fileData = new MockFileData(" ");
            _fileSystem.AddFile(filename, _fileData);

            // Assert on Act
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Read);
            string column1;
            string column2;
            var result = _csvReaderWriter.Read(out column1, out column2);

            // Assert
            Assert.That(result, Is.False, "It should return false");
        }

        [Test]
        public void Should_return_true_and_populate_out_parameters_when_reading_out_correctly_formatted_file()
        {
            // Arrange
            _fileData = new MockFileData("column1\tcolumn2\r\n");
            _fileSystem.AddFile(filename, _fileData);

            // Act
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Read);
            string column1;
            string column2;
            var result = _csvReaderWriter.Read(out column1, out column2);
            _csvReaderWriter.Close();

            // Assert
            Assert.That(result, Is.True, "It should return true");
            Assert.That(column1, Is.EqualTo("column1"), "It should populate the first column parameter with the first value read from the line");
            Assert.That(column2, Is.EqualTo("column2"), "It should populate the second column parameter with the second value read from the line");
        }

        [Test]
        public void Should_return_true_and_populate_out_parameters_when_reading_out_correctly_formatted_multiline_file()
        {
            // Arrange
            _fileData = new MockFileData("column1a\tcolumn2a\r\ncolumn1b\tcolumn2b\r\n");
            _fileSystem.AddFile(filename, _fileData);

            // Act
            _csvReaderWriter.Open(filename, CSVReaderWriter.Mode.Read);
            string column1A;
            string column2A;
            var resultA = _csvReaderWriter.Read(out column1A, out column2A);
            string column1B;
            string column2B;
            var resultB = _csvReaderWriter.Read(out column1B, out column2B);
            _csvReaderWriter.Close();

            // Assert
            Assert.That(resultA, Is.True, "It should return true when parsing the first line");
            Assert.That(column1A, Is.EqualTo("column1a"), "It should populate the first column parameter with the correct value");
            Assert.That(column2A, Is.EqualTo("column2a"), "It should populate the second column parameter with the correct value");

            Assert.That(resultB, Is.True, "It should return true when parsing the second line");
            Assert.That(column1B, Is.EqualTo("column1b"), "It should populate the first column parameter with the correct value");
            Assert.That(column2B, Is.EqualTo("column2b"), "It should populate the second column parameter with the correct value");
        }
    }
}