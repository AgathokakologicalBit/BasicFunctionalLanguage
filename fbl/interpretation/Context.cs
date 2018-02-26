using FBL.Parsing.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace FBL.Interpretation
{
    public class Context
    {
        public Context Parent;
        public Dictionary<string, ExpressionNode> Values;
        public long LastVisitedAt = -1;

        public bool IsDeterministic = true;


        public Context(Context context)
        {
            this.Parent = context;
            this.Values = new Dictionary<string, ExpressionNode>();
        }

        public override string ToString()
        {
            return string.Join("\n", Values.Select(v => $" >> {v.Key}\n<< {v.Value}\n===---   PARENT   ---===\n{Parent}"));
        }

        public bool DeepEquals(Context c, long visitId)
        {
            LastVisitedAt = visitId;

            if (this == c)
                return true;

            if (Values.Count != c.Values.Count)
                return false;

            return Values.All(v => c.Values.ContainsKey(v.Key) && v.Value.DeepEquals(c.Values[v.Key], visitId));
        }
    }
}
