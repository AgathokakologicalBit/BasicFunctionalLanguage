using FBL.Parsing.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace FBL.Interpretation
{
    public class Context
    {
        public Context parent;
        public Dictionary<string, ExpressionNode> values;

        public Context(Context context)
        {
            this.parent = context;
            this.values = new Dictionary<string, ExpressionNode>();
        }

        public override string ToString()
        {
            return string.Join("\n", values.Select(v => $" >> {v.Key}\n<< {v.Value}"));
        }
    }
}
