using System;

namespace LuaParser
{
    public class LexerError : SyntaxError
    {
        #region Constructors
        /// <summary>
        /// Create Lexer Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="line">Line</param>
        public LexerError(String message, int line)
            : base(message, line)
        {
            _type = "LexerError";
        }
        #endregion
    }
}
