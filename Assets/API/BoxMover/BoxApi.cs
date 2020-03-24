using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class BoxApi : MonoBehaviour {

    public static BoxApi instance;
    public GameObject box;
    public Vector3 localPos;
    public List<Vector3> positions = new List<Vector3>();
    public Tween<float> moveTween;

    private void Start() {
        instance = this;
        var rt = box.GetComponent<RectTransform>();
        localPos = rt.localPosition;
    }

    public void MoveBox(int x, int y) {
        var rt = box.GetComponent<RectTransform>();
        Vector3 cPos = localPos;
        Vector3 diff = new Vector3(rt.sizeDelta.x * x, rt.sizeDelta.y * y, 0);
        localPos += diff;
        if (positions.Count == 0) {
            positions.Add(cPos + diff);
            bool done = false;
            moveTween = gameObject.Tween(box, 0f, 1f, 0.5f, TweenScaleFunctions.QuadraticEaseInOut, (t)=> {
                Vector3 project = cPos + diff * t.CurrentValue;
                rt.localPosition = project;
            }).ContinueWith(new FloatTween().Setup(0, 1, 0.01f, TweenScaleFunctions.Linear, (t)=> {
                if (!done) {
                    done = true;
                    positions.RemoveAt(0);
                }
            }));
        } else {
            positions.Add(cPos + diff);
            bool done = false;
            moveTween = moveTween.ContinueWith(new FloatTween().Setup(0f, 1f, 0.5f, TweenScaleFunctions.QuadraticEaseInOut, (t) => {
                Vector3 project = cPos + diff * t.CurrentValue;
                rt.localPosition = project;
            }))
                .ContinueWith(new FloatTween().Setup(0, 1, 0.01f, TweenScaleFunctions.Linear, (t)=> {
                    if (!done) {
                        done = true;
                        positions.RemoveAt(0);
                    }
            }));
        }
    }

}
