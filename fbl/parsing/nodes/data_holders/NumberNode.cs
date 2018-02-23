using System.Globalization;

namespace FBL.Parsing.Nodes
{
    public class NumberNode : ExpressionNode
    {
        public string NumericValue { get; private set; }

        public bool IsDecimal { get; private set; }


        public NumberNode(string value, bool isDecimal)
        {
            NumericValue = value;
            IsDecimal = isDecimal;

            Value = this;
        }

        public NumberNode(decimal value)
        {
            NumericValue = value.ToString(CultureInfo.InvariantCulture);
            IsDecimal = true;

            Value = this;
        }

        public override string ToString()
        {
            return $"{NumericValue}";
        }

        public override ExpressionNode Clone()
        {
            return new NumberNode(NumericValue, IsDecimal);
        }
    }
}
