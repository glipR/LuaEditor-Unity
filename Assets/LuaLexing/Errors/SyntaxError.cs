using System;

namespace LuaParser
{
    public class SyntaxError
    {
        #region Fields
        private String _message;
        protected String _type;
        private int _line;
        #endregion
        
        #region Properties
        /// <summary>
        /// Get/set the error message
        /// </summary>
        public String Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Get/set the error line
        /// </summary>
        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        /// <summary>
        /// Get the error type
        /// </summary>
        public String Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new syntax error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="line">Line</param>
        public SyntaxError(String message, int line)
        {
            _message = message;
            _line = line;
            _type = "SyntaxError";
        }
        #endregion
    }
}
