using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APILoadingOptions {
    // Name of the api, for loading.
    public string api;
    // Arguments to pass to the api.
    public string args;
    // Sample code to be placed in the editor.
    public string code;
}

public static class ApiFactory {
    public enum Apis {
        Hanoi,
        Boxes
    }

    public static IAPIDef GetApi(string name) {
        if (name == Apis.Hanoi.ToString()) {
            return new HanoiAPIDefinition();
        }
        if (name == Apis.Boxes.ToString()) {
            return new BoxAPIDefinition();
        }
        throw new System.Exception("Invalid API Name: " + name);
    }

    public static IEnumerator LoadAPI(APILoadingOptions options) {
        var api = GetApi(options.api);
        var g = GameObject.Find("ApiPanel");
        foreach (Transform child in g.transform) {
            GameObject.Destroy(child.gameObject);
        }
        yield return api.Initialise(g);
        yield return api.Load(options.args);
        ScriptRunner.instance.Initialise();
        var defaultSuggest = new List<(List<string> name, string type)>();
        foreach (var keyword in "and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while".Split('|')) {
            var l = new List<string>();
            l.Add(keyword);
            defaultSuggest.Add((l, "keyword"));
        }
        CodeStyler.SetSuggestions(defaultSuggest);
        api.AddToScript(ScriptRunner.instance.script);
    }
}
