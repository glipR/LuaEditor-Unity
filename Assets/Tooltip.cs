using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour {

    public static Tooltip instance;

    private RectTransform rect;
    private Transform background;
    public GameObject textPrefab;
    private bool open;
    private List<(int toRemove, string toInsert)> suggestions;
    public int suggestionIndex;

    private void Awake() {
        instance = this;
        rect = GetComponent<RectTransform>();
        background = transform.Find("background");

        Hide();
    }

    private void TryShiftSelectionUp() {
        var tm = background.GetChild(suggestionIndex).GetComponent<Image>();
        tm.color = new Color(0, 0, 0, 0);
        if (suggestionIndex != 0) {
            suggestionIndex --;
        }
        Debug.Log(suggestionIndex);
        tm = background.GetChild(suggestionIndex).GetComponent<Image>();
        tm.color = new Color(0, 50, 0, 0.5f);
    }

    private void TryShiftSelectionDown() {
        var tm = background.GetChild(suggestionIndex).GetComponent<Image>();
        tm.color = new Color(0, 0, 0, 0);
        if (!(suggestionIndex >= suggestions.Count)) {
            suggestionIndex ++;
        }
        tm = background.GetChild(suggestionIndex).GetComponent<Image>();
        tm.color = new Color(0, 50, 0, 0.5f);
    }

    private void ShowTooltip(List<string> s) {
        suggestionIndex = 0;
        open = true;
        gameObject.SetActive(true);

        foreach (Transform t in background) {
            Destroy(t.gameObject);
        }

        Vector2 backgroundSize = new Vector2(0, 0);
        int i=0;
        foreach (var st in s) {
            var text = Instantiate(textPrefab, background);
            if (i == 0) {
                text.GetComponent<Image>().color = new Color(0, 50, 0, 0.5f);
            }
            text.name = "Tooltip " + (i++);
            var tm = text.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
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
        return instance.suggestions[instance.suggestionIndex];
    }

    public static void ShiftSelectionUp() {
        instance.TryShiftSelectionUp();
    }

    public static void ShiftSelectionDown() {
        instance.TryShiftSelectionDown();
    }
}
