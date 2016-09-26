using System.IO.Abstractions.TestingHelpers;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV.Integration
{
    [TestFixture]
    public class CSVReaderTests
    {
        private const string Filename = "C:\\filename.txt";

        private MockFileSystem _fileSystem;
        private MockFileData _fileData;

        private CSVReader _csvReader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _csvReader = new CSVReader(_fileSystem);
        }
        
        [Test]
        public void Should_return_false_when_reading_out_empty_file()
        {
            // Arrange
            _fileData = new MockFileData("");
            _fileSystem.AddFile(Filename, _fileData);

            // Act
            _csvReader.Open(Filename);
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);
            _csvReader.Close();

            // Assert
            Assert.That(result, Is.False, "It should return false");
        }

        [Test]
        public void Should_return_false_when_reading_out_incorrectly_formatted_file()
        {
            // Arrange
            _fileData = new MockFileData(" ");
            _fileSystem.AddFile(Filename, _fileData);

            // Act
            _csvReader.Open(Filename);
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);
            _csvReader.Close();

            // Assert
            Assert.That(result, Is.False, "It should return false");
        }

        [Test]
        public void Should_return_true_and_populate_out_parameters_when_reading_out_correctly_formatted_file()
        {
            // Arrange
            _fileData = new MockFileData("column1\tcolumn2\r\n");
            _fileSystem.AddFile(Filename, _fileData);

            // Act
            _csvReader.Open(Filename);
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);
            _csvReader.Close();

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
            _fileSystem.AddFile(Filename, _fileData);

            // Act
            _csvReader.Open(Filename);
            string column1A;
            string column2A;
            var resultA = _csvReader.Read(out column1A, out column2A);
            string column1B;
            string column2B;
            var resultB = _csvReader.Read(out column1B, out column2B);
            _csvReader.Close();

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