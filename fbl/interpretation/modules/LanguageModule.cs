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
        private long NextVisitId = 0;


        void IModule.OnLoad(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            var context = interpreter.GetGlobalContext();

            context.SetVariable(
                "include",
                new FunctionNode(Include, false, false) { Parameter = new VariableNode("filename") }
            );

            // TODO: Temporary
            context.SetVariable(
                "input",
                new FunctionNode(Input, true, true) { Parameter = new VariableNode("type") }
            );
            context.SetVariable(
                "print",
                new FunctionNode(Print, false, true) { Parameter = new VariableNode("value") }
            );
            context.SetVariable(
                "puts",
                PutsFunctionNode = new FunctionNode(Puts, false, true) { Parameter = new VariableNode("value") }
            );

            context.SetVariable(
                "int",
                new FunctionNode(ToInt, true, true) { Parameter = new VariableNode("value") }
            );
            context.SetVariable(
                "number",
                new FunctionNode(ToNumber, true, true) { Parameter = new VariableNode("value") }
            );
            context.SetVariable(
                "string",
                new FunctionNode(ToString, true, true) { Parameter = new VariableNode("value") }
            );

            context.SetVariable(
                "get_type",
                new FunctionNode(
                    (i, c) => new StringNode(i?.GetType().Name.Replace("Node", "").ToLower(), false), true, true)
                { Parameter = new VariableNode("value") }
            );

            context.SetVariable(
                "abs",
                new FunctionNode(NumberAbsolute, true, true) { Parameter = new VariableNode("value") }
            );


            context.SetVariable(
                "+",
                new FunctionNode(Add, true, true) { Parameter = new VariableNode("left") }
            );
            context.SetVariable(
                "-",
                new FunctionNode(NumbersSub, true, true) { Parameter = new VariableNode("left") }
            );
            context.SetVariable(
                "*",
                new FunctionNode(NumbersMul, true, true) { Parameter = new VariableNode("left") }
            );
            context.SetVariable(
                "/",
                new FunctionNode(NumbersDiv, true, true) { Parameter = new VariableNode("left") }
            );
            context.SetVariable(
                "%",
                new FunctionNode(NumbersMod, true, true) { Parameter = new VariableNode("left") }
            );

            context.SetVariable(
                "if",
                new FunctionNode(IfExpression, true, true)
                { Parameter = new VariableNode("condition") }
            );
            context.SetVariable(
                "equals",
                new FunctionNode(
                    (a, c1) => new FunctionNode(
                        (b, c2) => new NumberNode(a.DeepEquals(b, NextVisitId++) ? 1 : 0),
                        false, true
                    )
                    { Parameter = new VariableNode("right") }, false, true)
                { Parameter = new VariableNode("left") }
            );
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
            => new NumberNode(Math.Abs(ToNumber(input, context).NumericValue));

        ExpressionNode IfExpression(ExpressionNode condition, Context context)
            => new FunctionNode((ExpressionNode onTrue, Context context_true)
                => new FunctionNode((ExpressionNode onFalse, Context context_false)
                    => ToNumber(condition, context).NumericValue == 0 ? onFalse : onTrue, true, true)
                { Parameter = new VariableNode("on_false") }, true, true)
            { Parameter = new VariableNode("on_true") };

        ExpressionNode Input(ExpressionNode type, Context context)
        {
            var data = new StringNode(Console.ReadLine(), false);
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
                return new NumberNode(value);

            return new NumberNode(0);
        }

        static string GetLeadingInt(string input)
            => new string(input.Trim().TakeWhile((c) => Char.IsDigit(c)).ToArray());

        public static NumberNode ToNumber(ExpressionNode node, Context context)
        {
            if (decimal.TryParse(node.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                return new NumberNode(value);

            return new NumberNode(0);
        }

        public static StringNode ToString(ExpressionNode node, Context context)
            => new StringNode(node.ToString(), false);


        ExpressionNode Add(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) =>
                {
                    if (left is NumberNode && right is NumberNode)
                    {
                        return new NumberNode(
                              ToNumber(left, c).NumericValue
                            + ToNumber(right, c).NumericValue);
                    }

                    return new StringNode(left.ToString() + right.ToString(), false);
                }, true, true
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersSub(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode(
                       ToNumber(left, c).NumericValue
                     - ToNumber(right, c).NumericValue
                    ), true, true
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMul(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode(
                       ToNumber(left, c).NumericValue
                     * ToNumber(right, c).NumericValue
                    ), true, true
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersDiv(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode(
                       ToNumber(left, c).NumericValue
                     / ToNumber(right, c).NumericValue
                    ), true, true
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMod(ExpressionNode left, Context context)
        {
            return new FunctionNode(
                (right, c) => new NumberNode(
                       ToNumber(left, c).NumericValue
                     % ToNumber(right, c).NumericValue
                    ), true, true
            )
            { Parameter = new VariableNode("right") };
        }
    }
}
