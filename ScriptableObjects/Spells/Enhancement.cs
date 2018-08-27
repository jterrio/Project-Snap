using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enhancement Spell", menuName = "Spell: Enhancement")]
public class Enhancement : Enhance {

    public bool canTargetOthers; //if true, every character is target. if false, only can target self

    new void Start() {
        base.Start();
        type = Spell.Type.Enhancement;
    }

}
