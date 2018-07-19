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
        if (Input.GetKeyDown(KeyCode.Space)) {
            //player has pressed space to enter the door
            transform.parent.transform.position = target.GetComponent<UseDoorScript>().destination.transform.position; //set position equal to the door exit position
            transform.parent.GetComponent<PlayerInfo>().direction = target.GetComponent<UseDoorScript>().directionToFace; //set direction from entering/exiting door
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
