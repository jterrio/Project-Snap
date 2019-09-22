using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpItemScript : MonoBehaviour {

    private bool canPickUp = false;
    private bool hasPickedUp = false;
    private Item targetItem;
    public GameObject target;
	
	// Update is called once per frame
	void Update () {
        //only check code if we can reach an item
        if (!canPickUp) {
            return;
        }
        if (GameManagerScript.ins.player.GetComponent<PlayerInfo>().inCombat) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            //pick up item
            hasPickedUp = true; //set code for when ontriggerexit2d gets called // determine if inventory was full
            hasPickedUp = transform.parent.GetComponent<PlayerInfo>().inventory.CanAddItem();
            if (target != null && hasPickedUp) {
                targetItem = target.GetComponent<WorldItemScript>().item;
                //add item to inventory
                transform.parent.GetComponent<PlayerInfo>().inventory.AddItem(targetItem);
                target.SetActive(false); //delete item from world space
            }
            if (!hasPickedUp) { //if we were unable to pick up the item, return
                return;
            }
            //Display notification to player and check whether or not they have closed the prompt
            if (UIManager.ins.DisplayItemPickup(targetItem)) {
                canPickUp = false;
                targetItem = null;
                target = null;
                hasPickedUp = false;
            }
        }
	}


    private void OnTriggerEnter2D(Collider2D collision) {
        //check if it is an item
        if(collision.gameObject.tag != "Item") {
            return;
        }
        canPickUp = true;
        target = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        //check if it is an item
        if (collision.gameObject.tag != "Item" || hasPickedUp) {
            return;
        }
        canPickUp = false;
        target = null;
    }
}
