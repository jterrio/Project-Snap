using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject {

    [Header("Character Info")]
    public string characterName;
    public string nickname;
    public Archetype archetype;
    public int characterAge;
    public string description;
    public Item[] itemPool;
    public FactionManagerScript.Faction faction;

    [Header("Attribuites")]
    public bool isTalkable;
    public bool isKeyNPC;
    public bool isMerchant;
    public bool hasQuests;
    public bool canUseMagic;
    public WeaponType weaponType;
    public Traits traits;

    [Header("Key Features")]
    public int spawnChance; //if the npc is an enemy, what the spawn chance will be

    //Also maybe hide from inspector if they are Average.

    [System.Serializable]
    public class Traits {

        [Header("Personality Traits")]
        public Integrity integrity;
        public Creativity creativity;
        public Humor humor;
        public Attitude attitude;
        public Mind mind;
        public Reliability reliability;
        public Learning learning;
        public Leadership leadership;
        public Commitment commitment;
        public Thinking thinking;
        public Loyal loyal;
        public Persuadability persuadability;
        public Intelligence intelligence;
        public Travel travel;
        public Charisma charisma;
        public Friendship friendship;


        [Header("Physical Traits")]
        public Gender gender;
        public Body body;
        public Height height;
        public Hair hair;
        public HairColor hairColor;
        public SkinTone skinTone;

    }

    public enum WeaponType {
        GUN,
        SWORD,
        GUNSWORD,
        GUNMAGE,
        MAGESWORD,
        MAGIC,
        NOTHING
    }
    public enum Magic{
        YES,
        NO
    }
    public enum Archetype{
        MageStudent,
        StudyMagicStudent,
        JockSudent,
        IntellectualStudent,
        Student,
        GovernmentPerson,
        Laborer,
        Soldier,
        Thug,
        Blacksmith,
        Merchant,
        Veteran,
        Child,
        LowerClass,
        Hero
    }

    //Personality Traits
    public enum Integrity {
        AVERAGE,
        GENUINE,
        INGENUINE
    }
    public enum Creativity {
        AVERAGE,
        CREATIVE,
        UNCREATIVE
    }
    public enum Humor {
        AVERAGE,
        JOKESTER,
        SERIOUS,
    }
    public enum Attitude {
        AVERAGE,
        ARROGANT,
        SELFDEPRICATING
    }
    public enum Mind {
        AVERAGE,
        MEATHEAD,
        SAVVY
    }
    public enum Reliability{
        AVERAGE,
        RELIABLE,
        UNRELIABLE
    }
    public enum Learning {
        AVERAGE,
        BOOKWORM,
        HANDSON
    }
    public enum Leadership {
        AVERAGE,
        DIRECT,
        UNCLEAR
    }
    public enum Commitment {
        AVERAGE,
        COMMITTED,
        PROCRASTINATOR
    }
    public enum Thinking {
        AVERAGE,
        QUICKTHINKER,
        SLOW
    }
    public enum Loyal {
        AVERAGE,
        LOYAL,
        UNLOYAL
    }
    public enum Persuadability{
        AVERAGE,
        PUSHOVER,
        STUBBORN
    }
    public enum Intelligence {
        AVERAGE,
        INTELLIGENT,
        UNINTELLIGENT
    }
    public enum Travel {
        AVERAGE,
        WONDERLUST,
        SHUTIN
    }
    public enum Charisma {
        AVERAGE,
        CHARISMATIC,
        DULL
    }
    public enum Friendship {
        AVERAGE,
        FRIENDLY,
        UNFRIENDLY
    }

    //Phsyical Traits
    public enum Gender {
        MALE,
        FEMALE
    }
    public enum Body {
        AVERAGE,
        BUFF,
        SCRAWNY
    }
    public enum Height {
        AVERAGE,
        TALL,
        SHORT
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
    public enum SkinTone{
        WHITE,
        OLIVE,
        LIGHTBROWN,
        BROWN,
        DARKBROWN,
    }

    public enum MovementType {
        STATIONARY, //stands still, like a merchant
        PATROL, //moves between points
        AREA //move around a point
    }

}
