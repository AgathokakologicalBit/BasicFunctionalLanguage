using System.Collections.Generic;

namespace FBL.Parsing.Nodes
{
    public class BlockNode : ExpressionNode
    {
        /// <summary>
        /// Holds list of expressions
        /// </summary>
        public List<ExpressionNode> Code { get; private set; }

        public BlockNode()
        {
            Code = new List<ExpressionNode>();
        }

        public override string ToString()
        {
            return string.Join("\n", Code);
        }
    }
}
