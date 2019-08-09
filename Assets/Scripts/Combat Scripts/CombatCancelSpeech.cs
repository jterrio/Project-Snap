using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCancelSpeech : MonoBehaviour {

    public RectTransform parent;

    public void CancelSpeech() {
        CombatManager.ins.combatSpeech.RemoveFromQueue(parent.GetComponent<RectTransform>());
    }

}
