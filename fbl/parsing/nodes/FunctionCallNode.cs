namespace FBL.Parsing.Nodes
{
    public class FunctionCallNode : ExpressionNode
    {
        public ExpressionNode CalleeExpression { get; set; }
        public ExpressionNode Argument { get; set; }


        public override string ToString()
        {
            return $"[ {CalleeExpression} \u2190 {Argument} ]";
        }

        public override string ToCodeString(int depth)
        {
            var argStr = Argument.ToCodeString(depth);
            if (!(Argument is NumberNode || Argument is StringNode || Argument is VariableNode || Argument is FunctionNode))
                argStr = $"( {argStr} )";

            return $"{CalleeExpression.ToCodeString(depth)} {argStr}";
        }
    }
}
