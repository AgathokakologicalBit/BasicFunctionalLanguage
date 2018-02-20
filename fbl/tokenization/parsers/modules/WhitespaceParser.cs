using System;

namespace FBL.Tokenization.TokenParsing.ParsingModules
{
    public static class WhitespaceParser
    {
        public static Token Parse(Tokenizer.State state)
        {
            if (!Char.IsWhiteSpace(state.CurrentCharacter)) { return null; }

            var c = state.CurrentCharacter;
            var isNewLine = c == '\n';
            if (isNewLine)
            {
                state.Line += 1;
                state.LineBegin = state.Index + 1;
            }

            var token = new Token(
                value: c.ToString(),
                
                type: TokenType.Whitespace,
                subType: isNewLine
                    ? TokenSubType.NewLine
                    : TokenSubType.Space
            );

            state.Index += 1;
            return token;
        }
    }
}
