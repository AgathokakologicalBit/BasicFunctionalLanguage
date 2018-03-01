using FBL.Interpretation;
using FBL.Parsing.Nodes;
using FBL.Tokenization;

namespace FBL.Parsing
{
    public static class ExpressionParser
    {
        public static ExpressionNode Parse(Parser.State state, Context context)
        {
            if (state.IsErrorOccured() || state.GetToken().Is(TokenType.EOF))
                return null;

            var block = new BlockNode();

            do
            {
                while (state.GetToken().Is(TokenSubType.Colon))
                    state.GetNextNeToken();


                var unit = ParseUnit(state, context);
                FunctionCallNode fc_unit;

                if (unit == null || state.IsErrorOccured()) break;

                while (!state.IsErrorOccured()
                    && !state.GetToken().Is(TokenType.EOF)
                    && (fc_unit = ParseFunctionCall(state, unit, context)) != null)
                {
                    unit = fc_unit;
                }

                block.Code.Add(unit);
            } while (state.GetToken().Is(TokenSubType.Colon));

            return block;
        }

        private static ExpressionNode ParseUnit(Parser.State state, Context context)
        {
            if (state.GetToken().Is(TokenSubType.BraceSquareLeft))
            {
                state.GetNextNeToken();
                var func = new FunctionNode();

                FunctionParser.ParseParametersList(state, func);
                if (state.IsErrorOccured())
                    return null;

                var subContext = new Context(context, context.Values);
                func.Code = Parse(state, subContext);
                func.Context = subContext;

                if (state.IsErrorOccured())
                    return null;

                return func;
            }

            return ParseValue(state, context);
        }

        private static ExpressionNode ParseValue(Parser.State state, Context context)
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
                var node = Parse(state, context);
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

        internal static FunctionCallNode ParseFunctionCall(Parser.State state, ExpressionNode func, Context context)
        {
            var call = new FunctionCallNode
            {
                CalleeExpression = func
            };

            ExpressionNode arg = ParseUnit(state, context);
            if (state.IsErrorOccured() || arg == null)
                return null;

            call.Argument = arg;
            return call;
        }
    }
}
