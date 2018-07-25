using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour {

    public float[] attitudeFactionChangeOnCompletion; //attitude changes for factions
    public float attitudeNPCChangeOnCompletion; //attitude for the specific npc
    public GameObject[] questCompletionReward; //reward for when you complete the quest
    protected GameObject player;

    protected void Start() {
        player = UIManager.ins.player;
    }

    public enum QuestType {
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
