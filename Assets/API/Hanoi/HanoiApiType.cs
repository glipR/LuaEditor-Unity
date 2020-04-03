using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class HanoiApiType {
    public void MoveRing(int x, int y) {
        HanoiApi.instance.MoveRing(x, y);
    }
}

public class HanoiApiProxy {
    HanoiApiType obj;
    [MoonSharpHidden]
    public HanoiApiProxy(HanoiApiType p) {
        this.obj = p;
    }

    public void MoveRing(int x, int y) {
        this.obj.MoveRing(x, y);
    }
}
