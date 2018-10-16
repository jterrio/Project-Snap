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

    //used by the player
    public void CastSpell(CombatHUDAttack.Attack a, GameObject caster) {
        switch (a.selectedSpell.type) {
            case Spell.Type.Projectile:
                switch (a.fireMode) {
                    case CombatHUDAttack.FireMode.FREE:
                        SpellProjectileLookUp(a.selectedSpell, caster.transform.position, a.attackDirection, caster);
                        break;
                    case CombatHUDAttack.FireMode.POINT:
                        SpellProjectileLookUp(a.selectedSpell, caster.transform.position, (a.attackPointModePoint - caster.transform.position), caster);
                        break;
                    case CombatHUDAttack.FireMode.TARGET:
                        Vector3 memoryPos = CombatManager.ins.combatHUDAttack.memory[a.attackTarget];
                        SpellProjectileLookUp(a.selectedSpell, caster.transform.position, (memoryPos - caster.transform.position), caster);
                        break;
                    default:
                        SpellProjectileLookUp(a.selectedSpell, caster.transform.position, a.attackDirection, caster);
                        break;
                }
                break;
        }
        
    }

    //used by the ai
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
