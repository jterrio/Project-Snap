using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.UI;

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

        public PlayerCombatData pcd;

        public float timeScale;

    }

    public class PlayerCombatData {

        [XmlArray("PlayerCombatMovementData")]
        [XmlArrayItem("PlayerCombatMovement")]
        public List<PlayerCombatMovementData> pcmd;

        [XmlArray("PlayerCombatAttackData")]
        [XmlArrayItem("PlayerCombatAttack")]
        public List<PlayerCombatAttackData> pcad;

        [XmlArray("PlayerCombatOrderData")]
        [XmlArrayItem("PlayerCombatOrder")]
        public List<PlayerCombatOrderData> pcod;

        public PlayerCombatQueueData pcqd;
    }

    public class PlayerCombatMovementData {
        [XmlArray("PlayerCombatMovementDestination")]
        [XmlArrayItem("PlayerCombatMovementDestinationSingle")]
        public List<PlayerCombatMovementDestination> destination;
        public int hash;
    }

    public class PlayerCombatMovementDestination {
        public float posX;
        public float posY;
    }

    public class PlayerCombatAttackData {
        public int selectedSpellID;
        public bool selfCast;
        public bool isCasting;
        public CombatHUDAttack.FireMode fm;
        public int hash;
        public uint attackTarget;
        public float attackPointPosX;
        public float attackPointPosY;
        public float attackPointModePosX;
        public float attackPointModePosY;
        public float attackDirectionX;
        public float attackDirectionY;
    }

    public class PlayerCombatOrderData {
        public uint npcID;
        public CombatSpeech.Order o;
        public float standAreaPosX;
        public float standAreaPosY;
        public float watchAreaPosX;
        public float watchAreaPosY;
    }

    public class PlayerCombatQueueData {
        //attack-spell
        public int spellProgress;
        //order
        public int orderProgress;

        //attack-memory
        public List<MemoryData> memoryData;
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

    public class MemoryData {
        public uint npcID;
        public float posX;
        public float posY;
    }

    PlayerCombatData ExportPlayerCombatData() {
        PlayerCombatData playerCD = new PlayerCombatData();

        //Movement Data
        playerCD.pcmd = new List<PlayerCombatMovementData>();
        foreach(CombatHUDLog.Movement m in combatHUDLog.loggedMoves) {
            PlayerCombatMovementData playerCMD = new PlayerCombatMovementData();
            playerCMD.hash = m.hash;
            playerCMD.destination = new List<PlayerCombatMovementDestination>();
            foreach(Vector3 v in m.destination) {
                PlayerCombatMovementDestination playerCMDestination = new PlayerCombatMovementDestination();
                playerCMDestination.posX = v.x;
                playerCMDestination.posY = v.y;
                playerCMD.destination.Add(playerCMDestination);
            }
            playerCD.pcmd.Add(playerCMD);
        }
        //Attack Data - Spell Queue
        playerCD.pcad = new List<PlayerCombatAttackData>();
        foreach (CombatHUDAttack.Attack a in GameManagerScript.ins.playerInfo.spellQueue) {
            PlayerCombatAttackData playerCAD = new PlayerCombatAttackData();
            playerCAD.selectedSpellID = a.selectedSpell.ID;
            playerCAD.selfCast = a.selfCast;
            playerCAD.isCasting = a.isCasting;
            playerCAD.fm = a.fireMode;
            playerCAD.hash = a.hash;
            if (a.attackTarget != null) {
                playerCAD.attackTarget = a.attackTarget.GetComponent<CharacterInfo>().id;
            } else {
                playerCAD.attackTarget = 0;
            }
            playerCAD.attackPointPosX = a.attackPoint.x;
            playerCAD.attackPointPosY = a.attackPoint.y;
            playerCAD.attackPointModePosX = a.attackPointModePoint.x;
            playerCAD.attackPointModePosY = a.attackPointModePoint.y;
            playerCAD.attackDirectionX = a.attackDirection.x;
            playerCAD.attackDirectionY = a.attackDirection.y;
            playerCD.pcad.Add(playerCAD);
        }
        //Attack Data - On Line
        foreach (CombatHUDAttack.Attack a in combatHUDAttack.loggedAttacks) {
            PlayerCombatAttackData playerCAD = new PlayerCombatAttackData();
            playerCAD.selectedSpellID = a.selectedSpell.ID;
            playerCAD.selfCast = a.selfCast;
            playerCAD.isCasting = a.isCasting;
            playerCAD.fm = a.fireMode;
            playerCAD.hash = a.hash;
            if (a.attackTarget != null) {
                playerCAD.attackTarget = a.attackTarget.GetComponent<CharacterInfo>().id;
            } else {
                playerCAD.attackTarget = 0;
            }
            playerCAD.attackPointPosX = a.attackPoint.x;
            playerCAD.attackPointPosY = a.attackPoint.y;
            playerCAD.attackPointModePosX = a.attackPointModePoint.x;
            playerCAD.attackPointModePosY = a.attackPointModePoint.y;
            playerCAD.attackDirectionX = a.attackDirection.x;
            playerCAD.attackDirectionY = a.attackDirection.y;
            playerCD.pcad.Add(playerCAD);
        }
        //Order Data
        playerCD.pcod = new List<PlayerCombatOrderData>();
        foreach(CombatSpeech.GivenOrder go in combatSpeech.givenOrders) {
            PlayerCombatOrderData playerCOD = new PlayerCombatOrderData();
            playerCOD.npcID = go.npc.GetComponent<CharacterInfo>().id;
            playerCOD.o = go.o;
            if (go.standArea != null) {
                playerCOD.standAreaPosX = go.standArea.transform.position.x;
                playerCOD.standAreaPosY = go.standArea.transform.position.y;
            }
            if (go.watchArea != null) {
                playerCOD.watchAreaPosX = go.watchArea.transform.position.x;
                playerCOD.watchAreaPosY = go.watchArea.transform.position.y;
            }
            playerCD.pcod.Add(playerCOD);
        }
        //Progress Data
        playerCD.pcqd = new PlayerCombatQueueData();
        playerCD.pcqd.orderProgress = combatSpeech.progress;
        playerCD.pcqd.spellProgress = GameManagerScript.ins.playerInfo.progress;
        playerCD.pcqd.memoryData = new List<MemoryData>();
        foreach(GameObject g in combatHUDAttack.memory.Keys) {
            MemoryData md = new MemoryData();
            md.npcID = g.GetComponent<CharacterInfo>().id;
            md.posX = g.transform.position.x;
            md.posY = g.transform.position.y;
            playerCD.pcqd.memoryData.Add(md);
        }

        return playerCD;
    }

    void ImportPlayerCombatData(PlayerCombatData playerCD) {

        //Import Movement Data
        combatHUDLog.RemoveAllMovement();
        foreach (PlayerCombatMovementData playerCMD in playerCD.pcmd) {
            List<Vector3> des = new List<Vector3>();
            foreach (PlayerCombatMovementDestination vec in playerCMD.destination) {
                des.Add(new Vector3(vec.posX, vec.posY, 0));
            }
            combatHUDLog.LogMovePosition(des, playerCMD.hash);
        }
        if(combatHUDLog.loggedMoves.Count == 0) {
            GameManagerScript.ins.playerInfo.polyNav.Stop();
        }

        //Import Attack Data
        combatHUDAttack.RemoveAllAttacks();
        GameManagerScript.ins.playerInfo.CancelAllSpells();
        foreach (PlayerCombatAttackData playerCAD in playerCD.pcad) {
            CombatHUDAttack.Attack a = new CombatHUDAttack.Attack();
            a.selectedSpell = SpellManagerScript.ins.GetSpellFromID(playerCAD.selectedSpellID);
            a.selfCast = playerCAD.selfCast;
            a.isCasting = playerCAD.isCasting;
            a.fireMode = playerCAD.fm;
            a.attackPoint = new Vector3(playerCAD.attackPointPosX, playerCAD.attackPointPosY, 0);
            a.attackDirection = new Vector3(playerCAD.attackDirectionX, playerCAD.attackDirectionY, 0);
            a.attackPointModePoint = new Vector3(playerCAD.attackPointModePosX, playerCAD.attackPointModePosY, 0);
            a.hash = playerCAD.hash;
            a.attackTarget = NPCManagerScript.ins.GetNPCInSceneFromID(playerCAD.attackTarget);
            combatHUDAttack.LoadAttackToLayout(a);
        }
        combatHUDAttack.DrawAttackPositions();
        combatHUDAttack.SortAttackLayout();

        //Import Order Data
        combatSpeech.RemoveAllOrders();
        foreach (PlayerCombatOrderData playerCOD in playerCD.pcod) {
            combatSpeech.ImportOrderData(playerCOD.npcID, playerCOD.o, new Vector3(playerCOD.standAreaPosX, playerCOD.standAreaPosY, 0), new Vector3(playerCOD.watchAreaPosX, playerCOD.watchAreaPosY, 0));
        }

        //Import Progress Data
        combatHUDAttack.memory.Clear();
        GameManagerScript.ins.playerInfo.progress = playerCD.pcqd.spellProgress;
        GameManagerScript.ins.playerInfo.wasLoaded = true;
        
        if(GameManagerScript.ins.playerInfo.spellQueue.Count > 0) {
            GameManagerScript.ins.playerInfo.spellQueue[0].loggedInfo.GetComponentInChildren<Image>().fillAmount = playerCD.pcqd.spellProgress / (GameManagerScript.ins.playerInfo.spellQueue[0].selectedSpell.castTime * 100);
        }
        combatSpeech.progress = playerCD.pcqd.orderProgress;
        combatSpeech.wasLoaded = true;
        foreach(MemoryData md in playerCD.pcqd.memoryData) {
            combatHUDAttack.memory.Add(NPCManagerScript.ins.GetNPCInSceneFromID(md.npcID), new Vector3(md.posX, md.posY)); 
        }

    }

    public CombatManagerData ExportCombatHandlerData() {
        CombatManagerData cmd = new CombatManagerData() {
            chd = new List<CombatHandlerData>()
        };
        cmd.pcd = ExportPlayerCombatData();
        foreach (GameObject handler in allCombatHandlers) {
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

        cmd.timeScale = Time.timeScale;


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

        ImportPlayerCombatData(cmd.pcd);
        Time.timeScale = cmd.timeScale;
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
