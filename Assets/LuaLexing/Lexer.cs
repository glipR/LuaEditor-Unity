using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace LuaParser
{
    public class Lexer : IDisposable
    {
        #region Fields
        private String _lua;
        private Thread _thread;
        private Action<LexerResult> _callback;
        private bool _threadRunning = false;
        private static Regex endOfLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
        #endregion

        #region Properties
        /// <summary>
        /// Get the Lua in the lexer
        /// </summary>
        public String Lua
        {
            get { return _lua; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tokenize the Lua
        /// </summary>
        /// <returns>Lexer Data</returns>
        public LexerResult Tokenize()
        {
            int curPos = 0;
            int curLine = 1;
            int curCol = 0;
            List<Token> tokens = new List<Token>();

            // rest of the code
            for (long i = 0; i < 1000000; i++)
            {
                while (curPos < _lua.Length)
                {
                    LexerRule match = null;
                    int matchLen = 0;
                    string matchVal = "";

                    foreach (LexerRule rule in LexerRules.Rules)
                    {
                        Match m = rule.Rule.Match(_lua.Substring(curPos));
                        if (m.Success)
                        {
                            matchLen = m.Length;
                            match = rule;
                            matchVal = m.Value;
                            break;
                        }
                    }

                    if (match == null)
                    {
                        curPos = curPos + 1;
                    }
                    else
                    {
						string value = _lua.Substring(curPos, matchLen);
						Match eolMatch = endOfLineRegex.Match(value);
						if (eolMatch.Success) {
							curLine = curLine + 1;
						} else {
							curCol = 0;
						}

                        tokens.Add(new Token(match.Type, matchVal, new TokenLocation(
                            curCol,
                            curPos,
                            curLine
                        )));

                        curPos = curPos + matchLen;
                    }
                }
            }

            return new LexerResult(tokens.ToArray(), new LexerError[] { });
        }

        /// <summary>
        /// Tokenize the Lua
        /// </summary>
        /// <param name="callback">Callback</param>
        public void TokenizeAsync(Action<LexerResult> callback)
        {
            if (_threadRunning)
            { _thread.Abort(); }

            _threadRunning = true;
            _callback = callback;
            _thread.Name = "Lua Lexer";
            _thread.Start();
        }

        /// <summary>
        /// Destroy the lexer
        /// </summary>
        public void Dispose()
        {
            if (_threadRunning)
            { _thread.Abort(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new lexer
        /// </summary>
        /// <param name="lua">Lua</param>
        public Lexer(String lua)
        {
            _lua = lua;

            _thread = new Thread(new ThreadStart(delegate()
            {
                _callback.Invoke(Tokenize());
                _threadRunning = false;
            }));
        }
        #endregion
    }
}
