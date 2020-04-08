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
        string base_name = "Chandeliers";
        s.Globals[base_name] = new HanoiApiType();
        var suggest = new List<(List<string> name, string type)>();
        var chandeliers = new List<string>();
        chandeliers.Add(base_name);
        suggest.Add((chandeliers, "class"));
        var move = new List<string>();
        move.Add(base_name);
        move.Add("MoveChandelier");
        suggest.Add((move, "method"));
        var numFittings = new List<string>();
        numFittings.Add(base_name);
        numFittings.Add("NumFittings");
        suggest.Add((numFittings, "method"));
        var numChandeliers = new List<string>();
        numChandeliers.Add(base_name);
        numChandeliers.Add("NumChandeliers");
        suggest.Add((numChandeliers, "method"));
        CodeStyler.AddSuggestions(suggest);
    }

    public override IEnumerator Load(string optionString) {
        var options = JsonUtility.FromJson<HanoiOptions>(optionString);
        yield return Load(options);
    }

    public IEnumerator Load(HanoiOptions hs) {
        yield return HanoiApi.instance.Instantiate(hs);
    }
}
