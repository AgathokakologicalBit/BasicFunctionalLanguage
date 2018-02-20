using System.Collections.Generic;

namespace FBL.Parsing.Nodes
{
    public class CoreNode : Node
    {
        public List<Node> code = new List<Node>(100);

        public override string ToString()
        {
            return string.Join("\n\n", code);
        }
    }
}
