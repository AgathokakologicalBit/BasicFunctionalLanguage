namespace FBL.Tokenization
{
    /// <summary>
    /// Base token type (11 values excluding Unknown)
    /// </summary>
    public enum TokenType
    {
        Unknown,

        Commentary,

        Identifier,

        Whitespace,

        Number,
        String,

        Operator,

        EOF
    }

    /// <summary>
    /// Extended token type (38 values excluding Unknown)
    /// </summary>
    public enum TokenSubType
    {
        Unknown,

        InlineCommentary,

        Identifier,

        Space, NewLine,

        Integer, Decimal,

        String,

        BraceRoundLeft, BraceRoundRight,
        BraceSquareLeft, BraceSquareRight,
        BraceCurlyLeft, BraceCurlyRight,

        Colon, SemiColon,

        EndOfFile
    }
}
