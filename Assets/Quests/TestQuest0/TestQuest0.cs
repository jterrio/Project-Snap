using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQuest0 : Quest {


    public override void CompleteObjective() {
        switch (currentObjective) {
            case 0:
            default:
                QuestObjective.Collection c = (QuestObjective.Collection)questObjectives[0].subSectionObjectives[0];
                GameManagerScript.ins.playerInventory.RemoveItem(c.targetItem, c.targetCount);
                break;
        }
        currentObjective += 1;
        if (CanCompleteQuest()) {
            CompleteQuest();
        }
    }

    protected override void CompleteQuest() {
        GameManagerScript.ins.playerQuests.FinishQuest(this);
    }

    public override bool HasCompletedCurrentObjective() {
        switch (currentObjective) {
            case 0:
            default:
                QuestObjective.Collection c = (QuestObjective.Collection)questObjectives[0].subSectionObjectives[0];
                if (GameManagerScript.ins.playerInventory.HasItemCountInInventory(c.targetItem, c.targetCount)) {
                    return true;
                }
                return false;
        }

    }
}
