using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    class IOModule
    {
        StreamReader inputFile;
        StreamWriter listing;
        ErrorTable errorTable;
        string buffer;

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
                if (buffer == null)
                    return null;

                CurrentRow++;
                CurrentPosition = 0;
                errorTable.AddRow();
            }

            return buffer[CurrentPosition++];
        }

        public void AddError(int code)
        {
            errorTable.Add(CurrentRow, CurrentPosition, code);
        }

        public void AddError(int code, int row, int position)
        {
            errorTable.Add(row, position, code);
        }

        private void ReadNextLine()
        {
            buffer = inputFile.ReadLine();
            if (buffer == null)
            {
                inputFile.Close();
                listing.Close();
            }
        }

        private void WriteNextLineToListing()
        {
            listing.WriteLine("  " + (CurrentRow + 1).ToString().PadLeft(3, ' ') + "   " + buffer);
            foreach (var error in errorTable.Errors[CurrentRow])
            {
                listing.WriteLine(("******* ") + string.Format("{0," + (error.Position - 1) + "}", "") + "^Ошибка с кодом " + error.Code);
                listing.WriteLine(("******* ") + error.Message);
            }
        }
    }
}
