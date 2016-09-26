using System;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV.Unit
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
        private CSVReaderWriter _csvReaderWriter;
        private Mock<ICSVWriter> _csvWriter;
        private Mock<ICSVReader> _csvReader;

        [SetUp]
        public void SetUp()
        {
            _csvWriter = new Mock<ICSVWriter>();
            _csvReader = new Mock<ICSVReader>();
            _csvReaderWriter = new CSVReaderWriter(_csvWriter.Object, _csvReader.Object);
        }

        [Test]
        public void Should_open_CSVReader()
        {
            // Arrange

            // Act
            _csvReaderWriter.Open("filename.txt", CSVReaderWriter.Mode.Read);

            // Asserrt
            _csvReader.Verify(x => x.Open("filename.txt"), "It should defer opening to the the ICSVReader");
        }

        [Test]
        public void Should_open_CSVWriter()
        {
            // Arrange

            // Act
            _csvReaderWriter.Open("filename.txt", CSVReaderWriter.Mode.Write);

            // Asserrt
            _csvWriter.Verify(x => x.Open("filename.txt"), "It should defer opening to the the ICSVWriter");
        }

        [Test]
        public void Should_throw_unknown_file_mode()
        {
            // Arrange
            var unknownFileMode = (CSVReaderWriter.Mode)5;

            // Act / Assert
            var exception = Assert.Throws<Exception>(() => _csvReaderWriter.Open("filename.txt", unknownFileMode), "It should thow an exception");
            Assert.That(exception.Message, Is.EqualTo("Unknown file mode for filename.txt"), "It should return an informative message");
        }

        [Test]
        public void Should_close_both_CSVWriter_and_CSVReader()
        {
            // Arrange

            // Act
            _csvReaderWriter.Close();

            // Asserrt
            _csvWriter.Verify(x => x.Close(), "It should close the ICSVWriter");
            _csvReader.Verify(x => x.Close(), "It should close the ICSVReader");
        }

        [Test]
        public void Should_read_with_the_CSVReader()
        {
            string column1;
            string column2;

            // Arrange
            _csvReader.Setup(x => x.Read(out column1, out column2)).Returns(true);

            // Act
            var result = _csvReaderWriter.Read(out column1, out column2);

            // Asserrt
            _csvReader.Verify(x => x.Read(out column1, out column2), "It should use the read function from the ICSVReader");
            Assert.That(result, Is.True, "It should return the result from the ICSVReader");
        }

        [Test]
        public void Should_write_with_the_CSVWriter()
        {
            // Arrange
            string[] columns = { };

            // Act
            _csvReaderWriter.Write(columns);

            // Assert
            _csvWriter.Verify(x => x.Write(columns), "It should use the write function from the ICSVWriter");
        }
    }
}
