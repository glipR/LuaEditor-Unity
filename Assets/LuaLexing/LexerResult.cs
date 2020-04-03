using System;

namespace LuaParser
{
    public struct LexerResult
    {
        #region Fields
        private Token[] _tokens;
        private LexerError[] _errors;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the tokens
        /// </summary>
        public Token[] Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }

        /// <summary>
        /// Get/set the errors
        /// </summary>
        public LexerError[] Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new lexer result
        /// </summary>
        /// <param name="tokens">Tokens</param>
        /// <param name="errors">Errors</param>
        public LexerResult(Token[] tokens, LexerError[] errors)
        {
            _tokens = tokens;
            _errors = errors;
        }
        #endregion
    }
}
