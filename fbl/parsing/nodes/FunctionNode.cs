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


        public FunctionNode()
        {
        }

        public FunctionNode(Func<ExpressionNode, Context, ExpressionNode> import)
        {
            this.Function = import;
        }

        /// <summary>
        /// Generates string representation of function.
        /// Used for debug/logging purposes
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"(def [ {Parameter?.Name ?? "ø"} ] {String.Join(" : ", Code ?? new StringNode("ø"))} )";
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
                return ToString() == bf.ToString()
                    && (Context?.LastVisitedAt == visitId
                    || Context == bf.Context
                    || (Context?.DeepEquals(bf.Context, visitId) ?? false));
            }

            return false;
        }
    }
}
