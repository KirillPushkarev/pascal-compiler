using System.IO;

namespace PascalCompiler
{
    public class IOModule
    {
        private string listingFilename;

        private StreamReader inputFile;
        private StreamWriter listing;
        private ErrorTable errorTable;

        private string buffer;
        private int errorCount;

        public int CurrentRow { get; private set; } = 0;
        public int CurrentPosition { get; private set; } = 0;

        public IOModule(ErrorTable errorTable, string sourceFilename, string listingFilename)
        {
            this.inputFile = new StreamReader(sourceFilename);
            this.listingFilename = listingFilename;
            this.listing = new StreamWriter(listingFilename);
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
                CurrentPosition = 0;
                errorTable.AddRow();

                if (buffer == null)
                    return null;
            }

            return buffer[CurrentPosition++];
        }

        public Error AddError(int code, int row, int position)
        {
            Error error = errorTable.Add(code, row, position, errorCount++);

            if (buffer == null)
            {
                listing = new StreamWriter(listingFilename, true);
                listing.WriteLine(FormatErrorNumber(error));
                listing.WriteLine(("******* ") + error.Message);
                listing.Close();
            }

            return error;
        }

        private void ReadNextLine()
        {
            buffer = inputFile.ReadLine();

            if (buffer == null)
            {
                inputFile.Close();
                listing.Close();
                return;
            }

            buffer += "\n";
        }

        private void WriteNextLineToListing()
        {
            buffer = buffer.Replace("\n", "\r\n");
            listing.Write("  " + (CurrentRow + 1).ToString().PadLeft(3, ' ') + "   " + buffer);
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
    }
}
