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
        int currentRow = 0;
        int currentPosition = 0;

        public IOModule(ErrorTable errorTable, string sourceFilename, string listingFilename)
        {
            inputFile = new StreamReader(sourceFilename);
            listing = new StreamWriter(listingFilename);
            this.errorTable = errorTable;

            ReadNextLine();
        }

        public char? NextCh()
        {
            if (buffer == null)
                return null;

            if (currentPosition == buffer.Length)
            {
                WriteNextLineToListing();
                ReadNextLine();
                if (buffer == null)
                    return null;

                currentRow++;
                currentPosition = 0;
            }

            return buffer[currentPosition++];
        }

        public void AddError(int code, string message)
        {
            errorTable.Add(currentRow, new Error { Code = code, Position = currentPosition, Message = message });
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
            listing.WriteLine("  " + (currentRow + 1).ToString().PadLeft(3, '0') + "   " + buffer);
            foreach (var error in errorTable.Errors[currentRow])
            {
                listing.WriteLine(("******* ") + string.Format("{0," + (error.Position - 1) + "}", "") + "^Ошибка с кодом " + error.Code);
                listing.WriteLine(("******* ") + error.Message);
            }
        }
    }
}
