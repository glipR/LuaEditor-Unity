using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textHandler : MonoBehaviour {

   private TMP_InputField inputField;

   private void Start() {
       inputField = GetComponent<TMP_InputField>();
       inputField.handleEnterPress.AddListener(HandleEnter);
   }

   public void HandleEnter() {
       if (Tooltip.suggestionsOpen()) {
           inputField.RemoveLastAndInsert(Tooltip.currentSuggestion());
       } else {
           inputField.Append('\n');
       }
   }

   private void OnDestroy() {
       inputField.handleEnterPress.RemoveAllListeners();
   }
}
