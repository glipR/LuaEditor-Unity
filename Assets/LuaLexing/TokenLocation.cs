using System;

namespace LuaParser
{
    public struct TokenLocation
    {
        #region Fields
        private int _col;
        private int _pos;
        private int _line;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the column
        /// </summary>
        public int Column
        {
            get { return _col; }
            set { _col = value; }
        }

        /// <summary>
        /// Get/set the position
        /// </summary>
        public int Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        /// <summary>
        /// Get/set the line
        /// </summary>
        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new token location
        /// </summary>
        /// <param name="col">Column</param>
        /// <param name="pos">Position</param>
        /// <param name="line">Line</param>
        public TokenLocation(int col, int pos, int line)
        {
            _col = col;
            _pos = pos;
            _line = line;
        }
        #endregion
    }
}
