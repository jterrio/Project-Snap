using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject {

    public float energyToCast; //energy need to cast the spell; bigger spells are more exhausting
    public SpellMaterial[] spellMaterial; //what the spell is made of. most are of size 1, so just use spellMaterial[0], but some are mixed, so they have multiple.
    public Type type;
    public Genre genre;
    public int ID;
    public Sprite icon;
    public Sprite sprite;
    public float castTime; //time before spell uses
    public float castSpeed; //percentage that speed get reduced to. Leave at 1 for no reduction, 0.5 for half, and 0 for no movement
    public GameObject spellObject; //holds the script

    public enum SpellMaterial {
        Air,
        Water,
        Earth,
        Fire,
        Mana
    }

    public enum Genre {
        Damage,
        Defense,
        Enhance
    }


    public enum Type {
        Projectile,
        Beam,
        Wall,
        Ring,
        Enhancement,
        Utility,
        Forbidden
    }
    


}
