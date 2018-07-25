using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript ins;
    public GameObject player;
    public PlayerInfo playerInfo;
    public Inventory playerInventory;

	// Use this for initialization
	void Start () {
        //singleton
		if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerInfo = player.GetComponent<PlayerInfo>();
        playerInventory = playerInfo.inventory;
	}


    public void QuitGame() {
        Application.Quit();
    }
	
}
