using FBL.Parsing.Nodes;

namespace FBL.Parsing
{
    public static class ModuleParser
    {
        public static CoreNode Parse(Parser.State state)
        {
            var node = new CoreNode();
            Node value;

            int lastIndex = state.Index;
            while ((value = ExpressionParser.Parse(state)) != null && !state.IsErrorOccured())
            {
                if (lastIndex == state.Index)
                    break;

                lastIndex = state.Index;
                node.code.Add(value);
            }

            return node;
        }

    }
}