using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    public static CombatManager ins;
    public GameObject combatHandler;
    public CombatHUDLog combatHUDLog;
    public CombatHUDAttack combatHUDAttack;
    public PlayerDrawCombatMovePosition combatDrawMovePosition;
    public static GameObject playerCombatHandler; //the combat handler that the player is particpating in, if they are in combat;
    public List<GameObject> allCombatHandlers;

    public LayerMask obstacleTest;
    public LayerMask characterTest;
    public LayerMask characterVisibleTest;


    // Use this for initialization
    void Start () {
		//singleton
        if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(ins);
            }
        }
        DontDestroyOnLoad(gameObject);

	}

    void Update() {
        if(playerCombatHandler == null) {
            return; //we dont need to worry about merging handlers if the player is not in combat
        }
        CheckRange();
    }


    void CheckRange() {
        foreach (GameObject handler in allCombatHandlers) {
            if (handler == playerCombatHandler) {
                continue; //dont need to check playerCombatHandler since we are comparing everything to it
            }
            //check positions relative to each other if the player is in combat
            if (Vector3.Distance(handler.transform.position, playerCombatHandler.transform.position) <= 20) {
                playerCombatHandler.GetComponent<CombatHandler>().AddLateCharacters(handler.GetComponent<CombatHandler>().charactersInCombat);
                allCombatHandlers.Remove(handler);
                Destroy(handler);
                return;
            }
        }
    }

    public void ManualEndTurnButton() {
        GameManagerScript.ins.player.GetComponent<CombatScript>().EndTurn();
    }

    public void InitCombat(GameObject npc) {
        GameObject handler = Instantiate(combatHandler);
        handler.gameObject.transform.position = npc.transform.position; 
    }

    public void SetPlayerHandler(GameObject handler) {
        playerCombatHandler = handler;
    }

    public GameObject GetPlayerHandler() {
        return playerCombatHandler;
    }


}
