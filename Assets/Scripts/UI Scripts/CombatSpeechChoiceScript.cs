using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSpeechChoiceScript : MonoBehaviour{

    private int choice;

    /// <summary>
    /// Sends info of choice to the SpeechManager
    /// </summary>
    public void TaskOnClick() {
        CombatManager.ins.combatSpeech.ChoiceSelected(choice);
    }

    public void SetChoice(int i) {
        choice = i;
    }

}
