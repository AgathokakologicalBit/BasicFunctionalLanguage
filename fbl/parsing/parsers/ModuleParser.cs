using FBL.Parsing.Nodes;

namespace FBL.Parsing
{
    public static class ModuleParser
    {
        public static CoreNode Parse(Parser.State state)
        {
            var node = new CoreNode();
            Node value;
            while ((value = ExpressionParser.Parse(state)) != null && !state.IsErrorOccured())
            {
                node.code.Add(value);
            }
            return node;
        }

    }
}