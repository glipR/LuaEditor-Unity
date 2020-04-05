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

    public override IEnumerator Initialise(GameObject par) {
        apiPrefab = Resources.Load<GameObject>("Prefabs/API/Hanoi/Hanoi");
        yield return base.Initialise(par);
        while (!HanoiApi.ready) yield return null;
    }

    public override void AddToScript(Script s) {
        UserData.RegisterProxyType<HanoiApiProxy, HanoiApiType>(r => new HanoiApiProxy(r));
        s.Globals["Chandeliers"] = new HanoiApiType();
    }

    public override IEnumerator Load(string optionString) {
        var options = JsonUtility.FromJson<HanoiOptions>(optionString);
        yield return Load(options);
    }

    public IEnumerator Load(HanoiOptions hs) {
        yield return HanoiApi.instance.Instantiate(hs);
    }
}
