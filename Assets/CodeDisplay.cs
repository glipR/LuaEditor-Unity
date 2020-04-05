using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CodeDisplay : MonoBehaviour {

    public TextMeshProUGUI text;
    public TMP_InputField inputField;
    public string stringText;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void setText(string t) {
        StartCoroutine(setTextCoroutine(t));
    }

    public IEnumerator setTextCoroutine(string t) {
        stringText = t;
        var info = text.textInfo;
        var oldCount = info.characterCount;
        CodeStyler.SetString(t);
        text.SetText(CodeStyler.GetStyle());
        while (info.characterCount == oldCount) yield return null;

        var suggest = CodeStyler.GetSuggestions(inputField.caretPosition);
        if (suggest.Count == 0) {
            Tooltip.Hide();
        }
        int i=0;
        foreach (var c in info.characterInfo) {
            if (c.bottomRight.x != 0 || c.bottomRight.y != 0) {
                i ++;
                if (i == inputField.caretPosition) {
                    List<string> s = new List<string>();
                    foreach (var x in suggest) {
                        s.Add(x.name);
                    }
                    if (s.Count == 0) {
                        Tooltip.Hide();
                    } else {
                        Tooltip.SetTooltip(s, transform.TransformPoint(c.bottomRight));
                    }
                }
            }
        }
    }

    public void executeText() {
        ScriptRunner.instance.runText(stringText);
    }

}
