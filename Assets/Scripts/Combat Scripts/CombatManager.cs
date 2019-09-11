using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class CombatManager : MonoBehaviour {

    public static CombatManager ins;
    public GameObject combatHandler;
    public CombatHUDLog combatHUDLog;
    public CombatHUDAttack combatHUDAttack;
    public PlayerDrawCombatMovePosition combatDrawMovePosition;
    public CombatSpeech combatSpeech;
    public CombatTileMapCheck combatTileMapCheck;
    public static GameObject playerCombatHandler; //the combat handler that the player is particpating in, if they are in combat;
    public List<GameObject> allCombatHandlers;
    public float joinDistance = 10f;
    public int combatCooldownTime = 60;

    public LayerMask obstacleTest;
    public LayerMask characterTest;
    public LayerMask characterVisibleTest;
    public LayerMask spellTest;
    public LayerMask wallTest;
    public LayerMask mapTest;

    public bool isPlayerTurn = false;


    public class CombatManagerData {

        [XmlArray("CombatManagerData")]
        [XmlArrayItem("CombatHandlerData")]
        public List<CombatHandlerData> chd;

    }

    public class CombatHandlerData {

        public float posX;
        public float posY;
        public List<uint> charactersInCombat; //list of all characters participating, actively
        public List<uint> currentCharactersTurn; //list of characters who are currently turning
        public List<uint> charactersChecked; //characters check to join combat
        public List<uint> charactersToCheck; //characters queued to check to join combat
        public List<uint> charactersRemovedFromCombat; //characters removed and cannot join back

        [XmlArray("CharactersInfoInCombat")]
        [XmlArrayItem("CharacterInfo")]
        public List<CharInfoInCombat> charactersInfoInCombat;

    }

    public class CharInfoInCombat {
        public uint ID;
        public bool isReady;
        public bool endTurn;
        public float turnTimeOffset;
    }

    public CombatManagerData ExportCombatHandlerData() {
        CombatManagerData cmd = new CombatManagerData() {
            chd = new List<CombatHandlerData>()
        };
        foreach(GameObject handler in allCombatHandlers) {
            CombatHandlerData handlerData = new CombatHandlerData();
            CombatHandler ch = handler.GetComponent<CombatHandler>();

            handlerData.posX = handler.transform.position.x;
            handlerData.posY = handler.transform.position.y;
            handlerData.charactersInCombat = new List<uint>();
            handlerData.charactersInfoInCombat = new List<CharInfoInCombat>();
            foreach (GameObject c in ch.charactersInCombat) {
                CharInfoInCombat charInfo = new CharInfoInCombat();
                handlerData.charactersInCombat.Add(c.GetComponent<CharacterInfo>().id);
                charInfo.ID = c.GetComponent<CharacterInfo>().id;
                charInfo.isReady = c.GetComponent<CombatScript>().isReady;
                charInfo.endTurn = c.GetComponent<CombatScript>().endTurn;
                float a = Time.time - c.GetComponent<CombatScript>().turnTimerStart;
                if (a >= 0) {
                    charInfo.turnTimeOffset = a;
                } else {
                    charInfo.turnTimeOffset = 0;
                }
                handlerData.charactersInfoInCombat.Add(charInfo);
            }

            handlerData.currentCharactersTurn = new List<uint>();
            foreach (GameObject c in ch.currentCharactersTurn) {
                handlerData.currentCharactersTurn.Add(c.GetComponent<CharacterInfo>().id);
            }

            handlerData.charactersChecked = new List<uint>();
            foreach (GameObject c in ch.charactersChecked) {
                handlerData.charactersChecked.Add(c.GetComponent<CharacterInfo>().id);
            }

            handlerData.charactersToCheck = new List<uint>();
            foreach (GameObject c in ch.charactersToCheck) {
                handlerData.charactersToCheck.Add(c.GetComponent<CharacterInfo>().id);
            }

            handlerData.charactersRemovedFromCombat = new List<uint>();
            foreach (GameObject c in ch.charactersRemovedFromCombat) {
                handlerData.charactersRemovedFromCombat.Add(c.GetComponent<CharacterInfo>().id);
            }

            cmd.chd.Add(handlerData);
        }




        return cmd;
    }

    public void ImportCombatHandlerData(CombatManagerData cmd) {

        foreach(GameObject h in allCombatHandlers) {
            Destroy(h);
        }
        allCombatHandlers.Clear();

        foreach(CombatHandlerData chd in cmd.chd) {
            GameObject cHandler = Instantiate(combatHandler);
            CombatHandler ch = cHandler.GetComponent<CombatHandler>();

            ch.wasLoaded = true;
            cHandler.transform.position = new Vector3(chd.posX, chd.posY, 0);
            ch.charactersInCombat = new List<GameObject>();

            ch.currentCharactersTurn = new List<GameObject>();
            foreach (uint i in chd.currentCharactersTurn) {
                ch.currentCharactersTurn.Add(NPCManagerScript.ins.GetNPCInSceneFromID(i));
            }

            foreach (uint i in chd.charactersInCombat) {
                GameObject npc = NPCManagerScript.ins.GetNPCInSceneFromID(i);
                foreach(CharInfoInCombat info in chd.charactersInfoInCombat) {
                    if(info.ID == i) {
                        CombatScript cs = npc.GetComponent<CombatScript>();
                        npc.GetComponent<CombatScript>().isReady = info.isReady;
                        npc.GetComponent<CombatScript>().endTurn = info.endTurn;
                        if(cs.turnCoroutine != null) {
                            cs.StopTurnCoroutine();
                        }
                        if (!ch.currentCharactersTurn.Contains(npc)) {
                            npc.GetComponent<CombatScript>().StartWaiting(info.turnTimeOffset);
                        }
                        break;
                    }
                }
                ch.charactersInCombat.Add(npc);
            }

            ch.charactersChecked = new List<GameObject>();
            foreach (uint i in chd.charactersChecked) {
                ch.charactersChecked.Add(NPCManagerScript.ins.GetNPCInSceneFromID(i));
            }

            ch.charactersToCheck = new List<GameObject>();
            foreach (uint i in chd.charactersToCheck) {
                ch.charactersToCheck.Add(NPCManagerScript.ins.GetNPCInSceneFromID(i));
            }

            ch.charactersRemovedFromCombat = new List<GameObject>();
            foreach (uint i in chd.charactersRemovedFromCombat) {
                ch.charactersRemovedFromCombat.Add(NPCManagerScript.ins.GetNPCInSceneFromID(i));
            }


        }
    }


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
                    } else if (!c.GetComponent<NPCInfo>().combatCooldown){
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

    public void RemoveCharacter(GameObject c) {
        foreach(GameObject handler in allCombatHandlers) {
            foreach(GameObject g in new List<GameObject>(handler.GetComponent<CombatHandler>().charactersInCombat)) {
                if(c == g) {
                    handler.GetComponent<CombatHandler>().RemoveFromCombat(c);
                }
            }
        }
    }


}
