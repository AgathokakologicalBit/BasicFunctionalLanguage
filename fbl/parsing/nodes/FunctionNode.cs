using FBL.Interpretation;
using System;

namespace FBL.Parsing.Nodes
{
    public class FunctionNode : ExpressionNode
    {
        /// <summary>
        /// List of named parameters
        /// </summary>
        public VariableNode Parameter { get; set; }

        /// <summary>
        /// Block of code or MathExpression
        /// </summary>
        public ExpressionNode Code { get; set; }

        public Func<ExpressionNode, Context, ExpressionNode> Function { get; set; }

        public bool CheckedPure { get; set; } = false;
        public bool IsPureIn { get; set; } = false;
        public bool IsPureOut { get; set; } = false;


        public FunctionNode() { }

        public FunctionNode(Func<ExpressionNode, Context, ExpressionNode> import, bool isPureIn, bool isPureOut)
        {
            this.Function = import;
            this.CheckedPure = true;
            this.IsPureIn = isPureIn;
            this.IsPureOut = isPureOut;
        }

        /// <summary>
        /// Generates string representation of function.
        /// Used for debug/logging purposes
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"(def [ {Parameter?.Name ?? "ø"} ] {String.Join(" : ", Code ?? new StringNode("ø", false))} )";
        }


        public override ExpressionNode Clone()
        {
            return new FunctionNode
            {
                Parameter = Parameter,
                Code = Code,
                Function = Function,
                Context = Context
            };
        }

        public override bool DeepEquals(ExpressionNode b, long visitId)
        {
            if (this == b) return true;

            if (b is FunctionNode bf)
            {
                return Code == bf.Code && Context.DeepEquals(bf.Context, visitId);
            }

            return false;
        }

        public override string ToCodeString(int depth)
        {
            return $"([ {Parameter.Name} ] {Code.ToCodeString(depth + 1)})";
        }
    }
}
