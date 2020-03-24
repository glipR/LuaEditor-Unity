using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class BoxApiType {
    public void MoveBox(int x, int y) {
        BoxApi.instance.MoveBox(x, y);
    }
}

public class BoxApiProxy {
    BoxApiType obj;
    [MoonSharpHidden]
    public BoxApiProxy(BoxApiType p) {
        this.obj = p;
    }

    public void MoveBox(int x, int y) {
        this.obj.MoveBox(x, y);
    }
}
