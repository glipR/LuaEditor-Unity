using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {

    public static Tooltip instance;

    private RectTransform rect;
    private Transform background;
    public GameObject textPrefab;
    private bool open;
    private List<(int toRemove, string toInsert)> suggestions;

    private void Awake() {
        instance = this;
        rect = GetComponent<RectTransform>();
        background = transform.Find("background");

        Hide();
    }

    private void ShowTooltip(List<string> s) {
        open = true;
        gameObject.SetActive(true);

        foreach (Transform t in background) {
            Destroy(t.gameObject);
        }

        Vector2 backgroundSize = new Vector2(0, 0);
        foreach (var st in s) {
            var text = Instantiate(textPrefab, background);
            var tm = text.GetComponent<TextMeshProUGUI>();
            var str = st;
            if (st.Length > 14) {
                str = st.Substring(0, 11) + "...";
            }
            tm.SetText(str);
            backgroundSize.x = Mathf.Max(backgroundSize.x, tm.preferredWidth);
            backgroundSize.y += tm.preferredHeight;
        }
        backgroundSize.x += 20;
        backgroundSize.x = Mathf.Min(150, backgroundSize.x);
        rect.sizeDelta = backgroundSize;
    }

    private void HideTooltip() {
        open = false;
        gameObject.SetActive(false);
    }

    public static void SetTooltip(List<(int toRemove, string toInsert)> s, Vector3 worldPos) {
        var r = new List<string>();
        foreach (var t in s) r.Add(t.toInsert);
        instance.suggestions = s;
        instance.ShowTooltip(r);
        var sd = instance.GetComponent<RectTransform>().sizeDelta;
        instance.transform.position = worldPos + new Vector3(sd.x / 2f, -sd.y / 2f, 0);
    }

    public static void Hide() {
        instance.HideTooltip();
    }

    public static bool suggestionsOpen() {
        return instance.open;
    }

    public static (int toRemove, string toInsert) currentSuggestion() {
        return instance.suggestions[0];
    }
}
