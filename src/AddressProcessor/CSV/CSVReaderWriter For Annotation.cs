using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        1) List three to five key concerns with this implementation that you would discuss with the junior developer. 

        Please leave the rest of this file as it is so we can discuss your concerns during the next stage of the interview process.
        
        *) What is the responsibility of this class? Does the fact that it performs both reading and writing break SRP? Consider breaking into separate components to handle these things separately.
        *) Is it prudent to make it possible leave a StreamReader open? What would happen if another component tried to access a file that the StreamReader had open? What would happen if a consumer tried to do a write whilst the StreamReader was open? What would happen if a consumer of this class didn't close the StreamReader? Personally I'd explore if it was possible to manage the open/close state at the time you need to do a read or write. If not I'd suggest putting in guards to gracefully catch these cases.
        *) How might you go about unit testing this class? How could you get a handle on the FileInfo, or the StreamReader / StreamWriter to assert that they are behaving as expected? Consider depending on an abstract class or interface in either the constructor or a property, that we can swap out for a mock.  
        *) What is the purpose of bool Read(string column1, string column2)? The parameters are strings which behave like value types, so setting them later on in the function has no meaning. I believe the statement "if (columns.Length == 0)" represents an impossible scenario, see https://dotnetfiddle.net/J5jl6c . Given that information, it looks as though this method doesn't do anything, other than fall over when the source file data is the wrong shape, which doesn't look intentional. What real guards are there against incorrectly formatted data? Consider adding some.
        *) It's common to fail a review on badly formatted code. Run ReSharper! It's quick and in this class it'll pick up irregulaties in casing, tell you about unecessary type declarations and point out the redundant code from the previous point. Code is way easier to maintain when it conforms to a standard - StyleCop and Code Analysis would be an extension to this point.
    */

    public class CSVReaderWriterForAnnotation
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            string outPut = "";

            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i];
                if ((columns.Length - 1) != i)
                {
                    outPut += "\t";
                }
            }

            WriteLine(outPut);
        }

        public bool Read(string column1, string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();
            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            } 
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
    }
}
