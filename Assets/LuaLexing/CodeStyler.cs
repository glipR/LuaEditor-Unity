using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class StyleInsertion {
    public int index;
    public int length;
    public string styleName;

    public StyleInsertion(int i, int l, string s) {
        index = i;
        length = l;
        styleName = s;
    }
}

public static class CodeStyler {

    public static string Style(string luaCode) {
        var lexer = new LuaParser.Lexer(luaCode);
        var res = lexer.Tokenize();

        var styles = new List<StyleInsertion>();

        for (int i=0; i<res.Tokens.Length; i++) {
            if (res.Tokens[i].Type != "whitespace") {
                if (i + 1 == res.Tokens.Length) {
                    styles.Add(new StyleInsertion(res.Tokens[i].Location.Position, luaCode.Length - res.Tokens[i].Location.Position, res.Tokens[i].Type));
                } else {
                    styles.Add(new StyleInsertion(res.Tokens[i].Location.Position, res.Tokens[i+1].Location.Position - res.Tokens[i].Location.Position, res.Tokens[i].Type));
                }
            }
        }

        int inserted = 0;
        foreach (var si in styles) {
            luaCode = (
                luaCode.Substring(0, si.index + inserted) +
                "<style=\"" + si.styleName + "\">" +
                luaCode.Substring(si.index + inserted, si.length) +
                "</style>" +
                luaCode.Substring(si.index + inserted + si.length)
            );
            inserted += ("<style=\"" + si.styleName + "\">").Length + "</style>".Length;
        }

        return luaCode;
    }
}
