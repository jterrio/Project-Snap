using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface QuestBase {

    bool HasCompletedObjective(QuestObjective qo);
    void CompleteObjective();
    bool CanCompleteQuest();
    void CompleteQuest();

}
