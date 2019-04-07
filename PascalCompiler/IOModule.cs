using System;
using System.IO;
using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    public class IOModule
    {
        private StreamReader inputFile;
        private StreamWriter listing;
        private ErrorTable errorTable;

        private string buffer;
        private int errorCount;

        public int CurrentRow { get; private set; } = 0;
        public int CurrentPosition { get; private set; } = 0;

        public IOModule(ErrorTable errorTable, string sourceFilename, string listingFilename)
        {
            inputFile = new StreamReader(sourceFilename);
            listing = new StreamWriter(listingFilename);
            this.errorTable = errorTable;

            ReadNextLine();
            errorTable.AddRow();
        }

        public char? NextCh()
        {
            if (buffer == null)
                return null;

            if (CurrentPosition == buffer.Length)
            {
                WriteNextLineToListing();
                ReadNextLine();

                CurrentRow++;
                errorTable.AddRow();

                if (buffer == null)
                    return null;
                else
                    CurrentPosition = 0;
            }

            return buffer[CurrentPosition++];
        }

        public Error AddError(int code, int row, int position)
        {
            var error = errorTable.Add(code, row, position, errorCount++);
            if (buffer == null)
                WriteCurrentErrorsToListing();
            return error;
        }

        private void ReadNextLine()
        {
            buffer = inputFile.ReadLine();
            if (buffer != null)
            {
                buffer = buffer.Replace("\t", "    ");
                buffer += "\n";
            }
        }

        private void WriteNextLineToListing()
        {
            buffer = buffer.Replace("\n", "\r\n");
            listing.Write("  " + (CurrentRow + 1).ToString().PadLeft(3, ' ') + "   " + buffer);
            WriteCurrentErrorsToListing();
        }

        private void WriteCurrentErrorsToListing()
        {
            foreach (var error in errorTable.Errors[CurrentRow])
            {
                listing.WriteLine(FormatErrorNumber(error));
                listing.WriteLine(("******* ") + error.Message);
            }
        }

        private string FormatErrorNumber(Error error)
        {
            return
                ("**") + string.Format("{0:D3}", (error.Number + 1)) + ("** ") +
                string.Format("{0," + (error.Position - 1) + "}", "") +
                "^Ошибка с кодом " + error.Code;
        }

        public void Dispose()
        {
            listing.WriteLine();
            listing.WriteLine("Компиляция окончена. Количество ошибок: " + errorCount + ".");

            inputFile.Close();
            listing.Close();
        }
    }
}
