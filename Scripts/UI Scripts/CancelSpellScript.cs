using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelSpellScript : MonoBehaviour {

    public CombatHUDAttack.Attack parent;

	public void CancelSpell() {
        GameManagerScript.ins.playerInfo.CancelSpell(parent);
    }

}
