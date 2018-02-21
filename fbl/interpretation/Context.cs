using FBL.Parsing.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace FBL.Interpretation
{
    public class Context
    {
        public Context Parent;
        public Dictionary<string, ExpressionNode> Values;

        public Context(Context context)
        {
            this.Parent = context;
            this.Values = new Dictionary<string, ExpressionNode>();
        }

        public override string ToString()
        {
            return string.Join("\n", Values.Select(v => $" >> {v.Key}\n<< {v.Value}"));
        }
    }
}
