using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class HanoiApi : MonoBehaviour {

    public static HanoiApi instance;
    public static bool ready = false;

    public int actions = 0;
    public List<GameObject> Rungs;
    public List<GameObject> Rings;
    public List<List<(int, Vector3)> > RingMatrix;
    public List<float> Tweens;

    public float ringHeight;
    public float rungWidth;

    public IEnumerator Instantiate(HanoiOptions hs) {
        // First, remove all elements except the RungPanel
        foreach (Transform tf in transform) {
            if (tf.name == "RungPanel") {
                foreach (Transform tf2 in tf) {
                    Destroy(tf2.gameObject);
                }
            } else {
                Destroy(tf.gameObject);
            }
        }
        // Next, create the rungs/rings, and add them to the object.
        var rungPrefab = Resources.Load<GameObject>("Prefabs/API/Hanoi/Rung");
        var ringPrefab = Resources.Load<GameObject>("Prefabs/API/Hanoi/Ring");
        var rungPanel = transform.Find("RungPanel");
        Rungs = new List<GameObject>();
        Rings = new List<GameObject>();
        for (int i=1; i<=hs.numFittings; i++) {
            var g = Instantiate(rungPrefab, rungPanel);
            g.name = "Rung " + i;
            Rungs.Add(g);
        }
        Tweens = new List<float>();
        for (int i=1; i<= hs.numChandeliers; i++) {
            var g = Instantiate(ringPrefab, transform);
            g.GetComponent<RectTransform>().sizeDelta = new Vector2(50 + 25 * (hs.numChandeliers - i), 40);
            g.name = "Ring " + i;
            Rings.Add(g);
        }
        // Update the local values in RingMatrix
        RingMatrix = new List<List<(int, Vector3)>>();
        for (int i=0; i<Rungs.Count; i++)
            RingMatrix.Add(new List<(int, Vector3)>());
        for (int i=0; i<Rings.Count; i++) {
            var rt = Rings[i].GetComponent<RectTransform>();
            rt.localPosition = ChandelierPosition(0, i);
            RingMatrix[0].Add((i, rt.localPosition));
        }
        yield return null;
    }

    public Vector3 ChandelierPosition(int fittingNumber, int heightIndex) {
        var sd = GetComponent<RectTransform>().rect.size;
        var res = new Vector3(0, 0, 0);
        res.x += 20 + ((sd.x - 40) / NumRungs()) * (fittingNumber + 0.5f) - sd.x / 2f ;
        res.y += ringHeight * heightIndex - sd.y / 2f;
        return res;
    }

    private void Start() {
        instance = this;
        ready = true;
        var t = new HanoiOptions();
        t.numChandeliers = 4;
        t.numFittings = 3;
        StartCoroutine(Instantiate(t));
    }

    private void OnDestroy() {
        instance = null;
        ready = false;
    }

    public int NumRings() {
        return Rings.Count;
    }

    public int NumRungs() {
        return Rungs.Count;
    }

    public IEnumerator MoveRingCoroutine(RectTransform rectTransform, Vector3 currentPos, Vector3 raise, Vector3 aboveNew, Vector3 newLocation) {
        float num = UnityEngine.Random.Range(0f, 1f);
        Tweens.Add(num);
        while (Tweens[0] != num) yield return null;
        rectTransform.gameObject.Tween(rectTransform.gameObject, 0f, 1f, 0.3f, TweenScaleFunctions.QuadraticEaseIn, (t)=> {
            Vector3 project = (raise - currentPos) * t.CurrentValue + currentPos;
            rectTransform.localPosition = project;
        }).ContinueWith(new FloatTween().Setup(0f, 1f, 0.5f, TweenScaleFunctions.Linear, (t) => {
            Vector3 project = (aboveNew - raise) * t.CurrentValue + raise;
            rectTransform.localPosition = project;
        })).ContinueWith(new FloatTween().Setup(0f, 1f, 0.3f, TweenScaleFunctions.QuadraticEaseOut, (t) => {
            Vector3 project = (newLocation - aboveNew) * t.CurrentValue + aboveNew;
            rectTransform.localPosition = project;
        }, (t)=> {
            actions -= 1;
            Tweens.RemoveAt(0);
        }));
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
        var rt = Rings[v.Item1].GetComponent<RectTransform>();
        Vector3 endRungPos = ChandelierPosition(endIndex, 0);
        Vector3 cPos = v.Item2;
        Vector3 raise = cPos;
        raise.y = endRungPos.y + 220;
        Vector3 newLocation = ChandelierPosition(endIndex, RingMatrix[endIndex].Count);
        Vector3 aboveNew = newLocation;
        aboveNew.y = endRungPos.y + 220;
        v.Item2 = newLocation;
        RingMatrix[endIndex].Add(v);
        StartCoroutine(MoveRingCoroutine(rt, cPos, raise, aboveNew, newLocation));
    }

}
