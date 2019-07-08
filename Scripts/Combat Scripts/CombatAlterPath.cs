using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAlterPath : MonoBehaviour{

    public Vector3 newPos;

    public void OnMouseDrag() {
        if (!UIManager.ins.IsSelectingInCombat()) {
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos = new Vector3(m.x, m.y, 0);
            GameManagerScript.ins.playerInfo.polyNav.map.FindPath(transform.position, m, SendInfo);
        }
    }

    void SendInfo(Vector2[] v) {
        if (v != null) {
            CombatManager.ins.combatHUDLog.ChangeNode(transform.position, newPos);
        }
    }

}
