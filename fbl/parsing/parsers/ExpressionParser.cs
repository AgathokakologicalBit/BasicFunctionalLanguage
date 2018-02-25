using FBL.Parsing.Nodes;
using FBL.Tokenization;

namespace FBL.Parsing
{
    public static class ExpressionParser
    {
        public static ExpressionNode Parse(Parser.State state)
        {
            if (state.IsErrorOccured() || state.GetToken().Is(TokenType.EOF))
                return null;

            var block = new BlockNode();

            do
            {
                while (state.GetToken().Is(TokenSubType.Colon))
                    state.GetNextNeToken();


                var unit = ParseUnit(state);
                FunctionCallNode fc_unit;

                if (unit == null || state.IsErrorOccured()) break;

                while (!state.IsErrorOccured()
                    && !state.GetToken().Is(TokenType.EOF)
                    && (fc_unit = ParseFunctionCall(state, unit)) != null)
                {
                    unit = fc_unit;
                }

                block.Code.Add(unit);
            } while (state.GetToken().Is(TokenSubType.Colon));

            return block;
        }

        private static ExpressionNode ParseUnit(Parser.State state)
        {
            if (state.GetToken().Is(TokenSubType.BraceSquareLeft))
            {
                state.GetNextNeToken();
                var func = new FunctionNode();

                FunctionParser.ParseParametersList(state, func);
                if (state.IsErrorOccured())
                    return null;

                func.Code = Parse(state);
                if (state.IsErrorOccured())
                    return null;

                return func;
            }

            return ParseValue(state);
        }

        private static ExpressionNode ParseValue(Parser.State state)
        {
            state.Save();
            var token = state.GetTokenAndMoveNe();

            if (token.Is(TokenType.Number))
            {
                state.Drop();
                return new NumberNode(
                    value: token.Value
                );
            }
            else if (token.Is(TokenType.Identifier))
            {
                state.Drop();
                return new VariableNode(token.Value);
            }
            else if (token.Is(TokenType.String))
            {
                state.Drop();
                return new StringNode(token.Value, true);
            }
            else if (token.Is(TokenSubType.BraceRoundLeft))
            {
                state.Drop();
                var node = Parse(state);
                if (state.IsErrorOccured())
                    return node;

                if (!state.GetToken().Is(TokenSubType.BraceRoundRight))
                {
                    state.ErrorCode = (uint)ErrorCodes.P_ClosingBraceRequired;
                    state.ErrorMessage =
                        "Expected <BraceRoundRight>, " +
                        $"but <{state.GetToken().SubType}> was given";
                    return node;
                }

                state.GetNextNeToken();
                return node;
            }
            state.Restore();

            return null;
        }

        internal static FunctionCallNode ParseFunctionCall(Parser.State state, ExpressionNode func)
        {
            var call = new FunctionCallNode
            {
                CalleeExpression = func
            };

            ExpressionNode arg = ParseUnit(state);
            if (state.IsErrorOccured() || arg == null)
                return null;

            call.Argument = arg;
            return call;
        }
    }
}
