using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ring Spell", menuName = "Spell: Ring")]
public class Ring : Defense {

    public float radius; //radius of the ring
    public bool isCentered; //true if the ring is centered around the player or target object
    public float distance; //if isCentered is false, this is the distance the player can summon the center of the ring. If is centered is true, set this to -1

    new void Start() {
        base.Start();
        type = Spell.Type.Ring;
    }
}
