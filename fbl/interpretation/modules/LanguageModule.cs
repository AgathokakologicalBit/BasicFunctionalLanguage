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
                new FunctionNode((e, c) => new NumberNode(ToNumber(e, c).NumericValue == "0" ? 1 : 0))
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

        ExpressionNode Import(ExpressionNode input, Context context)
        {
            // TODO: Do something Oo
            return new ExpressionNode();
        }

        NumberNode NumberAbsolute(ExpressionNode input, Context context)
            => new NumberNode(Math.Abs(decimal.Parse(ToNumber(input, context).NumericValue)));

        ExpressionNode IfExpression(ExpressionNode condition, Context context)
            => new FunctionNode((ExpressionNode onTrue, Context context_true)
                => new FunctionNode((ExpressionNode onFalse, Context context_false)
                    => ToNumber(condition, context).NumericValue == "0" ? onFalse : onTrue)
                { Parameter = new VariableNode("on_false") })
            { Parameter = new VariableNode("on_true") };

        ExpressionNode Set(ExpressionNode input, Context context)
            => new FunctionNode((v, c) => interpreter.ChangeValue(input, v?.Clone(), context))
            { Parameter = new VariableNode("right") };

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

        NumberNode ToInt(ExpressionNode node, Context context)
        {
            if (int.TryParse(node.ToString(), out int value))
                return new NumberNode(value.ToString(), false);

            return new NumberNode("0", false);
        }

        NumberNode ToNumber(ExpressionNode node, Context context)
        {
            if (decimal.TryParse(node.ToString(), out decimal value))
                return new NumberNode(value.ToString(), true);

            return new NumberNode("0", false);
        }

        StringNode ToString(ExpressionNode node, Context context) => new StringNode(node.ToString());


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
