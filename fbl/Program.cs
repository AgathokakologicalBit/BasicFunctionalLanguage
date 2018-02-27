using FBL.Interpretation;
using FBL.Interpretation.Modules;
using FBL.Optimization;
using FBL.Parsing;
using FBL.Parsing.Nodes;
using FBL.Tokenization;
using System;
using System.Diagnostics;
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

            var watch = Stopwatch.StartNew();
            var tokens = tokenizer.Tokenize();
            watch.Stop();
            Console.WriteLine("\n===---        STATS        ---===");

            Console.WriteLine($" [T] Tokens count: {tokens.Length}");
            Console.WriteLine($" [T] Ellapsed time: {watch.Elapsed.TotalSeconds}s");

            watch.Start();
            var ast = Parser.Parse(tokens);
            var interpreter = new Interpreter(ast.Code.Context);
            interpreter.AddModule(new LanguageModule());
            ast = Optimizer.Optimize(ast);
            watch.Stop();

            Console.WriteLine($"\n [P] Ellapsed time: {watch.Elapsed.TotalSeconds}s");

            if (ast == null)
            {
                Console.Error.WriteLine("Error occurred during compilation process");
                return;
            }

            Console.WriteLine("\n\n===---  INTERPRETATION   ---===");
            

            try
            {
                watch.Start();
                interpreter.Run(ast);
                watch.Stop();
                Console.WriteLine($"\n [E] Ellapsed time: {watch.Elapsed.TotalSeconds}s");

                Console.WriteLine("\n\n===---   RUN MAIN   ---===");

                watch.Start();
                interpreter.Run("main", new StringNode("", false));
                watch.Stop();
                Console.WriteLine($"\n [E] Ellapsed time: {watch.Elapsed.TotalSeconds}s");
            }
            catch (Exception e)
            {
                Console.WriteLine("===---   ERROR OCCURED   ---===");
                Console.WriteLine(e.Message);
            }
        }
    }
}
