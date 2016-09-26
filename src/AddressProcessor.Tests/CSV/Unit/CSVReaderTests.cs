using System.IO;
using System.IO.Abstractions;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV.Unit
{
    [TestFixture]
    public class CSVReaderTests
    {
        private CSVReaderTestDouble _csvReader;
        private Mock<IFileSystem> _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Mock<IFileSystem>();
            _csvReader = new CSVReaderTestDouble(_fileSystem.Object);
        }

        [Test]
        public void Should_create_TextWriter_from_file_when_opening()
        {
            // Arrange
            var streamFromFile = new StreamReader("notempty");
            _fileSystem.Setup(x => x.File.OpenText("filename.txt")).Returns(streamFromFile);

            // Act
            _csvReader.Open("filename.txt");

            // Asserrt
            Assert.That(_csvReader.TextReader, Is.SameAs(streamFromFile), "It should create the TextReader using the specified file");
        }

        [Test]
        public void Should_parse_corectly_formatted_data_from_TextReader()
        {
            // Arrange
            var textReader = new Mock<TextReader>();
            textReader.Setup(x => x.ReadLine()).Returns("column1\tcolumn2");
            _csvReader.TextReader = textReader.Object;

            // Act
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);

            // Assert
            Assert.That(result, Is.True, "It should return true");
            Assert.That(column1, Is.EqualTo("column1"), "It should correctly parse the first column");
            Assert.That(column2, Is.EqualTo("column2"), "It should correctly parse the second column");
        }

        [Test]
        public void Should_not_parse_empty_data_from_TextReader()
        {
            // Arrange
            var textReader = new Mock<TextReader>();
            textReader.Setup(x => x.ReadLine()).Returns((string)null);
            _csvReader.TextReader = textReader.Object;

            // Act
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);

            // Assert
            Assert.That(result, Is.False, "It should return false");
            Assert.That(column1, Is.Null, "It should return null for the first column");
            Assert.That(column2, Is.Null, "It should return null for the second column");
        }

        [Test]
        public void Should_not_parse_incorrectly_formatted_data_from_TextReader()
        {
            // Arrange
            var textReader = new Mock<TextReader>();
            textReader.Setup(x => x.ReadLine()).Returns("something unexpected");
            _csvReader.TextReader = textReader.Object;

            // Act
            string column1;
            string column2;
            var result = _csvReader.Read(out column1, out column2);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(column1, Is.Null, "It should return null for the first column");
            Assert.That(column2, Is.Null, "It should return null for the second column");
        }

        [Test]
        public void Should_close_TextReader_on_close()
        {
            // Arrange
            var textReader = new Mock<TextReader>();
            _csvReader.TextReader = textReader.Object;

            //Act
            _csvReader.Close();

            // Assert
            textReader.Verify(x => x.Close(), "It should close the TextReader");
        }
    }

    public class CSVReaderTestDouble : CSVReader
    {
        public CSVReaderTestDouble(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public TextReader TextReader
        {
            get { return base.TextReader; }
            set { base.TextReader = value; }
        }
    }
}
