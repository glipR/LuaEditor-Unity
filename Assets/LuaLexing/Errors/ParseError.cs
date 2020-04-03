using System;

namespace LuaParser
{
    public class ParseError : SyntaxError
    {
        #region Constructors
        /// <summary>
        /// Create Parse Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="line">Line</param>
        public ParseError(String message, int line)
            : base(message, line)
        {
            _type = "ParseError";
        }
        #endregion
    }
}
