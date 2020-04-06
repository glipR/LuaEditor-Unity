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
    private static int totalIdentifiers;

    public static float scoreThreshold = 1.5f;
    public static int maximumSuggestions = 5;

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
        totalIdentifiers = 0;
        for (int i=0; i<lexerResult.Tokens.Length; i++) {
            // we only care about these stats for identifiers.
            if (lexerResult.Tokens[i].Type != "identifier") continue;
            totalIdentifiers ++;
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
                List<(SuggestionData, float)> scored = new List<(SuggestionData, float)>();
                string tokenLower = lexerResult.Tokens[i].Value.ToLower();
                for (int j=0; j<suggestions.Count; j++) {
                    // Rank suggestion.
                    int firstMatch = -1;
                    int lastMatch = -1;
                    int match_index = 0;
                    string lowerSuggestion = suggestions[j].name.ToLower();
                    for (int k=0; k<suggestions[j].name.Length; k++) {
                        if (tokenLower[match_index] == lowerSuggestion[k]) {
                            match_index ++;
                            if (match_index == 1) firstMatch = k;
                            lastMatch = k;
                            if (match_index == lexerResult.Tokens[i].Value.Length) break;
                        }
                    }
                    float score = 0;
                    if (match_index != 0) {
                        // Matched Chars
                        score += match_index;
                        int redundantChars = lastMatch - firstMatch - match_index;
                        float nonRedundantPct = (match_index - redundantChars) / (float) match_index;
                        float relativeSize = lexerResult.Tokens[i].Value.Length / (float) suggestions[j].name.Length;
                        // Handle larger cases
                        if (relativeSize > 1) relativeSize = 1 / relativeSize;
                        float relativeFreq = suggestions[j].occurrences / (float) totalIdentifiers;
                        // Just some hardcoded values for now - shouldn't need to be publiccally accessible.
                        score += 0.8f * nonRedundantPct;
                        score += 0.4f * (relativeSize - 0.5f);
                        score += 0.2f * relativeFreq;
                        if (lexerResult.Tokens[i].Value == suggestions[j].name && suggestions[j].occurrences == 1) {
                            // This is the only match
                            score = 0;
                        }
                    }
                    if (score > scoreThreshold) {
                        bool inserted = false;
                        for (int k=0; k<scored.Count; k++) {
                            if (scored[k].Item2 < score) {
                                scored.Insert(k, (suggestions[j], score));
                                inserted = true;
                                break;
                            }
                        }
                        if (!inserted) {
                            scored.Add((suggestions[j], score));
                        }
                        if (scored.Count > maximumSuggestions) {
                            scored.RemoveAt(scored.Count - 1);
                        }
                    }
                }
                foreach (var x in scored) {
                    res.Add(x.Item1);
                }
                return res;
            }
        }
        return res;
    }
}
