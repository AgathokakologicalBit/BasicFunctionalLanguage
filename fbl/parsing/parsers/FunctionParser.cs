using FBL.Parsing.Nodes;
using FBL.Tokenization;

namespace FBL.Parsing
{
    public static class FunctionParser
    {
        public static void ParseParametersList(Parser.State state, FunctionNode function)
        {
            if (state.GetToken().Is(TokenType.EOF))
            {
                state.ErrorCode = (uint)ErrorCodes.T_UnexpectedEndOfFile;
                state.ErrorMessage = "Required <Identifier> as a function parameter";
                return;
            }

            state.Save();
            var token = state.GetTokenAndMoveNe();
            if (!token.Is(TokenType.Identifier))
            {
                state.Restore();
                state.ErrorCode = (uint)ErrorCodes.P_IdentifierExpected;
                state.ErrorMessage = "Required <Identifier> as a function parameter";
                return;
            }
            state.Drop();

            function.Parameter = new VariableNode(token.Value);

            if (!state.GetToken().Is(TokenSubType.BraceSquareRight))
            {
                state.ErrorCode = (uint)ErrorCodes.P_ClosingBraceRequired;
                state.ErrorMessage = "Required <BraceSquareRight> at the end of the function parameters list";
                return;
            }

            state.GetNextNeToken();
        }
    }
}
