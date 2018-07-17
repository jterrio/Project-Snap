using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeechChecker : MonoBehaviour {

    private PlayerInfo info;
    private Ray ray;
    private RaycastHit hit;
    private bool canTalk;
    private GameObject targetNPC;
    private NPCSpeechHolder speech;
    public bool isTalking;

    // Use this for initialization
    void Start () {
        info = transform.parent.GetComponent<PlayerInfo>();
	}
	
	// Update is called once per frame
	void Update () {

        //checks to see if the player is able to talk before allowing them to hit space
        if (canTalk) {
            if (Input.GetKeyDown(KeyCode.Space)) {

                //enter talking (if we arent already) and send the info the speak managaer
                isTalking = true;
                SpeechManager.ins.Speak(transform.parent.gameObject, targetNPC);
            }
        }
    }

    
    void OnTriggerStay2D(Collider2D collision) {
        //return if collision is not with npc because that is what we are checking for
        if (collision.gameObject.tag.ToString() != "NPC") {
            return;
        }

        //check to see if we can talk, if we can, set the variables
        if (collision.gameObject.GetComponent<NPCInfo>().isTalkable) {
            speech = collision.gameObject.GetComponent<NPCSpeechHolder>();
            speech.Player = gameObject;
            speech.IsPlayerInRange = true;
            targetNPC = collision.gameObject;
            canTalk = true;
        } else {
            canTalk = false;
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision) {
        //return if collision is not with npc because that is what we are checking for
        /*might need to check to see if the npc is same one we entered with, but prob not since the detector
         * can only hit one at a time
        */
        if (collision.gameObject.tag.ToString() != "NPC") {
            return;
        }
        //reset the talking npc once we leave, incase it was premature
        if (isTalking) {
            isTalking = false;
            SpeechManager.ins.EndSpeech();
        }
        canTalk = false;
        targetNPC = null;
        speech.IsPlayerInRange = false;
        speech = null;
    }


}
