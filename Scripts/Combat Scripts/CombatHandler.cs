using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour {

    public List<GameObject> charactersInCombat;
    public List<GameObject> currentCharactersTurn;
    public List<GameObject> charactersJoinLate;
    public List<GameObject> charactersChecked;
    public List<GameObject> charactersToCheck;
    public int COMBAT_RANGE = 5;
    private bool combatStarted = false;
    public TurnStage turnStage;

    public EdgeCollider2D combatBounds;
    private Coroutine turnCoroutine;
    private bool isReady;

    public int AIProgressMax;
    public int AIProgress;


    public enum TurnStage {
        PRETURN,
        TURN,
        ENDTURN
    }

    // Use this for initialization
    void Start () {

        //search for players to add to combat
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, COMBAT_RANGE, (1 << 8) | (1 << 9)); //player is 9, npc is 8
        CheckCollider(colliders);
        //CheckCharacters();
        CombatManager.ins.allCombatHandlers.Add(this.gameObject);
        StartWaiting();
        combatStarted = true;

	}

    void Update() {
        if (turnCoroutine == null && Time.timeScale != 0f) {
            turnCoroutine = StartCoroutine(TurnCheck());
        } else if (Time.timeScale == 0f) {
            EndTurnCheck();
        }
    }

    public void ManualEndTurn() {
        foreach(GameObject c in currentCharactersTurn) {
            c.GetComponent<CombatScript>().EndTurn();
        }
    }


    void StartWaiting() {
        foreach(GameObject c in charactersInCombat) {
            c.GetComponent<CombatScript>().StartWaiting();
        }
    }



    IEnumerator TurnCheck() {
        yield return new WaitForEndOfFrame();
        foreach (GameObject c in charactersInCombat) {
            if (c.GetComponent<CombatScript>().isReady) {
                Time.timeScale = 0f;
                print("Turn: " + c.gameObject.name);
                currentCharactersTurn.Add(c);
                c.GetComponent<CombatScript>().isReady = false;
                if(c.layer == 8) { //npc, means when need to run the AI
                    c.GetComponent<CombatScript>().AI(charactersInCombat, CombatManager.ins.GetPlayerHandler() == this.gameObject);
                }
            }
        }
        turnCoroutine = null;
    }

    public void EndTurnCheck() {
        List<GameObject> charactersToRemove = new List<GameObject>();
        foreach(GameObject c in currentCharactersTurn) {
            if (!c.GetComponent<CombatScript>().endTurn) {
                continue;
            } else {
                //print("START WAITING: " + c);
                c.GetComponent<CombatScript>().StartWaiting();
                charactersToRemove.Add(c);
            }
        }
        if(charactersToRemove.Count > 0) {
            foreach(GameObject c in charactersToRemove) {
                currentCharactersTurn.Remove(c);
            }
        }
        if (currentCharactersTurn.Count <= 0) {
            Time.timeScale = 1f;
            return;
        }

    }


    void CheckCollider(Collider2D[] colliders) {
        foreach (Collider2D c in colliders) {
            if (charactersChecked.Contains(c.gameObject)) {
                continue;
            }
            if (c.gameObject.layer == 8) { //npc
                NPCInfo npcInfo = c.gameObject.GetComponent<NPCInfo>();
                if (!npcInfo.inCombat) {
                    //have them join combat and determine later if they will run
                    npcInfo.EnterCombat();
                    charactersInCombat.Add(c.gameObject);
                    charactersToCheck.Add(c.gameObject);
                }
            } else { //player
                PlayerInfo playerInfo = c.gameObject.GetComponent<PlayerInfo>();
                if (!playerInfo.inCombat) {
                    playerInfo.EnterCombat();
                    CombatManager.ins.SetPlayerHandler(this.gameObject);
                    charactersInCombat.Add(c.gameObject);
                    charactersToCheck.Add(c.gameObject);
                }
            }
        } //end of adding to combat
    }


    void CheckCharacters() {
        foreach(GameObject n in charactersInCombat) {
            charactersChecked.Add(n);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(n.transform.position, COMBAT_RANGE, (1 << 8) | (1 << 9));
            CheckCollider(colliders);
        }
    }

    public void AddLateCharacters(List<GameObject> characters) {
        foreach(GameObject c in characters) {
            AddLateCharacter(c);
        }
    }

    public void AddLateCharacter(GameObject c) {
        charactersJoinLate.Add(c);
    }

    public void StopTime() {

    }







}
