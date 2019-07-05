using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAlterPath : MonoBehaviour{

    public void OnMouseDrag() {
        if (!UIManager.ins.IsSelectingInCombat()) {
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m = new Vector3(m.x, m.y, 0);
            CombatManager.ins.combatHUDLog.ChangeNode(transform.position, m);
        }
    }

}
