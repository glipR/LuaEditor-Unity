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

    public static LuaParser.Lexer lexer;
    public static LuaParser.LexerResult lexerResult;
    public static string original;

    public class SuggestionData {
        public string name;
        public string type;
        public int occurrences;

        public SuggestionData(string s1, string s2, int d1) {
            name = s1;
            type = s2;
            occurrences = d1;
        }
    }

    public static List<SuggestionData> suggestions;

    public static void SetString(string luaCode) {
        original = luaCode;
        lexer = new LuaParser.Lexer(luaCode);
        lexerResult = lexer.Tokenize();
        for (int i=0; i<suggestions.Count; i++) {
            suggestions[i].occurrences = 0;
        }
        for (int i=0; i<lexerResult.Tokens.Length; i++) {
            // we only care about these stats for identifiers.
            if (lexerResult.Tokens[i].Type != "identifier") continue;
            bool found = false;
            for (int j=0; j<suggestions.Count; j++) {
                if (suggestions[j].type == lexerResult.Tokens[i].Type && suggestions[j].name == lexerResult.Tokens[i].Value) {
                    found = true;
                    suggestions[j].occurrences ++;
                }
            }
            if (!found) {
                suggestions.Add(new SuggestionData(lexerResult.Tokens[i].Value, lexerResult.Tokens[i].Type, 1));
            }
        }
        var toRemove = new List<int>();
        // Remove unused identifiers.
        for (int i=0; i<suggestions.Count; i++) {
            if (suggestions[i].occurrences == 0 && suggestions[i].type == "identifier") {
                toRemove.Add(i);
            }
        }
        toRemove.Reverse();
        foreach (int i in toRemove) {
            suggestions.RemoveAt(i);
        }
    }

    public static void SetSuggestions(List<(string name, string type)> suggest) {
        suggestions = new List<SuggestionData>();
        foreach (var s in suggest) {
            suggestions.Add(new SuggestionData(s.name, s.type, 0));
        }
    }

    public static string GetStyle() {
        var styles = new List<StyleInsertion>();

        for (int i=0; i<lexerResult.Tokens.Length; i++) {
            if (lexerResult.Tokens[i].Type != "whitespace") {
                if (i + 1 == lexerResult.Tokens.Length) {
                    styles.Add(new StyleInsertion(lexerResult.Tokens[i].Location.Position, original.Length - lexerResult.Tokens[i].Location.Position, lexerResult.Tokens[i].Type));
                } else {
                    styles.Add(new StyleInsertion(lexerResult.Tokens[i].Location.Position, lexerResult.Tokens[i+1].Location.Position - lexerResult.Tokens[i].Location.Position, lexerResult.Tokens[i].Type));
                }
            }
        }

        string result = original;
        int inserted = 0;
        foreach (var si in styles) {
            result = (
                result.Substring(0, si.index + inserted) +
                "<style=\"" + si.styleName + "\">" +
                result.Substring(si.index + inserted, si.length) +
                "</style>" +
                result.Substring(si.index + inserted + si.length)
            );
            inserted += ("<style=\"" + si.styleName + "\">").Length + "</style>".Length;
        }

        return result;
    }

    public static List<SuggestionData> GetSuggestions(int caretPos) {
        var res = new List<SuggestionData>();
        for (int i=0; i<lexerResult.Tokens.Length; i++) {
            if (lexerResult.Tokens[i].Location.Position + lexerResult.Tokens[i].Value.Length == caretPos) {
                if (lexerResult.Tokens[i].Type != "identifier") return res;
                // TODO: Make good suggestions with substring matching, and ignoring itself if its the only identifier.
                return suggestions;
            }
        }
        return res;
    }
}
