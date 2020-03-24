using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

public class ScriptRunner : MonoBehaviour {
    void Start() {
        UserData.RegisterProxyType<BoxApiProxy, BoxApiType>(r => new BoxApiProxy(r));
        Script S = new Script(MoonSharp.Interpreter.CoreModules.Preset_SoftSandbox);
        S.Options.DebugPrint = s => { Debug.Log(s); };
        S.Globals["api"] = new BoxApiType();
        S.DoString(@"
            for i = 0,2,1 do
                api.MoveBox(0, -1)
                api.MoveBox(-1, 0)
            end
            for x = 0,4,1
            do
                for i = 0,4,1
                do
                    if x % 2 == 0 then
                        api.MoveBox(0, 1)
                    else
                        api.MoveBox(0, -1)
                    end
                end
                api.MoveBox(1, 0)
            end
        ");
    }

}
