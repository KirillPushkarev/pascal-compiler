using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    class ErrorTable
    {
        public Dictionary<int, string> ErrorDigest { get; set; }
        public List<List<Error>> Errors { get; set; }

        public ErrorTable(Dictionary<int, string> errorDigest, int rowCount = 0)
        {
            ErrorDigest = errorDigest;
            Errors = new List<List<Error>>();
            for (int i = 0; i < rowCount; i++)
            {
                Errors.Add(new List<Error>());
            }
        }

        public void Add(int row, Error error)
        {
            Errors[row].Add(error);
        }
    }
}
