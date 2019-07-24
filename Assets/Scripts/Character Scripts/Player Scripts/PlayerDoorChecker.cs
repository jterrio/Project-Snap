using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorChecker : MonoBehaviour {

    private GameObject target;
    private bool canUseDoor;


    void Update() {
        //if we cant use a door, no need to check if we have hit space
        if (!canUseDoor) {
            return;
        }
        if (GameManagerScript.ins.player.GetComponent<PlayerInfo>().inCombat) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            //player has pressed space to enter the door
            UseDoorScript useDoorScript = target.GetComponent<UseDoorScript>(); //get reference
            useDoorScript.PlaySound();
            if (!useDoorScript.goesToNewScene) { //door does not go to new area, so its like a house or something
                transform.parent.transform.position = useDoorScript.destination.transform.position; //set position equal to the door exit position
                transform.parent.GetComponent<PlayerMovementScript>().SetDirection(useDoorScript.directionToFace); //set direction from entering/exiting door
            } else { //door goes to new area, meaning that we are going to a new scene

            }
            
        }



    }


    void OnTriggerEnter2D(Collider2D collision) {
        //check to see if the object is not a door, since that is what this script checks
        if(collision.gameObject.tag.ToString() != "Door") {
            return;
        }
        canUseDoor = true;
        target = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        //check to see if the object is not a door, since that is what this script checks
        if (collision.gameObject.tag.ToString() != "Door") {
            return;
        }
        canUseDoor = false;
        target = null;
    }

}
