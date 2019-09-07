using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInventory : MonoBehaviour {

    public List<Quest> completedQuests;
    public List<Quest> inProgressQuests;


    public bool AddQuest(Quest q) {
        if (!HasFinishedQuest(q) && !IsQuestInProgress(q)) {
            inProgressQuests.Add(q);
            return true;
        }
        return false;
    }

    public bool FinishQuest(Quest q) {
        if (IsQuestInProgress(q)) {
            inProgressQuests.Remove(q);
            completedQuests.Add(q);
            return true;
        }
        return false;
    }

    public bool IsQuestInProgress(Quest q) {
        if (inProgressQuests.Contains(q)) {
            return true;
        }
        return false;
    }

    public bool HasFinishedQuest(Quest q) {
        if (completedQuests.Contains(q)) {
            return true;
        }
        return false;
    }
}
