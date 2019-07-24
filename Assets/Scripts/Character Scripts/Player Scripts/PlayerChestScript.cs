using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChestScript : MonoBehaviour {

    public bool canOpenChest = false;
    public bool inChest = false;
    public GameObject target;
	
    void Update() {
        if (!canOpenChest || inChest) {
            return;
        }
        if (GameManagerScript.ins.player.GetComponent<PlayerInfo>().inCombat) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            inChest = true;
            target.GetComponent<ChestScript>().OpenChest();
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag != "Chest") {
            return;
        }
        canOpenChest = true;
        target = collision.gameObject;

    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag != "Chest") {
            return;
        }
        canOpenChest = false;
        target = null;

    }
}
