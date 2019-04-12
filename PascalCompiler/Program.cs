using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PascalCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSyntacticAnalyzer();
        }

        static void TestIOModule()
        {
            var errorDigest = ReadErrorDigest();
            var errorTable = new ErrorTable(errorDigest, 59);

            var errorFile = new StreamReader(@".\errors.txt");
            string line;
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
            var errorDigest = ReadErrorDigest();
            var errorTable = new ErrorTable(errorDigest);

            var ioModule = new IOModule(errorTable, @"..\..\data\test_lex_3.pas", @"..\..\data\listing.txt");
            var lexicalAnalyzer = new LexicalAnalyzer(ioModule);

            Symbol.SymbolEnum symbol;
            Error error;
            do
            {
                lexicalAnalyzer.NextSymbol();

                if (lexicalAnalyzer.Error != null)
                {
                    error = lexicalAnalyzer.Error;
                    Console.WriteLine(error.Message);
                }
                else
                {
                    symbol = lexicalAnalyzer.CurrentSymbol;
                    Console.WriteLine(symbol.ToString());
                }
            }
            while (!lexicalAnalyzer.IsFinished);
            ioModule.Dispose();
        }

        static void TestSyntacticAnalyzer()
        {
            var errorDigest = ReadErrorDigest();
            var errorTable = new ErrorTable(errorDigest);

            var ioModule = new IOModule(errorTable, @"..\..\data\prog_Нейтрал.pas", @"..\..\data\listing.txt");
            var lexicalAnalyzer = new LexicalAnalyzer(ioModule);
            var syntacticAnalyzer = new SyntacticAnalyzer(ioModule, lexicalAnalyzer);
            syntacticAnalyzer.Run();
            ioModule.Dispose();
        }

        static Dictionary<int, string> ReadErrorDigest()
        {
            var errorDigest = new Dictionary<int, string>();
            string line;
            var errorDigestFile = new StreamReader(@"..\..\data\error_digest.txt", Encoding.Default);
            errorDigestFile.ReadLine();
            while ((line = errorDigestFile.ReadLine()) != null)
            {
                var lineParts = line.Split('|');
                int errorCode = int.Parse(lineParts[0]);
                string errorMessage = lineParts[1];
                errorDigest.Add(errorCode, errorMessage);
            }
            errorDigestFile.Close();

            return errorDigest;
        }
    }
}
