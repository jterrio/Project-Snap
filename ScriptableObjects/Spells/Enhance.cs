using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enhance : Spell {

    public int duration; //in seconds
    public GameObject target; //can be self or another npc

    protected void Start() {
        genre = Spell.Genre.Enhance;
    }
}
