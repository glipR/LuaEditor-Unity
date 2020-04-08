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
        public List<string> namePath;
        public string type;
        public int occurrences;
        public int toRemove;

        public SuggestionData(List<string> s1, string s2, int d1, int d2) {
            namePath = s1;
            type = s2;
            occurrences = d1;
            toRemove = d2;
        }
    }

    public static List<SuggestionData> suggestions;

    private static List<string> namePath(int tokenIndex) {
        var res = new List<string>();
        while (true) {
            res.Insert(0, lexerResult.Tokens[tokenIndex].Value);
            if (lexerResult.Tokens[tokenIndex].Type == "identifier" && tokenIndex > 1) {
                if (lexerResult.Tokens[tokenIndex - 1].Value == ".") {
                    tokenIndex -= 2;
                } else break;
            } else break;
        }
        return res;
    }

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
            var names = namePath(i);
            for (int j=0; j<suggestions.Count; j++) {
                bool same = false;
                if (suggestions[j].namePath.Count == names.Count) {
                    same = true;
                    for (int k=0; k<names.Count; k++) if (names[k] != suggestions[j].namePath[k]) same = false;
                }
                if (same) {
                    found = true;
                    suggestions[j].occurrences ++;
                }
            }
            if (!found) {
                suggestions.Add(new SuggestionData(names, lexerResult.Tokens[i].Type, 1, 0));
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

    public static void SetSuggestions(List<(List<string> name, string type)> suggest) {
        suggestions = new List<SuggestionData>();
        foreach (var s in suggest) {
            suggestions.Add(new SuggestionData(s.name, s.type, 0, 0));
        }
    }

    public static void AddSuggestions(List<(List<string> name, string type)> suggest) {
        foreach (var s in suggest) {
            suggestions.Add(new SuggestionData(s.name, s.type, 0, 0));
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
                if (lexerResult.Tokens[i].Type != "identifier" && lexerResult.Tokens[i].Value != ".") return res;
                if (lexerResult.Tokens[i].Value == "." && i == 0) return res;
                string endMatch;
                List<string> prePathMatch;
                if (lexerResult.Tokens[i].Type == "identifier") {
                    prePathMatch = namePath(i);
                    endMatch = prePathMatch[prePathMatch.Count - 1].ToLower();
                    prePathMatch.RemoveAt(prePathMatch.Count - 1);
                } else {
                    endMatch = "";
                    prePathMatch = namePath(i-1);
                }
                List<(SuggestionData, float)> scored = new List<(SuggestionData, float)>();
                for (int j=0; j<suggestions.Count; j++) {
                    bool same = false;
                    if (suggestions[j].namePath.Count == prePathMatch.Count + 1) {
                        same = true;
                        for (int k=0; k<prePathMatch.Count; k++) if (prePathMatch[k] != suggestions[j].namePath[k]) same = false;
                    }
                    // Must have the exact same parent structure.
                    if (!same) continue;
                    suggestions[j].toRemove = endMatch.Length;
                    // Rank suggestion.
                    int firstMatch = -1;
                    int lastMatch = -1;
                    int match_index = 0;
                    string lowerSuggest = suggestions[j].namePath[suggestions[j].namePath.Count - 1].ToLower();
                    for (int k=0; k<lowerSuggest.Length; k++) {
                        if (match_index == endMatch.Length) break;
                        if (endMatch[match_index] == lowerSuggest[k]) {
                            match_index ++;
                            if (match_index == 1) firstMatch = k;
                            lastMatch = k;
                        }
                    }
                    float score = 0;
                    if (match_index != 0) {
                        // Matched Chars
                        score += match_index;
                        int redundantChars = lastMatch - firstMatch - match_index;
                        float nonRedundantPct = (match_index - redundantChars) / (float) match_index;
                        float relativeSize = lexerResult.Tokens[i].Value.Length / (float) lowerSuggest.Length;
                        // Handle larger cases
                        if (relativeSize > 1) relativeSize = 1 / relativeSize;
                        float relativeFreq = suggestions[j].occurrences / (float) totalIdentifiers;
                        // Just some hardcoded values for now - shouldn't need to be publically accessible.
                        score += 0.8f * nonRedundantPct;
                        score += 0.4f * (relativeSize - 0.5f);
                        score += 0.2f * relativeFreq;
                    }
                    if (endMatch == lowerSuggest && suggestions[j].occurrences == 1) {
                        // This is the only match, ignore
                        continue;
                    }
                    if (score > scoreThreshold || (prePathMatch.Count > 0)) {
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
                var y = namePath(i);
                if ((scored.Count == 1 || (scored.Count > 1 && (scored[0].Item2 - scored[1].Item2 > 1.5f))) && scored[0].Item1.namePath[scored[0].Item1.namePath.Count - 1] == y[y.Count - 1]) {
                    // Show nothing - The only suggestion is your element in particular - or the difference is too big.
                    scored = new List<(SuggestionData, float)>();
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
