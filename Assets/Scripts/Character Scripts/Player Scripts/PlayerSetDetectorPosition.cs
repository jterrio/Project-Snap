using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetDetectorPosition : MonoBehaviour {

    private PlayerInfo info;

    // Use this for initialization
    void Start () {
        info = transform.parent.GetComponent<PlayerInfo>();
    }
	
	// Update is called once per frame
	void Update () {
        SetPosition();
    }

    //
    void SetPosition() {
        //move the detector to the correct angle based on which direction we are facing
        //might need improvement, such as 45 * int of direction
        switch (info.direction) {
            case (CharacterInfo.Direction.FRONT):
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case (CharacterInfo.Direction.FRONTRIGHT):
                transform.eulerAngles = new Vector3(0, 0, 45);
                break;
            case (CharacterInfo.Direction.RIGHT):
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case (CharacterInfo.Direction.BACKRIGHT):
                transform.eulerAngles = new Vector3(0, 0, 135);
                break;
            case (CharacterInfo.Direction.BACK):
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case (CharacterInfo.Direction.BACKLEFT):
                transform.eulerAngles = new Vector3(0, 0, 225);
                break;
            case (CharacterInfo.Direction.LEFT):
                transform.eulerAngles = new Vector3(0, 0, 270);
                break;
            case (CharacterInfo.Direction.FRONTLEFT):
                transform.eulerAngles = new Vector3(0, 0, 315);
                break;
        }
    }


}
