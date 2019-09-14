using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour {

    public FactionManagerScript.FactionRelation[] attitudeFactionChangeOnCompletion; //attitude changes for factions
    public float attitudeNPCChangeOnCompletion; //attitude for the specific npc
    public int NPCID;
    public Item[] questCompletionReward; //reward for when you complete the quest
    public int questGoldReward;
    public List<QuestObjectives> questObjectives;
    public int questID;
    public int skillPointsRewards;

    public int currentObjective = 0;

    [System.Serializable]
    public class QuestObjectives {
        public List<QuestObjective> subSectionObjectives;
    }

    public enum Objectives {
        Delivery,
        Defend,
        Escort,
        Assault,
        Collection,
        Elimination
    }

    public enum QuestLine {
        None,
        MainStory,
        BlackRose,
        Susie,
        LongIsles,
        NMerva,
        SMerva,
        TripleIsles,
        Wasteland,
        Colonies,
        TheNorth
        //add faction's stories
    }

    protected bool CanCompleteQuest() {
        if ((currentObjective + 1) > questObjectives.Count) {
            return true;
        }
        return false;
    }

    public abstract void CompleteObjective();
    public abstract bool HasCompletedCurrentObjective();
    protected abstract void CompleteQuest();

}

