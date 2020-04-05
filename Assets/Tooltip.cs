using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {

    public static Tooltip instance;

    private RectTransform rect;
    private Transform background;
    public GameObject textPrefab;

    private void Awake() {
        instance = this;
        rect = GetComponent<RectTransform>();
        background = transform.Find("background");

        Hide();
    }

    private void ShowTooltip(List<string> s) {
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
        gameObject.SetActive(false);
    }

    public static void SetTooltip(List<string> s, Vector3 worldPos) {
        instance.ShowTooltip(s);
        var sd = instance.GetComponent<RectTransform>().sizeDelta;
        instance.transform.position = worldPos + new Vector3(sd.x / 2f, -sd.y / 2f, 0);
    }

    public static void Hide() {
        instance.HideTooltip();
    }
}
