using FBL.Interpretation;

namespace FBL.Parsing.Nodes
{
    public class ExpressionNode : Node
    {
        public ExpressionNode Value { get; set; }
        public Context Context { get; set; }


        public ExpressionNode()
        {
            this.Value = this;
        }

        public virtual ExpressionNode Clone()
        {
            if (Value == this) return this;
            return Value?.Clone() ?? new ExpressionNode();
        }

        public override string ToString()
        {
            return "null";
        }
    }
}
