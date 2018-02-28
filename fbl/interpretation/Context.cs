using FBL.Interpretation.Modules;
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

        public Context()
        {
            this.Values = new Dictionary<string, ExpressionNode>()
            {
                { "set", new FunctionNode((i, c) => Set(i, c), false, false)
                    { Parameter = new VariableNode("name"), Context = this } },
                { "get", new FunctionNode((i, c) => Get(i, c), false, false)
                    { Parameter = new VariableNode("name"), Context = this } },
            };
        }

        public Context(Context context) : this()
        {
            this.Parent = context;
        }

        public Context Clone()
            => new Context(Parent)
            {
                Values = new Dictionary<string, ExpressionNode>(Values),
                IsDeterministic = IsDeterministic
            };


        public ExpressionNode SetVariable(string name, ExpressionNode value)
        {
            var ctx = this;
            while (ctx != null)
            {
                if (ctx.Values.ContainsKey(name))
                    return ctx.Values[name] = value;

                ctx = ctx.Parent;
            }

            Values.Add(name, value);
            return value;
        }

        public ExpressionNode GetVariable(string name)
        {
            var ctx = this;
            while (ctx != null)
            {
                if (ctx.Values.TryGetValue(name, out ExpressionNode value))
                    return value;

                ctx = ctx.Parent;
            }

            return new ExpressionNode();
        }

        public ExpressionNode Get(ExpressionNode input, Context context)
            => context.GetVariable(LanguageModule.ToString(input, context).StringValue);

        public FunctionNode Set(ExpressionNode input, Context context)
            => new FunctionNode(
                (v, c) => c.SetVariable(LanguageModule.ToString(input, c).StringValue, v),
                false, false
            )
            { Parameter = new VariableNode("right"), Context = context };


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
