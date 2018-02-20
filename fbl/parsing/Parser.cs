using FBL.Parsing.Nodes;
using FBL.Tokenization;
using System.Collections.Generic;

namespace FBL.Parsing
{
    public static class Parser
    {
        public class State : FBL.State
        {
            private int _index = -1;
            private Stack<State> _stateSaves = new Stack<State>(2);
            private List<FunctionCallNode> _attributes = new List<FunctionCallNode>();

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }


            public State(Token[] tokens)
            {
                Tokens = new List<Token>(tokens);
            }

            public State(State state)
            {
                Restore(state);
            }


            /// <summary>
            /// Gets next available token or null
            /// </summary>
            /// <returns>Next token</returns>
            public Token GetNextToken()
            {
                if (_index >= Tokens.Count) { return null; }

                _index += 1;
                return GetToken();
            }

            /// <summary>
            /// Gets current token or null
            /// Then moves to next token
            /// </summary>
            /// <returns>Current token</returns>
            public Token GetTokenAndMove()
            {
                var tok = GetToken();
                if (tok == null) { return null; }

                _index += 1;
                return tok;
            }

            /// <summary>
            /// Gets current token or null
            /// </summary>
            /// <returns>Current token</returns>
            public Token GetToken()
            {
                if (_index >= Tokens.Count) { return null; }
                if (_index < 0) { _index = 0; }

                return Tokens[_index];
            }

            /// <summary>
            /// Gets next valuable token or null
            /// valuable - not empty/commentary
            /// </summary>
            /// <returns>Next valuable token</returns>
            public Token GetNextNeToken()
            {
                Token t;

                do
                {
                    t = GetNextToken();
                } while (t?.Type == TokenType.Commentary);

                return t;
            }

            /// <summary>
            /// Gets current token or null
            /// Then moves to the next valuable token
            /// valuable - not empty/commentary
            /// </summary>
            /// <returns>Current token</returns>
            public Token GetTokenAndMoveNe()
            {
                var tok = GetToken();
                GetNextNeToken();
                return tok;
            }

            /// <summary>
            /// Saves state's copy to stack
            /// </summary>
            public override void Save() => _stateSaves.Push(new State(this));
            /// <summary>
            /// Restores state's copy from stack
            /// </summary>
            public override void Restore() => Restore(_stateSaves.Pop());
            /// <summary>
            /// Removes state's copy from stack without restoring values
            /// </summary>
            public override void Drop() => _stateSaves.Pop();

            /// <summary>
            /// Restores state's copy from given instance
            /// </summary>
            /// <param name="state">Target state</param>
            public void Restore(State state)
            {
                _index = state._index;
                Tokens = state.Tokens;
                _stateSaves = state._stateSaves;
            }

            public int GetIndex()
            {
                return _index;
            }

            internal void PushdAttribute(FunctionCallNode call)
            {
                _attributes.Add(call);
            }

            internal void ClearAttributes()
            {
                _attributes.Clear();
            }

            internal List<FunctionCallNode> GetAttributes()
            {
                return _attributes;
            }
        }

        /// <summary>
        /// Performs parsing of whole module (file)
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>Parsed AST</returns>
        public static CoreNode Parse(Token[] tokens)
        {
            var state = new State(tokens);
            var result = ModuleParser.Parse(state);

            if (!state.IsErrorOccured() && !state.GetToken().Is(TokenType.EOF))
            {
                state.ErrorCode = (uint)ErrorCodes.T_UnexpectedEndOfFile;
                state.ErrorMessage = "Parsing was stopped not at the end of the file";
            }

            if (!state.IsErrorOccured())
                return result;
            
            ReportError(state);
            return null;
        }

        private static void ReportError(State state)
        {
            ErrorHandler.LogErrorInfo(state);
            ErrorHandler.LogErrorPosition(state.GetToken());
            ErrorHandler.LogErrorTokensPart(state, state.GetIndex(), 3, 2);
        }
    }
}
