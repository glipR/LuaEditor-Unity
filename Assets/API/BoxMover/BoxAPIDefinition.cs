using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[System.Serializable]
public class BoxOptions {
    public int boxX;
    public int boxY;
}

public class BoxAPIDefinition: IAPIDef {
    public new string docLink = "test";

    public override void Initialise(GameObject par) {
        apiPrefab = Resources.Load<GameObject>("Prefabs/API/Box");
        base.Initialise(par);
    }

    public override void AddToScript(Script s) {
        UserData.RegisterProxyType<BoxApiProxy, BoxApiType>(r => new BoxApiProxy(r));
        s.Globals["Box"] = new BoxApiType();
    }

    public override void Load(string optionString) {
        var options = JsonUtility.FromJson<BoxOptions>(optionString);
        Load(options);
    }

    public void Load(BoxOptions hs) {

    }
}
