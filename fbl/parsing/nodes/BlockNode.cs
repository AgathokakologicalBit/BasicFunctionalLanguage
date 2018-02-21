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
            Value = this;
        }

        public override string ToString()
        {
            return string.Join("\n", Code);
        }

        public override ExpressionNode Clone()
        {
            var new_block = new BlockNode() { Context = Context };

            foreach (var c in Code)
                new_block.Code.Add(c.Clone());

            return new_block;
        }
    }
}
