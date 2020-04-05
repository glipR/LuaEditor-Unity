using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

public class ScriptRunner : MonoBehaviour {

    public Script script;
    public static ScriptRunner instance;

    public void runText(string t) {
        script.DoString(t);
    }

    public void Initialise() {
        script = new Script(MoonSharp.Interpreter.CoreModules.Preset_Default);
        script.Options.DebugPrint = s => { Debug.Log(s); };
        ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "Assets/Resources/MoonSharp/Scripts/?" };
    }

    void Start() {
        instance = this;
        var opts = new APILoadingOptions();
        opts.api = "Hanoi";
        opts.code = "-- test program\nprint(Chandelier.NumFittings())";
        opts.args = "{\"numFittings\": 3, \"numChandeliers\": 5}";
        StartCoroutine(ApiFactory.LoadAPI(opts));
    }

}
