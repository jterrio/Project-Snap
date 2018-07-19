using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {

    public GameObject player;


    void Start() {
        //Camera.main.orthographicSize = (Screen.height / (1 * 32)) * 0.25f;
        //Camera.main.orthographicSize = (Screen.height / 2);

    }

    // Update is called once per frame
    void Update () {
        //follow the player
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
	}
}
