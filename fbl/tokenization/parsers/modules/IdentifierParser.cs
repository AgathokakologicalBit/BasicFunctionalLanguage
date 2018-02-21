using System;
using System.Linq;

namespace FBL.Tokenization.TokenParsing.ParsingModules
{
    public static class IdentifierParser
    {
        public static Token Parse(Tokenizer.State state)
        {
            if (!IsMatching(state.CurrentCharacter, state)) { return null; }

            var begin = state.Index;
            state.Index += 1;
            while (IsMatching(state.CurrentCharacter, state))
            {
                state.Index += 1;
            }
            
            return new Token(
                value: state.Code.Substring(begin, state.Index - begin),
                
                type: TokenType.Identifier,
                subType: TokenSubType.Identifier
            );
        }

        private static bool IsMatching(char c, Tokenizer.State state)
        {
            const string possible = @"!?@#$%,^&|*/\_+~-<=>";
            if (Char.IsLetter(c) || possible.Contains(c))
            {
                if (c > 127)
                {
                    // If identifier contains non-ascii letters
                    // it will be written as error to state
                    state.ErrorCode = (uint)ErrorCodes.T_InvalidIdentifier;
                    state.ErrorMessage =
                        "Identifier can contains only:\n" +
                        "  - latin letters\n" +
                        $"  - '{possible}' symbols";
                }

                return true;
            }

            return false;
        }
    }
}
