using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTileMapCheck : MonoBehaviour {


    public bool IsInMap(Vector3 origin, Vector3 dest) {
        RaycastHit2D hit = Physics2D.Raycast(dest, Vector2.zero, Mathf.Infinity, CombatManager.ins.mapTest);
        if(hit.collider != null) {
            return true;
        }
        return false;
    }

}
