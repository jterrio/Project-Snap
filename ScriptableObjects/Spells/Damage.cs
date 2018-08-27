using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : Spell {

    public float damage; //max is 100
    public float killChance; //percentage to kill target instantly
    public bool doesExplode; //bool value for if explodes on impact
    public float explosionDamage; //explosion damage (if true) when projectile is not a direct hit. Put at -1 for no explosion.

    protected void Start() {
        genre = Spell.Genre.Damage;
    }

}
