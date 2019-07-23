using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    public static CombatManager ins;
    public GameObject combatHandler;
    public CombatHUDLog combatHUDLog;
    public CombatHUDAttack combatHUDAttack;
    public PlayerDrawCombatMovePosition combatDrawMovePosition;
    public CombatSpeech combatSpeech;
    public static GameObject playerCombatHandler; //the combat handler that the player is particpating in, if they are in combat;
    public List<GameObject> allCombatHandlers;
    public float joinDistance = 10f;

    public LayerMask obstacleTest;
    public LayerMask characterTest;
    public LayerMask characterVisibleTest;
    public LayerMask spellTest;
    public LayerMask wallTest;


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
        RemoveHandlers(); //remove destroyed combat handlers
        CheckRange(); //see if we need to merge and player needs to join
    }


    /// <summary>
    /// Remove handlers that were destroyed, meaning that combat ended for them
    /// </summary>
    void RemoveHandlers() {
        List<GameObject> handlers = new List<GameObject>(allCombatHandlers);
        foreach(GameObject handler in handlers) {
            if(handler == null) {
                allCombatHandlers.Remove(handler);
            }
        }
    }

    /// <summary>
    /// Check range of handlers to the player's and see if they need to merge with it
    /// </summary>
    void CheckRange() {

        //Test to see if any handlers can merge into the player's handler, given that it exists
        if (playerCombatHandler != null) {
            foreach (GameObject handler in allCombatHandlers) {
                if (handler == playerCombatHandler) {
                    continue; //dont need to check playerCombatHandler since we are comparing everything to it
                }
                //check positions relative to each other if the player is in combat
                if (Vector3.Distance(handler.transform.position, playerCombatHandler.transform.position) <= joinDistance) {
                    MergeHandler(playerCombatHandler, handler);
                    break;
                }
            }
        }

        //Test to see if anyone can get pulled in
        foreach (GameObject handler in allCombatHandlers) {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, joinDistance, CombatManager.ins.characterTest);
            foreach (Collider2D c in hitColliders) {
                if (!c.GetComponent<CharacterInfo>().inCombat) {
                    if (c.gameObject == GameManagerScript.ins.player) {
                        InitCombat(GameManagerScript.ins.player);
                    } else {
                        handler.GetComponent<CombatHandler>().AddLateCharacter(c.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Merge handlers together into one big combat zone
    /// </summary>
    /// <param name="to">Handler to merge into</param>
    /// <param name="from">Handler to merge from</param>
    void MergeHandler(GameObject to, GameObject from) {
        to.GetComponent<CombatHandler>().AddLateCharacters(from.GetComponent<CombatHandler>().charactersInCombat, from);
        allCombatHandlers.Remove(from);
        Destroy(from);
    }

    public void ManualEndTurnButton() {
        GameManagerScript.ins.player.GetComponent<CombatScript>().EndTurn();
    }

    public void InitCombat(GameObject c) {
        GameObject handler = Instantiate(combatHandler);
        handler.gameObject.transform.position = c.transform.position; 
    }

    public void SetPlayerHandler(GameObject handler) {
        playerCombatHandler = handler;
    }

    public GameObject GetPlayerHandler() {
        return playerCombatHandler;
    }


}
