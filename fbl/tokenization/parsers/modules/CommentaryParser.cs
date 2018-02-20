using System.Linq;

namespace FBL.Tokenization.TokenParsing.ParsingModules
{
    public static class CommentaryParser
    {
        public static Token Parse(Tokenizer.State state)
        {
            if (state.CurrentCharacter == '`')
            {
                var begin = state.Index;
                state.Index += 1;

                ParseInlineCommentary(state);

                if (state.IsErrorOccured())
                    state.Index = begin + 2;

                return new Token(
                    value: state.Code.Substring(begin, state.Index - begin),

                    type: TokenType.Commentary,
                    subType: TokenSubType.InlineCommentary
                );
            }

            return null;
        }

        private static void ParseInlineCommentary(Tokenizer.State state)
        {
            while (state.CurrentCharacter != '\0' && !"\r\n".Contains(state.CurrentCharacter))
                state.Index += 1;
        }
    }
}
