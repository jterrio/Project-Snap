using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface QuestBase {

    bool HasCompletedCurrentObjective();
    void CompleteObjective();
    bool CanCompleteQuest();
    void CompleteQuest();

}
