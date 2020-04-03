using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

public class ScriptRunner : MonoBehaviour {

    Script script;
    public static ScriptRunner instace;

    public void runText(string t) {
        script.DoString(t);
    }

    void Start() {
        instace = this;
        UserData.RegisterProxyType<HanoiApiProxy, HanoiApiType>(r => new HanoiApiProxy(r));
        // MAKE THIS SOFT SANDBOX
        script = new Script(MoonSharp.Interpreter.CoreModules.Preset_Default);
        ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "Assets/Resources/MoonSharp/Scripts/?" };
        script.Options.DebugPrint = s => { Debug.Log(s); };
        script.Globals["api"] = new HanoiApiType();
    }

}
