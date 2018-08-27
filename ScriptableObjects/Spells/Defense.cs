using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : Spell {

    public float duration; //in seconds
    public float strength; //how much health it has
    public bool canBlockProjectiles; //true if the wall blocks them, false is not
    public bool canWalkThrough; //true if characters can move through, false is not. Even if true, may still take damage
    public float walkThroughDamage; //damage taken if the character can walk through and takes damage. set to -1 if none

    protected void Start() {
        genre = Spell.Genre.Defense;
    }

}
