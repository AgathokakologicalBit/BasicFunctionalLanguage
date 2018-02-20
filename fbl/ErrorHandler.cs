using FBL.Parsing;
using FBL.Tokenization;
using System;
using System.Linq;

namespace FBL
{
    public static class ErrorHandler
    {
        public static void LogError(State state)
        {
            LogErrorInfo(state);
            if (state is Parser.State) { state.Restore(); }

            var lastToken = GetLastTokenOrEof(state);
            LogErrorPosition(lastToken);

            var codePointerString = new String(
                '^',
                Math.Max(1, lastToken.Length)
            );

            LogCodePart(state, codePointerString, lastToken.Index);
        }

        public static void LogErrorInfo(State state)
        {
            Console.Error.WriteLine(
                $"Error #LC{state.ErrorCode.ToString("D3")}:\n{state.ErrorMessage}\n"
            );
        }

        public static void LogErrorPosition(Token token)
        {
            Console.Error.WriteLine(
                $"Line: {token.Line}\nPosition: {token.Position}\n"
            );
        }

        public static void LogErrorTokensPart(State state, int index, int leftBound, int rightBound)
        {
            var reader = state.Tokens.Skip(index - leftBound);
            var tokensToPrint = reader.Take(leftBound + rightBound + 1);

            Console.Error.WriteLine(
                String.Join(
                    " ",
                    tokensToPrint.Select(t => t.Value)
                )
            );

            var codePointerString = new String(
                '^',
                state.Tokens[index].Length
            );
            var shift = tokensToPrint.Take(leftBound).Select(t => t.Value.Length).Sum() + leftBound;
            var underline = new String(' ', shift) + codePointerString;
            Console.Error.WriteLine(underline);
        }

        private static void LogCodePart(State state, string underline, int from)
        {
            Console.Error.WriteLine(
                state.Code
                    .Substring(from, underline.Length)
                    .Replace('\n', ' ') +
                    "\n" + underline + "\n"
            );
        }

        private static Token GetLastTokenOrEof(State state)
        {
            return state.Tokens.LastOrDefault() ?? new Token(
                0, "", 1, 1,
                Tokenizer.State.Context.Global,
                TokenType.EOF, TokenSubType.EndOfFile
            );
        }
    }
}
