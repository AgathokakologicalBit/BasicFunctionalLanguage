namespace FBL.Tokenization.TokenParsing.ParsingModules
{
    public static class OperatorParser
    {
        public static Token Parse(Tokenizer.State state)
        {
            var st = GetTypeFor(state.CurrentCharacter);

            if (st == TokenSubType.Unknown) { return null; }

            var t = new Token(
                value: state.CurrentCharacter.ToString(),

                type: TokenType.Operator,
                subType: st
            );

            state.Index += 1;
            return t;
        }

        private static TokenSubType GetTypeFor(char c)
        {
            switch (c)
            {
                case ':': return TokenSubType.Colon;
                case ';': return TokenSubType.SemiColon;


                case '(': return TokenSubType.BraceRoundLeft;
                case ')': return TokenSubType.BraceRoundRight;
                
                case '[': return TokenSubType.BraceSquareLeft;
                case ']': return TokenSubType.BraceSquareRight;

                case '{': return TokenSubType.BraceCurlyLeft;
                case '}': return TokenSubType.BraceCurlyRight;


                default: return TokenSubType.Unknown;
            }
        }
    }
}
