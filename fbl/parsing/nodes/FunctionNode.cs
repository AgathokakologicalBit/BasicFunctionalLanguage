using FBL.Interpretation;
using System;

namespace FBL.Parsing.Nodes
{
    public class FunctionNode : ExpressionNode
    {
        /// <summary>
        /// Parent Class/Function
        /// </summary>
        public Node Parent { get; set; }
        
        /// <summary>
        /// List of named parameters
        /// </summary>
        public VariableNode Parameter { get; set; }

        /// <summary>
        /// Block of code or MathExpression
        /// </summary>
        public ExpressionNode Code { get; set; }


        public bool IsNative { get; set; }
        public Func<ExpressionNode, ExpressionNode> Function { get; set; }
        

        public FunctionNode(Context context)
        {
            this.IsNative = false;
            this.Value = this;
            this.Context = new Context(context);
        }

        public FunctionNode(Func<ExpressionNode, ExpressionNode> import, Context context)
        {
            this.IsNative = true;
            this.Function = import;
            this.Context = new Context(context);
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
            return new FunctionNode (Context)
            {
                Parent = Parent,
                Parameter = Parameter,
                Code = Code?.Clone(),
                IsNative = IsNative,
                Function = Function,
            };
        }
    }
}
