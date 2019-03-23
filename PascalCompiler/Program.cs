using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            TestLexicalAnalyzer();
        }

        static void TestIOModule()
        {
            // Прочитать справочник ошибок
            var errorDigest = new Dictionary<int, string>();
            string line;
            var errorDigestFile = new StreamReader(@".\error_digest.txt", Encoding.Default);
            errorDigestFile.ReadLine();
            while ((line = errorDigestFile.ReadLine()) != null)
            {
                var lineParts = line.Split(':');
                int errorCode = int.Parse(lineParts[0]);
                string errorMessage = lineParts[1];
                errorDigest.Add(errorCode, errorMessage);
            }
            errorDigestFile.Close();

            // Прочитать список ошибок
            var errorTable = new ErrorTable(errorDigest, 59);
            var errorFile = new StreamReader(@".\errors.txt");
            while ((line = errorFile.ReadLine()) != null)
            {
                var lineParts = line.Split();
                var row = int.Parse(lineParts[0]);
                var position = int.Parse(lineParts[1]);
                var code = int.Parse(lineParts[2]);
                errorTable.Add(row - 1, new Error { Position = position, Code = code, Message = errorDigest[code] });
            }
            errorFile.Close();

            // Прочитать файл с исходным кодом
            var ioModule = new IOModule(errorTable, @".\1.pas", @".\listing.txt");
            char? ch;
            while ((ch = ioModule.NextCh()) != null)
            {
                // TODO: лексический анализатор
            }
        }

        static void TestLexicalAnalyzer()
        {
            // Прочитать справочник ошибок
            var errorDigest = new Dictionary<int, string>();
            string line;
            var errorDigestFile = new StreamReader(@".\error_digest.txt", Encoding.Default);
            errorDigestFile.ReadLine();
            while ((line = errorDigestFile.ReadLine()) != null)
            {
                var lineParts = line.Split(':');
                int errorCode = int.Parse(lineParts[0]);
                string errorMessage = lineParts[1];
                errorDigest.Add(errorCode, errorMessage);
            }
            errorDigestFile.Close();

            var errorTable = new ErrorTable(errorDigest);

            var ioModule = new IOModule(errorTable, @".\1.pas", @".\listing.txt");
            var lexicalAnalizer = new LexicalAnalyzer(ioModule);
            Symbol? symbol = null; 
            do
            {
                symbol = lexicalAnalizer.NextSymbol();
                Console.WriteLine(symbol.Value.ToString());
            }
            while (symbol != null);
        }
    }
}
