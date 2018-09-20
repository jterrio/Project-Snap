using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript ins;
    public GameObject player;
    public PlayerInfo playerInfo;
    public Inventory playerInventory;
    public SpellInventory playerSpells;

	// Use this for initialization
	void Awake () {
        Application.runInBackground = true;
        //singleton
		if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
        playerInfo = player.GetComponent<PlayerInfo>();
        playerInventory = playerInfo.inventory;
        playerSpells = playerInfo.spellInventory;
	}


    public void QuitGame() {
        Application.Quit();
    }

    public Vector3 GetPlayerFeetPosition() {
        return new Vector3(player.transform.position.x, player.transform.position.y - 0.45f, 0);
    }
	
}
