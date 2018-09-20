using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManagerScript : MonoBehaviour {

    public static SpellManagerScript ins;

	// Use this for initialization
	void Start () {

        //singleton
		if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
	}

    public void CastSpell(CombatHUDAttack.Attack a, GameObject caster) {
        switch (a.selectedSpell.type) {
            case Spell.Type.Projectile:
                SpellProjectileLookUp(a.selectedSpell, caster.transform.position, a.attackDirection, caster);
                break;
        }
        
    }

    public void CastSpell(Spell spell, Vector3 location, Vector3 dir, GameObject caster) {
        switch (spell.type) {
            case Spell.Type.Projectile:
                SpellProjectileLookUp(spell, location, dir, caster);
                break;
        }


    }



    void SpellProjectileLookUp(Spell spell, Vector3 location, Vector3 dir, GameObject caster) {
        switch (spell.ID) {
            case 0:
                GameObject s = Instantiate(spell.spellObject);
                s.GetComponent<BasicFireballScript>().SetSpell(spell, location, dir, caster);
                return;


            
        }
    }



}
