using System;
using System.Threading;

namespace LuaParser
{
    public class Parser
    {
        #region Fields
        private Thread _thread;
        private Action<ParserResult> _callback;
        private bool _threadRunning = false;
        private bool _asyncTokenParse = false;
        private Object _asyncData = null;
        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// Parse the tokens synchronously
        /// </summary>
        /// <returns>Result</returns>
        public ParserResult Parse(LexerResult data)
        {
            return new ParserResult(new ParseError[] { });
        }

        /// <summary>
        /// Parse the Lua synchronously
        /// </summary>
        /// <param name="lua">Lua</param>
        /// <returns>Result</returns>
        public ParserResult Parse(String lua)
        {
            return Parse(new Lexer(lua).Tokenize());
        }

        /// <summary>
        /// Parse the tokens asynchronously
        /// </summary>
        /// <param name="data">Lexer Data</param>
        /// <param name="callback">Asynchronous Callback</param>
        public void ParseAsync(LexerResult data, Action<ParserResult> callback)
        {
            if (_threadRunning)
            { _thread.Abort(); }

            _asyncTokenParse = true;
            _asyncData = callback;
            _asyncData = data;
            _threadRunning = true;
            _callback = callback;
            _thread.Name = "Lua Parser";
            _thread.Start();
        }

        /// <summary>
        /// Parse the Lua asynchronously
        /// </summary>
        /// <param name="data">Lua Data</param>
        /// <param name="callback">Asynchronous Callback</param>
        public void ParseLuaAsync(String data, Action<ParserResult> callback)
        {
            if (_threadRunning)
            { _thread.Abort(); }

            _asyncTokenParse = false;
            _asyncData = data;
            _threadRunning = true;
            _callback = callback;
            _thread.Name = "Lua Parser";
            _thread.Start();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new lexer
        /// </summary>
        /// <param name="lua">Lua</param>
        public Parser()
        {
            _thread = new Thread(new ThreadStart(delegate()
            {
                if (_asyncTokenParse == true)
                {
                    _callback.Invoke(Parse((LexerResult)_asyncData));
                }
                else { _callback.Invoke(Parse((String)_asyncData)); }
               
                _threadRunning = false;
            }));
        }
        #endregion
    }
}
