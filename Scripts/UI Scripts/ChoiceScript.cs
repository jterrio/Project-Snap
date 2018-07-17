using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScript : MonoBehaviour {

    public int choiceSet; //tells npc speech holder which set to jump to

    //send the info the speech manager of our choice
    public void TaskOnClick() {
        SpeechManager.ins.PickChoice(choiceSet);
    }
}
