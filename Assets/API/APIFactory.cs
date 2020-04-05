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

    public static void LoadAPI(APILoadingOptions options) {
        var api = GetApi(options.api);
        var g = GameObject.Find("ApiPanel");
        foreach (Transform child in g.transform) {
            GameObject.Destroy(child.gameObject);
        }
        api.Initialise(g);
        api.Load(options.args);
        ScriptRunner.instance.Initialise();
        api.AddToScript(ScriptRunner.instance.script);
    }
}
