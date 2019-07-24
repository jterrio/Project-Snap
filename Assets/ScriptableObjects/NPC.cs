using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject {

    public string characterName;
    public string archetype;
    public int characterAge;
    public bool isTalkable;
    public bool isKeyNPC;
    public bool isMerchant;
    public bool hasQuests;
    public bool canUseMagic;
    public string weaponType;
    public int spawnChance; //if the npc is an enemy, what the spawn chance will be

    public Item[] itemPool;
    public FactionManagerScript.Faction faction;
    public string description;
    public enum WeaponType {
        GUN,
        SWORD,
        GUNSWORD,
        GUNMAGE,
        MAGESWORD,
        MAGIC,
        NOTHING
    }
    public enum Integrity {
        GENUINE,
        AVERAGE,
        INGENUINE
    }
    public enum Creativity {
        CREATIVE,
        AVERAGE,
        UNCREATIVE
    }
    public enum Humor {
        JOKESTER,
        AVERAGE,
        SERIOUS,
    }
    public enum Attitude {
        Arrogant,
        AVERAGE,
        SelfDepricating
    }
    public enum Mind {
        MEATHEAD,
        AVERAGE,
        SAVVY
    }
    public enum Learning {
        BOOKWORM,
        AVERAGE,
        HANDSON
    }
    public enum Leadership {
        DIRECT,
        AVERAGE,
        UNCLEAR
    }
    public enum Commitment {
        COMMITTED,
        AVERAGE,
        PROCRASTINATOR
    }
    public enum Thinking {
        QUICKTHINKER,
        AVERAGE,
        SLOW
    }
    public enum Loyal {
        LOYAL,
        AVERAGE,
        UNLOYAL
    }
    public enum Intelligence {
        INTELLIGENT,
        AVERAGE,
        UNINTELLIGENT
    }
    public enum Travel {
        WONDERLUST,
        AVERAGE,
        SHUTIN
    }
    public enum Charisma {
        CHARISMATIC,
        AVERAGE,
        DULL
    }
    public enum Friendship {
        FRIENDLY,
        AVERAGE,
        UNFRIENDLY
    }
    public enum Sex {
        MALE,
        FEMALE
    }
    public enum Body {
        BUFF,
        Average,
        Scrawny
    }
    public enum Height {
        Tall,
        Average,
        Short
    }
    public enum Hair {
        LONGCURLY,
        SHORTSHTRAIGHT,
        SHORTCURLY,
        LONGSTRAIGTH,
        BALD,
        RECEDING,
        THINNING
    }
    public enum HairColor {
        BLONDE,
        BROWN,
        BLACK,
        GREY,
        WHITE,
        RED
    }


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
