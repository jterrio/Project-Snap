using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour {

    public List<GameObject> charactersInCombat; //list of all characters participating, actively
    public List<GameObject> currentCharactersTurn; //list of characters who are currently turning
    public List<GameObject> charactersChecked; //characters check to join combat
    public List<GameObject> charactersToCheck; //characters queued to check to join combat
    private bool combatStarted = false; //if the first turn has started yet
    public TurnStage turnStage; //turn of stage - MAY BE LEGACY AND NOT USED

    public EdgeCollider2D combatBounds; // Never used to my knowlage -Ryan
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, CombatManager.ins.joinDistance, CombatManager.ins.characterTest); //player is 9, npc is 8
        CheckCollider(colliders);
        //CheckCharacters();
        CombatManager.ins.allCombatHandlers.Add(this.gameObject);
        StartWaiting();
        combatStarted = true;

	}

    void Update() {
        RemoveDeadCharacters();
        RemoveFarCharacters();
        if (CanEndCombat()) {
            print("Combat's over boys!");
            EndCombat();
        }
        if (turnCoroutine == null && Time.timeScale != 0f) { //if time is moving and turns have just ended, start turn checks
            turnCoroutine = StartCoroutine(TurnCheck());
        } else if (Time.timeScale == 0f) { //if time is frozen, it is someone's turn and we need to do end turn check
            EndTurnCheck();
        }
    }

    /// <summary>
    /// DEBUG FUNCTION TO END ALL TURNS
    /// </summary>
    public void ManualEndTurn() {
        foreach(GameObject c in currentCharactersTurn) {
            c.GetComponent<CombatScript>().EndTurn();
        }
    }

    /// <summary>
    /// Start of combat function to begin the turn process for each character in combat
    /// </summary>
    void StartWaiting() {
        foreach(GameObject c in charactersInCombat) {
            StartWaiting(c);
        }
    }

    /// <summary>
    /// Begins combat for a single characer; used for joining late
    /// </summary>
    /// <param name="c"></param>
    void StartWaiting(GameObject c) {
        c.GetComponent<CombatScript>().StartWaiting();
    }

    /// <summary>
    /// Remove null (Dead) game object from characters in combat
    /// </summary>
    void RemoveDeadCharacters() {
        foreach(GameObject c in new List<GameObject>(charactersInCombat)) {
            if(c == null) {
                charactersInCombat.Remove(c);
            }
        }
    }

    /// <summary>
    /// remove characters that have strayed too far
    /// </summary>
    void RemoveFarCharacters() {
        List<GameObject> characters = new List<GameObject>(charactersInCombat);
        bool doRemove = true;
        foreach(GameObject self in characters) {
            foreach(GameObject c in characters) {
                if (self.GetComponent<CharacterInfo>().DoHate(c)) {
                    if(Vector3.Distance(self.transform.position, c.transform.position) < CombatManager.ins.joinDistance * 2.5) {
                        doRemove = false;
                    }
                }
            }
            if (doRemove) {
                charactersInCombat.Remove(self);
                RemoveFromCombat(self);
            }
        }
    }

    /// <summary>
    /// End combat for every character in this combat
    /// </summary>
    void EndCombat() {
        foreach(GameObject c in charactersInCombat) {
            RemoveFromCombat(c);
        }
        Time.timeScale = 1f;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Remove a specific character from combat
    /// </summary>
    /// <param name="c"></param>
    void RemoveFromCombat(GameObject c) {
        if (c == GameManagerScript.ins.player) {
            c.GetComponent<PlayerInfo>().LeaveCombat();
        } else {
            c.GetComponent<NPCInfo>().LeaveCombat();
        }
    }

    /// <summary>
    /// Checks with factionManager to see if everyone has applicable targets
    /// </summary>
    /// <returns></returns>
    bool CanEndCombat() {
        foreach(GameObject c in charactersInCombat) {
            foreach(GameObject d in charactersInCombat) {
                if (d != GameManagerScript.ins.player) {
                    if (c != GameManagerScript.ins.player) {
                        //both characters are AI
                        if (FactionManagerScript.ins.DoesDislike(c.GetComponent<NPCInfo>().faction, d.GetComponent<NPCInfo>().faction)) {
                            return false;
                        }
                    } else {
                        if (d.GetComponent<Stats>().attitude <= -30) {
                            return false;
                        }
                    }
                }
                
            }
        }


        return true;
    }

    /// <summary>
    /// check to see if a characters is ready to begin their turn
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnCheck() {
        yield return new WaitForEndOfFrame();
        RemoveDeadCharacters();
        foreach (GameObject c in charactersInCombat) {
            if (c.GetComponent<CombatScript>().isReady) {
                Time.timeScale = 0f;
                //print("Turn: " + c.gameObject.name + " at " + Time.time);
                currentCharactersTurn.Add(c);
                c.GetComponent<CombatScript>().isReady = false;
                if(c.layer == 8) { //npc, means when need to run the AI
                    c.GetComponent<CombatScript>().AI(charactersInCombat, CombatManager.ins.GetPlayerHandler() == this.gameObject);
                }
            }
        }
        turnCoroutine = null;
    }
    /// <summary>
    /// check to see if a characters is ready to end their turn
    /// </summary>
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

    /// <summary>
    /// check to see if any characters are pulling in nearby characters in for combat
    /// </summary>
    /// <param name="colliders"></param>
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

    /// <summary>
    /// check characters pulled to see if they will actually join
    /// </summary>
    void CheckCharacters() {
        foreach(GameObject n in charactersInCombat) {
            charactersChecked.Add(n);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(n.transform.position, CombatManager.ins.joinDistance, CombatManager.ins.characterTest);
            CheckCollider(colliders);
        }
    }

    public void AddLateCharacters(List<GameObject> characters, GameObject handler) {
        foreach(GameObject c in characters) {
            if (!charactersInCombat.Contains(c)) {

                if (handler.GetComponent<CombatHandler>().currentCharactersTurn.Contains(c)) {
                    AddLateCharacterTurn(c);
                } else {
                    AddLateCharacter(c);
                }
            }
        }
    }

    public void AddLateCharacter(GameObject c) {
        charactersInCombat.Add(c);
        if (!c.GetComponent<CharacterInfo>().inCombat) {
            if (c != GameManagerScript.ins.player) {
                c.GetComponent<NPCInfo>().EnterCombat();
            } else {
                c.GetComponent<PlayerInfo>().EnterCombat();
            }
            StartWaiting(c);
        }
    }

    public void AddLateCharacterTurn(GameObject c) {
        currentCharactersTurn.Add(c);
    }







}
