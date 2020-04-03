using System;
using System.Text.RegularExpressions;

namespace LuaParser
{
    public class LexerRule
    {
        #region Fields
        private Regex _rule;
        private String _type;
        #endregion

        #region Properties
        /// <summary>
        /// Get the rule
        /// </summary>
        public Regex Rule
        {
            get { return _rule; }
        }

        /// <summary>
        /// Get the type
        /// </summary>
        public String Type
        {
            get { return _type; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a Lua rule
        /// </summary>
        /// <param name="rule">Regex</param>
        /// <param name="type">Type</param>
        public LexerRule(Regex rule, String type)
        {
            _rule = rule;
            _type = type;
        }
        #endregion
    }
}
