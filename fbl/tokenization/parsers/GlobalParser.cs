using FBL.Tokenization.TokenParsing.ParsingModules;
using System;
using System.Collections.Generic;

namespace FBL.Tokenization.TokenParsing
{
    public static class GlobalParser
    {
        private static readonly List<Func<Tokenizer.State, Token>> parsers
            = new List<Func<Tokenizer.State, Token>> {
                WhitespaceParser.Parse,
                CommentaryParser.Parse,

                OperatorParser.Parse,

                NumberParser.Parse,
                IdentifierParser.Parse,
                
                StringParser.Parse,
            };

        public static Token Parse(Tokenizer.State state)
        {
            foreach (var parser in parsers)
            {
                Token token;
                if (TryParse(state, parser, out token))
                {
                    return token;
                }
            }

            return null;
        }

        private static bool TryParse
            (Tokenizer.State state, Func<Tokenizer.State, Token> parser, out Token token)
        {
            state.Save();
            var stateCopy = new Tokenizer.State(state);

            token = parser(state);

            if (state.IsErrorOccured())
            {
                state.Drop();
                return true;
            }
            else if (token != null)
            {
                token.Index = stateCopy.Index;
                token.Line = stateCopy.Line;
                token.Position = stateCopy.Position;

                state.Drop();
                return true;
            }

            state.Restore();
            return false;
        }
    }
}
