using FBL.Interpretation;
using FBL.Interpretation.Modules;
using FBL.Parsing;
using FBL.Parsing.Nodes;
using FBL.Tokenization;
using System;
using System.IO;

namespace FBL
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length == 0)
            {
                Console.WriteLine("Target file is required");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine(
                     "Error #LC001:\n" +
                    $"File {args[0]} does not exists:\n" +
                    $"  {Path.GetFullPath(args[0])}"
                );
                return;
            }

            var code = File.ReadAllText(args[0]);
            var tokenizer = new Tokenizer(code, new TokenizerOptions
            {
                SkipWhitespace = true
            });

            var tokens = tokenizer.Tokenize();
            Console.WriteLine("\n===---        STATS        ---===");

            Console.WriteLine($" [T] Tokens count: {tokens.Length}");
            
            var ast = Parser.Parse(tokens);
            if (ast == null)
            {
                Console.Error.WriteLine("Error occurred during compilation process");
                return;
            }

            Console.WriteLine("\n\n===---        AST        ---===");
            Console.WriteLine(ast?.ToString());


            Console.WriteLine("\n\n===---  INTERPRETATION   ---===");
            var interpreter = new Interpreter();
            interpreter.AddModule(new LanguageModule());

            try
            {
                interpreter.Run(ast);

                Console.WriteLine("\n\n===---   RUN MAIN   ---===");
                interpreter.Run("main", new StringNode("world"));
            } catch (Exception e)
            {
                Console.WriteLine("===---   ERROR OCCURED   ---===");
                Console.WriteLine(e.Message);
            }
        }
    }
}
