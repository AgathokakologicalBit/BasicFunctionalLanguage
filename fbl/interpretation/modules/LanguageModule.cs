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
                new FunctionNode(Import, context) { Parameter = new VariableNode("module") },
                context
            );
            interpreter.SetVariable(
                "set",
                new FunctionNode(Set, context) { Parameter = new VariableNode("name") },
                context
            );

            // TODO: Temporary
            interpreter.SetVariable(
                "input",
                new FunctionNode(Input, context) { Parameter = new VariableNode("type") },
                context
            );
            interpreter.SetVariable(
                "print",
                new FunctionNode(Print, context) { Parameter = new VariableNode("value") },
                context
            );

            interpreter.SetVariable(
                "int",
                new FunctionNode(ToInt, context) { Parameter = new VariableNode("value") },
                context
            );
            interpreter.SetVariable(
                "string",
                new FunctionNode(ToString, context) { Parameter = new VariableNode("value") },
                context
            );

            interpreter.SetVariable(
                "+",
                new FunctionNode(Add, context) { Parameter = new VariableNode("left") },
                context
            );
            interpreter.SetVariable(
                "-",
                new FunctionNode(NumbersSub, context) { Parameter = new VariableNode("left") },
                context
            );
        }

        ExpressionNode Import(ExpressionNode input)
        {
            // TODO: Do something Oo
            return new ExpressionNode();
        }

        ExpressionNode Set(ExpressionNode input)
            => new FunctionNode((v) => interpreter.ChangeValue(input, v, input.Context), input.Context)
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

        StringNode ToString(ExpressionNode node) => new StringNode(node.ToString());


        ExpressionNode Add(ExpressionNode left)
        {
            return new FunctionNode(
                (right) =>
                {
                    if (left is NumberNode && right is NumberNode)
                    {
                        return new NumberNode(
                            + double.Parse(ToInt(left).NumericValue)
                            + double.Parse(ToInt(right).NumericValue));
                    }

                    return new StringNode(left.ToString() + right.ToString());
                }, interpreter.GetGlobalContext()
            )
            { Parameter = new VariableNode("right") };
        }
        
        ExpressionNode NumbersSub(ExpressionNode left)
        {
            return new FunctionNode(
                (right) => new NumberNode((
                     + int.Parse(ToInt(left).NumericValue)
                     - int.Parse(ToInt(right).NumericValue)
                    ).ToString(), false)
                , interpreter.GetGlobalContext()
            )
            { Parameter = new VariableNode("right") };
        }
    }
}
