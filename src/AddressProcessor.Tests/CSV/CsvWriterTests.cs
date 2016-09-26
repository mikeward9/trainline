using System.IO;
using System.IO.Abstractions;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV
{
    [TestFixture]
    public class CSVWriterTests
    {
        private CSVWriterTestDouble _csvWriter;
        private Mock<IFileSystem> _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Mock<IFileSystem>();
            _csvWriter = new CSVWriterTestDouble(_fileSystem.Object);
        }

        [Test]
        public void Should_create_TextWriter_from_file_when_opening()
        {
            // Arrange
            var fileInfoBase = new Mock<FileInfoBase>();
            var streamFromFile = new StreamWriter("notempty");
            fileInfoBase.Setup(x => x.CreateText()).Returns(streamFromFile);
            _fileSystem.Setup(x => x.FileInfo.FromFileName("filename.txt")).Returns(fileInfoBase.Object);

            // Act
            _csvWriter.Open("filename.txt");

            // Asserrt
            Assert.That(_csvWriter.TextWriter, Is.SameAs(streamFromFile), "It should create the TextWriter using the specified file");
        }

        [Test]
        public void Should_write_to_TextWriter_on_write()
        {
            // Arrange
            var textWriter = new Mock<TextWriter>();
            _csvWriter.TextWriter = textWriter.Object;

            // Act
            _csvWriter.Write("column1", "column2");

            // Assert
            textWriter.Verify(x=>x.WriteLine("column1\tcolumn2"), "It should format and write the input to the TextWriter");
        }

        [Test]
        public void Should_close_TextWriter_on_close()
        {
            // Arrange
            var textWriter = new Mock<TextWriter>();
            _csvWriter.TextWriter = textWriter.Object;

            //Act
            _csvWriter.Close();
            // Assert
            textWriter.Verify(x => x.Close(), "It should close the TextWriter");
        }
    }

    public class CSVWriterTestDouble : CSVWriter
    {
        public CSVWriterTestDouble(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public TextWriter TextWriter
        {
            get { return base.TextWriter; }
            set { base.TextWriter = value; }
        }
    }
}
