using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public abstract class IAPIDef {

    public string docLink;

    public abstract void AddToScript(Script s);

    public GameObject apiPrefab;

    // Also must implement:
    // * An options class - which defines the different ways the scene can be loaded

    // String Serialized version of options class.
    public abstract IEnumerator Load(string optionString);

    public virtual IEnumerator Initialise(GameObject par) {
        GameObject.Instantiate(apiPrefab, par.transform);
        yield return null;
    }

}
