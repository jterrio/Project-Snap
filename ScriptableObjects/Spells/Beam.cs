using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Beam Spell", menuName = "Spell: Beam")]
public class Beam : Damage {

    public float width; //width of the beam
    //damage is only used when direct hit and explosion damage when caught in the blast

    new void Start() {
        base.Start();
        type = Spell.Type.Beam;
    }

}
