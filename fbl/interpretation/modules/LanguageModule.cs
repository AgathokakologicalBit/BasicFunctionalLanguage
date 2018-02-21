using FBL.Parsing.Nodes;
using System;

namespace FBL.Interpretation.Modules
{
    public class LanguageModule : IModule
    {
        private Interpreter interpreter;

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
                "set",
                new FunctionNode(Set) { Parameter = new VariableNode("name") },
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


            interpreter.SetVariable("true", new NumberNode(1), context);
            interpreter.SetVariable("false", new NumberNode(0), context);
            interpreter.SetVariable(
                "not",
                new FunctionNode((e) => new NumberNode(ToNumber(e).NumericValue == "0" ? 1 : 0))
                { Parameter = new VariableNode("value") },
                context
            );
            interpreter.SetVariable(
                "if",
                new FunctionNode(IfExpression)
                { Parameter = new VariableNode("condition") },
                context
            );
        }

        ExpressionNode Import(ExpressionNode input)
        {
            // TODO: Do something Oo
            return new ExpressionNode();
        }

        NumberNode NumberAbsolute(ExpressionNode input)
            => new NumberNode(Math.Abs(decimal.Parse(ToNumber(input).NumericValue)));

        ExpressionNode IfExpression(ExpressionNode condition)
            => new FunctionNode((ExpressionNode onTrue)
                => new FunctionNode((ExpressionNode onFalse) => ToNumber(condition).NumericValue == "0" ? onFalse : onTrue)
                { Parameter = new VariableNode("on_false") })
            { Parameter = new VariableNode("on_true") };

        ExpressionNode Set(ExpressionNode input)
            => new FunctionNode((v) => interpreter.ChangeValue(input, v, input.Context))
            { Parameter = new VariableNode("right") };

        ExpressionNode Input(ExpressionNode type)
        {
            var data = new StringNode(Console.ReadLine());
            var result = interpreter.Run(type, data);
            return result;
        }

        ExpressionNode Print(ExpressionNode node)
        {
            Console.WriteLine(node?.ToString() ?? "null");
            return node;
        }

        NumberNode ToInt(ExpressionNode node)
        {
            if (int.TryParse(node.ToString(), out int value))
                return new NumberNode(value.ToString(), false);

            return new NumberNode("0", false);
        }

        NumberNode ToNumber(ExpressionNode node)
        {
            if (decimal.TryParse(node.ToString(), out decimal value))
                return new NumberNode(value.ToString(), true);

            return new NumberNode("0", false);
        }

        StringNode ToString(ExpressionNode node) => new StringNode(node.ToString());


        ExpressionNode Add(ExpressionNode left)
        {
            return new FunctionNode(
                (right) =>
                {
                    if (left is NumberNode && right is NumberNode)
                    {
                        return new NumberNode(
                              decimal.Parse(ToNumber(left).NumericValue)
                            + decimal.Parse(ToNumber(right).NumericValue));
                    }

                    return new StringNode(left.ToString() + right.ToString());
                }
            )
            { Parameter = new VariableNode("right") };
        }
        
        ExpressionNode NumbersSub(ExpressionNode left)
        {
            return new FunctionNode(
                (right) => new NumberNode((
                       decimal.Parse(ToNumber(left).NumericValue)
                     - decimal.Parse(ToNumber(right).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMul(ExpressionNode left)
        {
            return new FunctionNode(
                (right) => new NumberNode((
                       decimal.Parse(ToNumber(left).NumericValue)
                     * decimal.Parse(ToNumber(right).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersDiv(ExpressionNode left)
        {
            return new FunctionNode(
                (right) => new NumberNode((
                       decimal.Parse(ToNumber(left).NumericValue)
                     / decimal.Parse(ToNumber(right).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }

        ExpressionNode NumbersMod(ExpressionNode left)
        {
            return new FunctionNode(
                (right) => new NumberNode((
                       decimal.Parse(ToNumber(left).NumericValue)
                     % decimal.Parse(ToNumber(right).NumericValue)
                    ).ToString(), true)
            )
            { Parameter = new VariableNode("right") };
        }
    }
}
