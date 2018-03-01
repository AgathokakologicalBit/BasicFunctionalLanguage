using FBL.Interpretation;
using FBL.Parsing;
using FBL.Parsing.Nodes;
using FBL.Tokenization;
using System;
using System.Collections.Generic;
using System.IO;

namespace FBL.Optimization
{
    public class Optimizer
    {
        private static List<string> includedFiles = new List<string>(100);

        private static List<string> insertionQueue = new List<string>(10);
        private static bool newDependencies = false;
        private static Dictionary<string, HashSet<string>> dependencyGraph = new Dictionary<string, HashSet<string>>();


        private static string currentCodeName = null;
        private static Dictionary<string, CoreNode> codes = null;


        public static CoreNode Optimize(CoreNode code)
        {
            int pass = 0;

            codes = new Dictionary<string, CoreNode>()
            {
                { "<INITIAL>", code }
            };
            dependencyGraph.Add("<INITIAL>", new HashSet<string>());

            BlockNode combined;
            bool anyOptimized;

            do
            {
                anyOptimized = false;
                newDependencies = false;

                combined = new BlockNode() { Context = code.Code.Context };
                foreach (var c in codes)
                    combined.Code.Add(c.Value.Code);

                File.WriteAllText($"_optimized_pass_{pass++}.fbl", combined.ToCodeString(0));

                var oldCodes = new Dictionary<string, CoreNode>(codes);
                foreach (var c in oldCodes)
                {
                    currentCodeName = c.Key;
                    var optimized = MakePass(c.Value);
                    if (optimized != c.Value)
                    {
                        codes[c.Key] = optimized;
                        anyOptimized = true;
                    }
                }
            } while (anyOptimized || newDependencies);

            combined = new BlockNode() { Context = code.Code.Context };
            foreach (var ins in Sort(codes.Keys, x => dependencyGraph[x]))
            {
                var c = codes[ins].Code;
                if (c is BlockNode cb) combined.Code.AddRange(cb.Code);
                else combined.Code.Add(c);
            }

            var opt = new CoreNode() { Code = combined };
            do
            {
                anyOptimized = false;
                File.WriteAllText($"_optimized_pass_{pass++}.fbl", opt.ToCodeString(0));

                code = opt;
                opt = MakePass(code);
            } while (opt != code);

            return opt;
        }

        public static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }


        public static CoreNode MakePass(CoreNode code)
        {
            var newCode = MakePass((dynamic)code.Code);

            if (code.Code == newCode)
                return code;

            return new CoreNode() { Code = newCode };
        }

        private static ExpressionNode StaticInclude(ExpressionNode input, Context context)
        {
            if (!(input is StringNode inString))
                throw new InvalidOperationException("A constant string must be passed to the include");

            var path = Path.GetFullPath(inString.StringValue);

            if (!dependencyGraph.ContainsKey(currentCodeName))
                dependencyGraph.Add(currentCodeName, new HashSet<string>());
            if (!dependencyGraph.ContainsKey(path))
                dependencyGraph.Add(path, new HashSet<string>());

            dependencyGraph[currentCodeName].Add(path);

            if (includedFiles.Contains(path))
                return new BlockNode();

            if (!File.Exists(path))
                throw new FileNotFoundException("Can not include the file!", path);

            Console.WriteLine($"Including: {path}");
            includedFiles.Add(path);

            var code = File.ReadAllText(path);
            var tokenizer = new Tokenizer(code, new TokenizerOptions { SkipWhitespace = true });
            var ast = Parser.Parse(tokenizer.Tokenize());

            if (ast?.Code == null)
                throw new InvalidProgramException("Can not generate AST for given file");

            codes.Add(path, ast);
            newDependencies = true;

            return new BlockNode();
        }

        public static ExpressionNode MakePass(BlockNode node)
        {
            var newNode = new BlockNode() { Context = node.Context };
            bool anyChanges = false;
            foreach (var line in node.Code)
            {
                var newExpression = MakePass((dynamic)line);
                anyChanges |= newExpression != line;

                if (newExpression is BlockNode nb)
                {
                    newNode.Code.AddRange(nb.Code);
                    anyChanges = true;
                }
                else
                {
                    newNode.Code.Add(newExpression);
                }
            }

            if (newNode.Code.Count == 1)
                return newNode.Code[0];

            return anyChanges ? newNode : node;
        }

        public static ExpressionNode MakePass(FunctionCallNode node)
        {
            var callee = MakePass((dynamic)node.CalleeExpression);
            var argument = MakePass((dynamic)node.Argument);

            ExpressionNode result = node;
            if (node.CalleeExpression != callee || node.Argument != argument)
                result = new FunctionCallNode { CalleeExpression = callee, Argument = argument };

            if (callee is FunctionNode cf && cf.Function != null
                && cf.CheckedPure && cf.IsPureIn && cf.IsPureOut
                && (argument is NumberNode || argument is StringNode))
            {
                return cf.Function(argument, node.Context);
            }

            return result;
        }

        public static ExpressionNode MakePass(FunctionNode node)
        {
            if (node.Function != null)
                return node;

            var newCode = MakePass((dynamic)node.Code);
            if (newCode != node.Code)
            {
                return new FunctionNode()
                {
                    Code = newCode,
                    Parameter = node.Parameter,

                    CheckedPure = node.CheckedPure,
                    IsPureIn = node.IsPureIn,
                    IsPureOut = node.IsPureOut,

                    Context = node.Context
                };
            }

            return node;
        }

        public static ExpressionNode MakePass(VariableNode node)
        {
            if (node.Name != "include")
                return node;

            return new FunctionNode(StaticInclude, true, true);
        }

        public static ExpressionNode MakePass(ExpressionNode node)
        {
            // Too simple, nothing can be done
            return node;
        }
    }
}
