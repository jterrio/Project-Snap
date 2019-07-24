using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScript : MonoBehaviour {

    public int choiceSet; //tells npc speech holder which set to jump to

    /// <summary>
    /// Sends info of choice to the SpeechManager
    /// </summary>
    public void TaskOnClick() {
        SpeechManager.ins.PickChoice(choiceSet);
    }
}
