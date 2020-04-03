using System;

namespace LuaParser
{
    public struct Token
    {
        #region Fields
        private String _type;
        private String _val;
        private TokenLocation _loc;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the type
        /// </summary>
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Get/set the type
        /// </summary>
        public String Value
        {
            get { return _val; }
            set { _val = value; }
        }

        /// <summary>
        /// Get/set the token location
        /// </summary>
        public TokenLocation Location
        {
            get { return _loc; }
            set { _loc = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new token
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        /// <param name="location">Location Data</param>
        public Token(string type, string value, TokenLocation location)
        {
            _type = type;
            _val = value;
            _loc = location;
        }
        #endregion
    }    
}
