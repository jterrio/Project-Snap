using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject {

    public string characterName;
    public int characterAge;
    public bool isTalkable;
    public bool isKeyNPC;
    public bool isMerchant;
    public bool hasQuests;
    public int spawnChance; //if the npc is an enemy, what the spawn chance will be

    public Item[] itemPool;
    public FactionManagerScript.Faction faction;

    /*public enum Faction {
        EMPIRE, //those allied to sasha but not in black rose and are fighting against daud
        REBELS, //daud and his crew
        BLACKROSE, //sasha and her crew
        METRODORA, //isabel and her crew
        CIVILIAN, //civilians, like random people or merchants
        GANG //expand on this (for multiple gangs)
    }*/

    public enum MovementType {
        STATIONARY, //stands still, like a merchant
        PATROL, //moves between points
        AREA //move around a point
    }

}
