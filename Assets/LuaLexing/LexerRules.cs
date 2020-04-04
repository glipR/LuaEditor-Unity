using System;
using System.Text.RegularExpressions;

namespace LuaParser
{
    public class LexerRules
    {
        /// <summary>
        /// Keywords
        /// </summary>
        public static String[] Keywords = new String[] {
            "and", "break","do","else","elseif","end","false",
            "for","function","if","in","local","nil","not","or",
            "repeat","return","then","true","until","while"
        };

        /// <summary>
        /// Operators
        /// </summary>
        public static String[] Operators = new String[] {
            "\\+", "\\-", "\\*", "\\/", "\\%", "\\^", "\\#",
            "\\=\\=", "\\~\\=", "\\<\\=", "\\>\\=", "\\<", "\\>", "\\=",
            "\\(", "\\)", "\\{", "\\}", "\\[", "\\]", "\\;", "\\:",
            "\\,","\\.","\\.\\.","\\.\\.\\.","\\!\\="
        };

        /// <summary>
        /// Regex Stuff
        /// </summary>
        public static Regex[] Matches = new Regex[]{
            new Regex(@"^[\t\n\r \xA0]+"), //whitespace
            new Regex("^\'(((?<=\\\\)\')?([^\n\r\'])*)*\'"), // string '
            new Regex("^\"(((?<=\\\\)\")?([^\n\r\"])*)*\""), // string "
            new Regex(@"^--(?:\[(=*)\[[\s\S]*?(?:\]\1\]|$)|[^\r\n]*)[-]*"), //comment
            new Regex(@"^\[(=*)\[[\s\S]*?(?:\]\1\]|$)"), //multiline string vanilla
            new Regex(@"^(?:and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while)\b"), //keywords
            new Regex(@"^(?:\+|\-|\*|\/|\%|\^|\#|\=\=|\~\=|\<\=|\>\=|\<|\>|\=|\(|\)|\{|\}|\;|\:|\.|\.\.|\.\.\.|\!\=|\!)"), //operators
            new Regex(@"^[+-]?(?:0x[\da-f]+|(?:(?:\.\d+|\d+(?:\.\d*)?)(?:e[+\-]?\d+)?))"), //literal
            new Regex(@"^[A-z_]\w*"), //identifier
            new Regex(@"^[^\w\t\n\r \xA0][^\w\t\n\r \xA0\" + "\"" + @"\'\-\+=]*") //punctuation
        };

        /// <summary>
        /// Rules
        /// </summary>
        public static LexerRule[] Rules = new LexerRule[] {
            new LexerRule(Matches[0],"whitespace"),
            new LexerRule(Matches[1],"string"),
            new LexerRule(Matches[2],"string"),
            new LexerRule(Matches[3],"comment"),
            new LexerRule(Matches[4],"string"),
            new LexerRule(Matches[5],"keyword"),
            new LexerRule(Matches[6],"operator"),
            new LexerRule(Matches[7],"number"),
            new LexerRule(Matches[8],"identifier"),
            new LexerRule(Matches[9],"punctuation")
        };
    }
}
