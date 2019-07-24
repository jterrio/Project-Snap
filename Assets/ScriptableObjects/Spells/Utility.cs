using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Utility Spell", menuName = "Spell: Utility")]
public class Utility : Enhancement {

    //These are usually special spells that allow for out of combat things to happen, such as fly or teleport.

    new void Start() {
        base.Start();
        type = Spell.Type.Enhancement;
    }
}
