namespace FBL.Tokenization
{
    public class Token
    {
        /// <summary>
        /// Context in wich token is placed
        /// </summary>
        public Tokenizer.State.Context Context { get; set; }

        /// <summary>
        /// Token's index in code string
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Token's line
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// Token's position on line
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// String representation of token
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Length of string representation of token
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Base token type
        /// </summary>
        public TokenType Type { get; set; }
        /// <summary>
        /// Extended token type
        /// </summary>
        public TokenSubType SubType { get; set; }

        public Token
        (
            string value,
            TokenType type, TokenSubType subType
        )
            : this(
                0, value, 0, 0,
                Tokenizer.State.Context.Global,
                type, subType
            )
        { }

        public Token
        (
            int index,
            string value,

            int line, int position,

            Tokenizer.State.Context context,
            TokenType type, TokenSubType subType
        )
        {
            Context = context;

            Index = index;
            Line = line;
            Position = position;

            Value = value;
            Length = value.Length;

            Type = type;
            SubType = subType;
        }

        /// <summary>
        /// Formats token for debug usage
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return  $"({Line.ToString().PadLeft(3, '0')}:" +
                    $"{Position.ToString().PadLeft(3, '0')}) " +
                    $"{SubType.ToString().PadRight(20, ' ')} " +
                    $"({Length.ToString().PadLeft(2, ' ')}) {Value}";
        }

        /// <summary>
        /// Checks if token corresponds to given type and matches given value(string representation)
        /// </summary>
        /// <param name="t">Target type</param>
        /// <param name="v">Target value(string representation)</param>
        /// <returns>True if token matches needed value</returns>
        public bool Is(TokenType t, string v) => Type == t && Value == v;

        /// <summary>
        /// Checks if token corresponds to given type
        /// </summary>
        /// <param name="t">Target type</param>
        /// <returns>True if token matches needed type</returns>
        public bool Is(TokenType t) => Type == t;

        /// <summary>
        /// Checks if token corresponds to given subtype and matches given value(string representation)
        /// </summary>
        /// <param name="st">Target extended type</param>
        /// <param name="v">Target value(string representation)</param>
        /// <returns>True if token matches needed value</returns>
        public bool Is(TokenSubType st, string v) => SubType == st && Value == v;

        /// <summary>
        /// Checks if token corresponds to given subtype
        /// </summary>
        /// <param name="st">Target extended type</param>
        /// <returns>True if token matches needed type</returns>
        public bool Is(TokenSubType st) => SubType == st;
    }
}
