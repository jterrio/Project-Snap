using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Spell: Projectile")]
public class Projectile : Damage {

    public float distance; //distance the projectie will travel before ending. Put at -1 for infinite
    public float speed;
    //damage is only used when direct hit and explosion damage when caught in the blast

    new void Start() {
        base.Start();
        type = Spell.Type.Projectile;
    }

}
