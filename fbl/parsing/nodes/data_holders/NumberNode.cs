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

        public override bool DeepEquals(ExpressionNode b, long visitId)
        {
            if (this == b) return true;
            if (b is NumberNode bn)
                return NumericValue == bn.NumericValue;

            return false;
        }
    }
}
