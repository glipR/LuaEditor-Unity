using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class HanoiApiType {
    public void MoveRing(int x, int y) {
        HanoiApi.instance.MoveRing(x, y);
    }

    public int NumRings() {
        return HanoiApi.instance.NumRings();
    }

    public int NumRungs() {
        return HanoiApi.instance.NumRungs();
    }
}

public class HanoiApiProxy {
    HanoiApiType obj;
    [MoonSharpHidden]
    public HanoiApiProxy(HanoiApiType p) {
        this.obj = p;
    }

    public void MoveChandelier(int x, int y) {
        this.obj.MoveRing(x, y);
    }

    public int NumChandeliers() {
        return this.obj.NumRings();
    }

    public int NumFittings() {
        return this.obj.NumRungs();
    }
}
