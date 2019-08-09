using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDragOrders : MonoBehaviour {

    public Vector3 newPos;
    private Vector3 potNew;

    public void OnMouseDrag() {
        if (CombatManager.ins.isPlayerTurn) {
            potNew = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos = new Vector3(potNew.x, potNew.y, 0);
            GameManagerScript.ins.playerInfo.polyNav.map.FindPath(transform.position, potNew, SendInfo);
        }
    }

    void SendInfo(Vector2[] v) {
        if (v != null) {
            gameObject.transform.position = potNew;
        }
    }
}
