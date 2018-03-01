using FBL.Interpretation;

namespace FBL.Parsing.Nodes
{
    public class ExpressionNode : Node
    {
        public Context Context { get; set; } = null;

        public virtual ExpressionNode Clone()
        {
            return this;
        }

        public override string ToString()
        {
            return "null";
        }

        public virtual bool DeepEquals(ExpressionNode b, long visitId)
        {
            return this == b;
        }

        public override string ToCodeString(int depth) { return "#%@#^INVALID WTF#(%!?!?#%&@"; }
    }
}
