using FBL.Tokenization;
using System.Collections.Generic;

namespace FBL
{
    /// <summary>
    /// Abstract Compiler state
    /// Holds stack of it's own copies
    /// Holds errors
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Code that is being parsed
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// List of parsed tokens
        /// </summary>
        public List<Token> Tokens { get; set; }

        public uint ErrorCode { get; set; }
        public string ErrorMessage { get; set; }


        /// <summary>
        /// Checks if any errors were written to state
        /// </summary>
        /// <returns>True if error is occured</returns>
        public bool IsErrorOccured() => ErrorCode > 0;

        /// <summary>
        /// Saves state's copy to stack
        /// </summary>
        public abstract void Save();
        /// <summary>
        /// Restores state's copy from stack
        /// </summary>
        public abstract void Restore();
        /// <summary>
        /// Removes state's copy from stack without restoring values
        /// </summary>
        public abstract void Drop();
    }
}
