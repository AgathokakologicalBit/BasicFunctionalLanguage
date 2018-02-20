using System.Linq;

namespace FBL.Tokenization.TokenParsing.ParsingModules
{
    public static class StringParser
    {
        public static Token Parse(Tokenizer.State state)
        {
            if (state.CurrentCharacter != '"') { return null; }

            var begin = state.Index + 1;

            do
            {
                state.Index += 1;
                while (state.CurrentCharacter == '\\')
                {
                    state.Index += 2;
                }
            } while (!"\"\0".Contains(state.CurrentCharacter));

            if (state.CurrentCharacter == '\0')
            {
                state.Index -= 1;
                state.ErrorCode = (uint)ErrorCodes.T_UnexpectedEndOfFile;
                state.ErrorMessage = "End of string was not found";
            }

            return new Token(
                value: state.Code.Substring(begin, state.Index++ - begin),

                type: TokenType.String,
                subType: TokenSubType.String
            );
        }
    }
}
