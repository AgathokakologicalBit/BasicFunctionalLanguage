using System;
using System.Collections.Generic;
using System.Linq;

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

        public override string ToCodeString(int depth)
        {
            return $"\n{new String(' ', depth * 2)}: " + string.Join(
                $"\n{new String(' ', depth * 2)}: ",
                Code.Select(line => line.ToCodeString(depth + 1)));
        }
    }
}
