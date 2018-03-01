using System.Globalization;

namespace FBL.Parsing.Nodes
{
    public class NumberNode : ExpressionNode
    {
        public decimal NumericValue { get; private set; }


        public NumberNode(string value)
        {
            NumericValue = decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        public NumberNode(decimal value)
        {
            NumericValue = value;
        }

        public override string ToString()
        {
            return NumericValue.ToString();
        }

        public override bool DeepEquals(ExpressionNode b, long visitId)
        {
            if (this == b) return true;
            if (b is NumberNode bn)
                return NumericValue == bn.NumericValue;

            return false;
        }

        public override string ToCodeString(int depth)
        {
            return NumericValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
