using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class HanoiApi : MonoBehaviour {

    public static HanoiApi instance;

    public int actions = 0;
    private List<Func<bool> > tweens = new List<Func<bool> >();
    public List<GameObject> Rungs;
    public List<GameObject> Rings;
    public List<List<(int, Vector3)> > RingMatrix;

    public float ringHeight;
    public float rungWidth;

    private void Start() {
        instance = this;
        // Place the rings on the correct rung.
        var rung = Rungs[0].GetComponent<RectTransform>();
        RingMatrix = new List<List<(int, Vector3)>>();
        for (int i=0; i<Rungs.Count; i++)
            RingMatrix.Add(new List<(int, Vector3)>());
        for (int i=0; i<Rings.Count; i++) {
            var rt = Rings[i].GetComponent<RectTransform>();
            rt.localPosition = rung.localPosition + new Vector3(rungWidth / 2f, i * ringHeight, 0);
            RingMatrix[0].Add((i, rt.localPosition));
        }
    }

    private void RunTween() {
        if (tweens.Count == 0) return;
        var f = tweens[0];
        tweens.RemoveAt(0);
        f();
    }

    public int NumRings() {
        return Rings.Count;
    }

    public int NumRungs() {
        return Rungs.Count;
    }

    public void MoveRing(int startIndex, int endIndex) {
        if (startIndex < 0 || startIndex >= Rings.Count)
            throw new System.Exception("Invalid Rung index!");
        if (endIndex < 0 || endIndex >= Rings.Count)
            throw new System.Exception("Invalid Rung index!");
        if (RingMatrix[startIndex].Count == 0)
            throw new System.Exception("Rung selected is empty!");
        if (RingMatrix[endIndex].Count != 0 && RingMatrix[endIndex][RingMatrix[endIndex].Count - 1].Item1 > RingMatrix[startIndex][RingMatrix[startIndex].Count - 1].Item1)
            throw new System.Exception("Destination rung has larger disk than beginning disk!");
        var v = RingMatrix[startIndex][RingMatrix[startIndex].Count - 1];
        RingMatrix[startIndex].RemoveAt(RingMatrix[startIndex].Count - 1);
        actions ++;
        bool done = false;
        var rt = Rings[v.Item1].GetComponent<RectTransform>();
        Vector3 endRungPos = Rungs[endIndex].GetComponent<RectTransform>().localPosition;
        Vector3 cPos = v.Item2;
        Vector3 raise = cPos;
        raise.y = endRungPos.y + 220;
        Vector3 newLocation = endRungPos + new Vector3((1 - endIndex) * rungWidth / 2f, RingMatrix[endIndex].Count * ringHeight, 0);
        Vector3 aboveNew = newLocation;
        aboveNew.y = endRungPos.y + 220;
        v.Item2 = newLocation;
        RingMatrix[endIndex].Add(v);
        tweens.Add(() => {rt.gameObject.Tween(rt.gameObject, 0f, 1f, 0.3f, TweenScaleFunctions.QuadraticEaseIn, (t)=> {
            Vector3 project = (raise - cPos) * t.CurrentValue + cPos;
            rt.localPosition = project;
        }).ContinueWith(new FloatTween().Setup(0f, 1f, 0.5f, TweenScaleFunctions.Linear, (t) => {
            Vector3 project = (aboveNew - raise) * t.CurrentValue + raise;
            rt.localPosition = project;
        })).ContinueWith(new FloatTween().Setup(0f, 1f, 0.3f, TweenScaleFunctions.QuadraticEaseOut, (t) => {
            Vector3 project = (newLocation - aboveNew) * t.CurrentValue + aboveNew;
            rt.localPosition = project;
        })).ContinueWith(new FloatTween().Setup(0f, 1f, 0.01f, TweenScaleFunctions.Linear, (t)=> {
            if (!done) {
                done = true;
                actions -= 1;
                RunTween();
            }
        })); return true; });
        if (actions == 1) {
            RunTween();
        }
    }

}
