namespace FBL.Parsing.Nodes
{
    public class VariableNode : ExpressionNode
    {
        /// <summary>
        /// Variable name. Might be represented as path on access(separated by colon(:))
        /// </summary>
        public string Name { get; set; }


        public VariableNode(string name)
        {
            Name = name;
        }


        /// <summary>
        /// Generates string representation of variable.
        /// Used for debug/logging purposes
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return Name;
        }

        public override string ToCodeString(int depth)
        {
            return Name;
        }
    }
}
