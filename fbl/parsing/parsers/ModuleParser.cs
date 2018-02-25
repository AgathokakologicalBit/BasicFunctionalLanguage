using FBL.Parsing.Nodes;
using System;

namespace FBL.Parsing
{
    public static class ModuleParser
    {
        public static CoreNode Parse(Parser.State state)
        {
            var node = new CoreNode();
            ExpressionNode value;

            int lastIndex = state.Index;
            while ((value = ExpressionParser.Parse(state)) != null && !state.IsErrorOccured())
            {
                if (lastIndex == state.Index)
                    break;

                lastIndex = state.Index;
                node.Code = value;

                if (state.Index + 1 < state.Tokens.Count && !state.IsErrorOccured())
                {
                    Console.WriteLine($"{state.Index} / {state.Tokens.Count}");
                    state.ErrorCode = (uint)ErrorCodes.T_UnexpectedEndOfFile;
                    state.ErrorMessage = "Something really wrong happend";
                }
            }

            return node;
        }

    }
}