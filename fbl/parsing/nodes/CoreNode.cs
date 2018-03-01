namespace FBL.Parsing.Nodes
{
    public class CoreNode : Node
    {
        public ExpressionNode Code = null;

        public override string ToString()
        {
            return Code.ToString();
        }

        public override string ToCodeString(int depth)
        {
            return Code.ToCodeString(0);
        }
    }
}
