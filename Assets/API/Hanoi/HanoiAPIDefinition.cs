using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[System.Serializable]
public class HanoiOptions {
    public int numFittings;
    public int numChandeliers;
}

public class HanoiAPIDefinition: IAPIDef {
    public new string docLink = "test";

    public override void Initialise(GameObject par) {
        apiPrefab = Resources.Load<GameObject>("Prefabs/API/Hanoi");
        base.Initialise(par);
    }

    public override void AddToScript(Script s) {
        UserData.RegisterProxyType<HanoiApiProxy, HanoiApiType>(r => new HanoiApiProxy(r));
        s.Globals["Chandeliers"] = new HanoiApiType();
    }

    public override void Load(string optionString) {
        var options = JsonUtility.FromJson<HanoiOptions>(optionString);
        Load(options);
    }

    public void Load(HanoiOptions hs) {

    }
}
