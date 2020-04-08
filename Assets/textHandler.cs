using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textHandler : MonoBehaviour {

   private EventfullInputField inputField;

   private void Start() {
       inputField = GetComponent<EventfullInputField>();
       inputField.handleEnterPress.AddListener(HandleEnter);
       inputField.handleUpPress.AddListener((bool x) => HandleUpDown(x, true));
       inputField.handleDownPress.AddListener((bool x) => HandleUpDown(x, false));
       inputField.handleEscapePress.AddListener(HandleEscape);
   }
   public void HandleEnter() {
       if (Tooltip.suggestionsOpen()) {
           inputField.RemoveLastAndInsert(Tooltip.currentSuggestion());
           Tooltip.Hide();
       } else {
           inputField.Append('\n');
       }
   }

   public void HandleEscape() {
       if (Tooltip.suggestionsOpen()) {
           Tooltip.Hide();
       } else {
           inputField.EscapeSelection();
       }
   }

   public void HandleUpDown(bool shift, bool dir) {
       if (shift || !Tooltip.suggestionsOpen()) {
           if (dir) {
               inputField.MoveUp(shift);
           } else {
               inputField.MoveDown(shift);
           }
       } else {
           if (dir) {
               Tooltip.ShiftSelectionUp();
           } else {
               Tooltip.ShiftSelectionDown();
           }
       }
   }

   private void OnDestroy() {
       inputField.handleEnterPress.RemoveAllListeners();
   }
}
