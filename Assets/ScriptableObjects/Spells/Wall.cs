using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Wall Spell", menuName = "Spell: Wall")]
public class Wall : Defense {

    public float distance; //max distance the player can summon the wall
    public float length; //length from the center of the wall

    new void Start() {
        base.Start();
        type = Spell.Type.Wall;
    }
}
