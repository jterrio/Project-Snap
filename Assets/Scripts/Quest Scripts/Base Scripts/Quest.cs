using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour {

    private float[] attitudeFactionChangeOnCompletion; //attitude changes for factions
    private float attitudeNPCChangeOnCompletion; //attitude for the specific npc
    private GameObject[] questCompletionReward; //reward for when you complete the quest
    public List<QuestObjective> questObjectives;

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
}

