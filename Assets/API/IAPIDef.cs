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
    public abstract void Load(string optionString);

    public virtual void Initialise(GameObject par) {
        GameObject.Instantiate(apiPrefab, par.transform);
    }

}
