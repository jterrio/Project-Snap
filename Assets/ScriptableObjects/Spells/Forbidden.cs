using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Forbidden Spell", menuName = "Spell: Forbidden")]
public class Forbidden : Spell {

    //Special spells which just need the classification: Forbidden Magic, Null Magic
    void Start() {
        genre = Spell.Genre.Enhance;
        type = Spell.Type.Forbidden;
    }

}
