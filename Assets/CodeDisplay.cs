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
        stringText = t;
        text.text = CodeStyler.Style(t);
    }

    public void executeText() {
        ScriptRunner.instance.runText(stringText);
    }

}
