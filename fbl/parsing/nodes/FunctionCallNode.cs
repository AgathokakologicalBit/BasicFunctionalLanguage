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
    }
}
