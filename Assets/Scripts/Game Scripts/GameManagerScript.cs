using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript ins;
    public GameObject player;
    public PlayerInfo playerInfo;
    public Inventory playerInventory;
    public SpellInventory playerSpells;
    public QuestInventory playerQuests;

    //TAGS FOR SPELLS
    public string projectileTag = "Projectile";
    public string wallTag = "Wall";

    //KEYS FOR INPUT
    private KeyCode openMenu = KeyCode.Tab; public KeyCode OpenMenu { get { return openMenu; } }
    private KeyCode toggleFireMode = KeyCode.Q; public KeyCode ToggleFireMode {  get { return toggleFireMode; } }
    private KeyCode centerCamera = KeyCode.Home; public KeyCode CenterCamera { get { return centerCamera; } }
    private KeyCode unlockCamera = KeyCode.LeftAlt; public KeyCode UnlockCamera { get { return unlockCamera; } }
    private KeyCode moveLeft = KeyCode.A; public KeyCode MoveLeft { get { return moveLeft; } }
    private KeyCode moveRight = KeyCode.D; public KeyCode MoveRight { get { return moveRight; } }
    private KeyCode moveDown= KeyCode.S; public KeyCode MoveDown { get { return moveDown; } }
    private KeyCode moveUp = KeyCode.W; public KeyCode MoveUp { get { return moveUp; } }




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
        playerQuests = player.GetComponent<QuestInventory>();
	}


    public void QuitGame() {
        Application.Quit();
    }

    public Vector3 GetPlayerFeetPosition() {
        return new Vector3(player.transform.position.x, player.transform.position.y - 0.45f, 0);
    }
	
}
