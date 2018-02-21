using FBL.Interpretation;

namespace FBL.Parsing.Nodes
{
    public class FunctionCallNode : ExpressionNode
    {
        public ExpressionNode CalleeExpression { get; set; }
        public ExpressionNode Argument { get; set; }


        public FunctionCallNode()
        {
            Value = this;
        }


        public override string ToString()
        {
            return $"[ {CalleeExpression} \u2190 {Argument} ]";
        }


        public override ExpressionNode Clone()
        {
            return new FunctionCallNode
            {
                CalleeExpression = CalleeExpression.Clone(),
                Argument = Argument.Clone(),
                Context = Context
            };
        }
    }
}
