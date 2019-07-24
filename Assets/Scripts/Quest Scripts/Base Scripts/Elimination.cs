using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elimination : Quest {

    public GameObject targetNPC;
    public int targetCount;
    public int currentCount;


    public bool isTarget(GameObject test) {
        if(test.GetComponent<NPCInfo>().npc == targetNPC.GetComponent<NPCInfo>().npc) {
            return true;
        }
        return false;
    }

    public void AddKill() {
        currentCount += 1;
    }

    bool HasKilledEnough() {
        if(currentCount >= targetCount) {
            return true;
        }
        return false;
    }

}
