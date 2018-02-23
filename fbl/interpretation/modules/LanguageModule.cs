using FBL.Parsing;
using FBL.Parsing.Nodes;
using FBL.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FBL.Interpretation.Modules
{
    public class LanguageModule : IModule
    {
        private Interpreter interpreter;
        private HashSet<string> includedFiles = new HashSet<string>();

        private FunctionNode PutsFunctionNode = null;


        void IModule.OnLoad(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            var context = interpreter.GetGlobalContext();

            interpreter.SetVariable(
                "import",
                new FunctionNode(Import) { Parameter = new VariableNode("module") },
                context
            );
            interpreter.SetVariable(
                "include",
                new FunctionNode(Include) { Parameter = new VariableNode("filename") },
                context
            );

            // TODO: Temporary
            interpreter.SetVariable(
                "input",
                new FunctionNode(Input) { Parameter = new VariableNode("type") },
                context
            );
            interpreter.SetVariable(
                "print",
                new FunctionNode(Print) { Parameter = new VariableNode("value") },
                context
            );
            interpreter.SetVariable(
                "puts",
                PutsFunctionNode = new FunctionNode(Puts) { Parameter = new VariableNode("value") },
                context
            );

            interpreter.SetVariable(
                "int",
                new FunctionNode(ToInt) { Parameter = new VariableNode("value") },
                context
            );
            interpreter.SetVariable(
                "number",
                new FunctionNode(ToNumber) { Parameter = new VariableNode("value") },
                context
            );
            interpreter.SetVariable(
                "string",
                new FunctionNode(ToString) { Parameter = new VariableNode("value") },
                context
            );


            interpreter.SetVariable(
                "abs",
                new FunctionNode(NumberAbsolute) { Parameter = new VariableNode("value") },
                context
            );


            interpreter.SetVariable(
                "+",
                new FunctionNode(Add) { Parameter = new VariableNode("left") },
                context
            );
            interpreter.SetVariable(
                "-",
                new FunctionNode(NumbersSub) { Parameter = new VariableNode("left") },
                context
            );
            interpreter.SetVariable(
                "*",
                new FunctionNode(NumbersMul) { Parameter = new VariableNode("left") },
                context
            );
            interpreter.SetVariable(
                "/",
                new FunctionNode(NumbersDiv) { Parameter = new VariableNode("left") },
                context
            );
            interpreter.SetVariable(
                "%",
                new FunctionNode(NumbersMod) { Parameter = new VariableNode("left") },
                context
            );

            interpreter.SetVariable(
                "if",
                new FunctionNode(IfExpression)
                { Parameter = new VariableNode("condition") },
                context
            );
            interpreter.SetVariable(
                "equals",
                new FunctionNode(
                    (a, c1) => new FunctionNode(
                        (b, c2) => new NumberNode(a.GetType() == b.GetType() && a.ToString() == b.ToString() ? 1 : 0)
                    )
                    { Parameter = new VariableNode("right") })
                { Parameter = new VariableNode("left") },
                context
            );
        }

        ExpressionNode Import(ExpressionNode input, Context context)
        {
            // TODO: Do something Oo
            return new ExpressionNode();
        }

        ExpressionNode Include(ExpressionNode input, Context context)
        {
            var path = Path.GetFullPath(ToString(input, context).StringValue);
            if (!File.Exists(path)) return new ExpressionNode();

            if (includedFiles.Contains(path))
                return new ExpressionNode();

            includedFiles.Add(path);

            var code = File.ReadAllText(path);
            var tokenizer = new Tokenizer(code, new TokenizerOptions { SkipWhitespace = true });
            var ast = Parser.Parse(tokenizer.Tokenize());

            return interpreter.Run(ast);
        }

        NumberNode NumberAbsolute(ExpressionNode input, Context context)
            => new NumberNode(Math.Abs(decimal.Parse(ToNumber(input, context).NumericValue)));

        ExpressionNode IfExpression(ExpressionNode condition, Context context)
            => new FunctionNode((ExpressionNode onTrue, Context context_true)
                => new FunctionNode((ExpressionNode onFalse, Context context_false)
                    => ToNumber(condition, context).NumericValue == "0" ? onFalse : onTrue)
                { Parameter = new VariableNode("on_false") })
            { Parameter = new VariableNode("on_true") };

        ExpressionNode Input(ExpressionNode type, Context context)
        {
            var data = new StringNode(Console.ReadLine());
            var result = interpreter.Run(type, data);
            return result;
        }

        ExpressionNode Print(ExpressionNode node, Context context)
        {
            Console.WriteLine(node?.ToString() ?? "null");
            return node;
        }

        ExpressionNode Puts(ExpressionNode node, Context context)
        {
            Console.Write(node?.ToString() ?? String.Empty);
            return PutsFunctionNode;
        }

        public static NumberNode ToInt(ExpressionNode node, Context context)
        {
            if (int.TryParse(GetLeadingInt(node.ToString()), NumberStyles.Number, CultureInfo.InvariantCulture, out int value))
                return new NumberNode(value.ToString(CultureInfo.InvariantCulture), false);

            return new NumberNode("0", false);
        }

        static string GetLeadingInt(string input)
            => new string(input.Trim().TakeWhile((c) => Char.IsDigit(c)).ToArray());

        public static NumberNode ToNumber(ExpressionNode node, Context context)
        {
            if (decimal.TryParse(node.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                return new NumberNode(value.ToString(CultureInfo.InvariantCulture), true);

            return new NumberNode("0", false);
        }

        public static StringNode ToString(ExpressionNode node, Context context)
            => new StringNode(node.ToString());


        ExpressionNode Add(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) =>
                {
                    if (left is NumberNode && right is NumberNode)
                    {
                        return new NumberNode(
                              decimal.Parse(ToNumber(left, c).NumericValue)
                            + decimal.Parse(ToNumber(right, c).NumericValue));
                    }

                    return new StringNode(left.ToString() + right.ToString());
                }
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersSub(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode((
                       decimal.Parse(ToNumber(left, c).NumericValue)
                     - decimal.Parse(ToNumber(right, c).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMul(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode((
                       decimal.Parse(ToNumber(left, c).NumericValue)
                     * decimal.Parse(ToNumber(right, c).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersDiv(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode((
                       decimal.Parse(ToNumber(left, c).NumericValue)
                     / decimal.Parse(ToNumber(right, c).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMod(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode((
                       decimal.Parse(ToNumber(left, c).NumericValue)
                     % decimal.Parse(ToNumber(right, c).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }
    }
}
