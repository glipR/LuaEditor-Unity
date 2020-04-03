using System;

namespace LuaParser
{
    public struct ParserResult
    {
        #region Fields
        private SyntaxError[] _errors;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the errors
        /// </summary>
        public SyntaxError[] Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new parse result
        /// </summary>
        /// <param name="errors">Errors</param>
        public ParserResult(SyntaxError[] errors)
        {
            _errors = errors;
        }
        #endregion
    }
}
