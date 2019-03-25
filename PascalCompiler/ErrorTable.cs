using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    public class ErrorTable
    {
        const int MAX_ERROR_IN_ROW_COUNT = 5;
        const int MAX_ERROR_TOTAL_COUNT = 20;

        public Dictionary<int, string> ErrorDigest { get; set; }
        public List<List<Error>> Errors { get; set; }
        public int ErrorCount { get; private set; } = 0;

        public ErrorTable(Dictionary<int, string> errorDigest, int rowCount = 0)
        {
            ErrorDigest = errorDigest;
            Errors = new List<List<Error>>();
            for (int i = 0; i < rowCount; i++)
            {
                Errors.Add(new List<Error>());
            }
        }

        public void AddRow()
        {
            Errors.Add(new List<Error>());
        }

        public Error Add(int row, Error error)
        {
            if (Errors[row].Count >= MAX_ERROR_IN_ROW_COUNT ||
                ErrorCount > MAX_ERROR_TOTAL_COUNT) return null;

            Errors[row].Add(error);
            ErrorCount++;

            return error;
        }

        public Error Add(int code, int row, int position, int number)
        {
            var error = new Error { Code = code, Position = position, Message = ErrorDigest[code], Number = number };
            Add(row, error);

            return error;
        }
    }
}
