namespace FBL.Parsing.Nodes
{
    public class StringNode : ExpressionNode
    {
        public string StringValue { get; set; }

        public StringNode(string value)
        {
            StringValue = value;
            Value = this;
        }

        public override string ToString()
        {
            return $"{StringValue}";
        }

        public override ExpressionNode Clone()
        {
            return new StringNode(StringValue);
        }
    }
}
