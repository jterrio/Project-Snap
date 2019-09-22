using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

public class NPCManagerScript : MonoBehaviour {

    public static NPCManagerScript ins;
    public List<NPCData> allNPCData = new List<NPCData>();
    public List<GameObject> allNPCsInScene = new List<GameObject>();

    [System.Serializable]
    public class SharedData {
        public uint id;
        public bool active;

        public float x;
        public float y;

        public int direction;
        public int state;

        public float currentHealth;
        public float currentStamina;

        public bool canMove;
        public bool inCombat;
        public int castProgress;

        public int intuition;
        public int intelligence;
        public int strength;
        public int charisma;
        public int precision;
        public int spirituality;
        public int dexterity;
        public int perception;
        public int attitude;
        public int fear;
        public int danger;


        [XmlArray("Inventory")]
        [XmlArrayItem("Slot")]
        public List<ItemManagerScript.InventorySlotData> inventory;
        public int maxSize;
    }

    [System.Serializable]
    public class PlayerData : SharedData{

        public string charName;
        public float money;
        public bool polyNavActive;

    }


    [System.Serializable]
    public class NPCData : SharedData {

        public int selectedSpellID;
        public int combatState;
        public int energyState;

        public float merchantMoney;

        public bool combatCooldown;
        public float cooldown;

        public int currentLine;
        public int currentSet;

        public int moveType;

        public float areaPointX;
        public float areaPointY;

        public bool hasPatrol;
        public float patrolPointX;
        public float patrolPointY;

        public bool isWaiting;
        public bool isMoving;



    }



    // Start is called before the first frame update
    void Awake() {
        //singleton
        if (ins == null) {
            ins = this;
        } else if (ins != this) {
            foreach(Transform child in transform) {
                child.SetParent(ins.transform);
            }
            ins.GetAllNPCsInScene();
            Destroy(this);
        }
        //allNPCData = new List<NPCData>();
        DontDestroyOnLoad(gameObject);
        GetAllNPCsInScene();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GetAllNPCsInScene() {
        allNPCsInScene.Clear();
        foreach(Transform child in transform) {
            allNPCsInScene.Add(child.gameObject);
        }
    }

    public List<NPCData> CollectCurrentData() {
        foreach (GameObject c in allNPCsInScene) {
            if(c.tag.ToString() != "NPC") {
                continue;
            }

            NPCData temp = new NPCData();
            NPCInfo a = c.GetComponent<NPCInfo>();
            foreach (NPCData npcd in allNPCData) {
                if (npcd.id == a.id) {
                    allNPCData.Remove(npcd);
                    break;
                }
            }

            Stats s = c.GetComponent<Stats>();
            NPCSpeechHolder h = c.GetComponent<NPCSpeechHolder>();
            CombatScript cs = c.GetComponent<CombatScript>();

            temp.x = c.transform.position.x;
            temp.y = c.transform.position.y;
            temp.direction = (int)a.direction;
            temp.state = (int)a.state;

            temp.id = a.id;
            temp.active = c.gameObject.activeSelf;
            temp.canMove = a.canMove;
            temp.inCombat = a.inCombat;
            if (cs.selectedSpell != null) {
                temp.selectedSpellID = cs.selectedSpell.ID;
            } else {
                temp.selectedSpellID = -1;
            }
            temp.castProgress = cs.ProgressI;
            temp.combatState = (int)cs.state;
            temp.energyState = (int)cs.energyState;
            temp.currentHealth = a.currentHealth;
            temp.currentStamina = a.currentStamina;
            temp.merchantMoney = a.merchantMoney;
            temp.combatCooldown = a.combatCooldown;
            temp.cooldown = a.timeStoppedCombat;
            temp.intuition = s.intuition;
            temp.intelligence = s.intelligence;
            temp.strength = s.strength;
            temp.charisma = s.charisma;
            temp.precision = s.precision;
            temp.dexterity = s.dexterity;
            temp.perception = s.perception;
            temp.attitude = s.attitude;
            temp.fear = s.fear;
            temp.danger = s.danger;
            temp.spirituality = s.spirituality;
            temp.currentLine = h.currentLine;
            temp.currentSet = h.currentSet;
            temp.moveType = (int)a.movementType;
            temp.areaPointX = a.destination.x;
            temp.areaPointY = a.destination.y;
            temp.isWaiting = a.isWaiting;
            temp.isMoving = a.isMoving;
            if (a.patrolPoints.Count > 0) {
                temp.hasPatrol = true;
                temp.patrolPointX = a.patrolPoints[0].transform.position.x;
                temp.patrolPointY = a.patrolPoints[0].transform.position.y;
            } else {
                temp.hasPatrol = false;
                temp.patrolPointX = 0;
                temp.patrolPointY = 0;
            }

            temp.inventory = new List<ItemManagerScript.InventorySlotData>();
            foreach(Inventory.InventorySlot invSlot in a.merchantInventory.inventory) {
                ItemManagerScript.InventorySlotData invSlotData = new ItemManagerScript.InventorySlotData();
                invSlotData.id = invSlot.item.ID;
                invSlotData.count = invSlot.count;
                temp.inventory.Add(invSlotData);
            }

            temp.maxSize = a.merchantInventory.maxSize;

            AddToData(temp);
        }
        return allNPCData;
    }

    public void LoadNPCSceneData(List<NPCData> apc) {
        allNPCData = apc;
        foreach (GameObject c in allNPCsInScene) {
            if (c.tag.ToString() != "NPC") {
                continue;
            }
            foreach (NPCData data in allNPCData) {
                if(c.GetComponent<NPCInfo>().id == data.id) {
                    NPCInfo a = c.GetComponent<NPCInfo>();
                    Stats s = c.GetComponent<Stats>();
                    NPCSpeechHolder h = c.GetComponent<NPCSpeechHolder>();
                    CombatScript cs = c.GetComponent<CombatScript>();

                    c.transform.position = new Vector3(data.x, data.y, 0);
                    a.UpdateColliders();
                    a.direction = (CharacterInfo.Direction)data.direction;
                    a.state = (CharacterInfo.MovementState)data.state;


                    cs.ProgressI = data.castProgress;
                    if (data.selectedSpellID != -1) {
                        cs.selectedSpell = SpellManagerScript.ins.GetSpellFromID(data.selectedSpellID);
                    } else {
                        if (cs.spellCoroutine != null) {
                            cs.StopCoroutine(cs.spellCoroutine);
                        }
                    }
                    cs.state = (CombatScript.CombatState)data.combatState;
                    cs.energyState = (CombatScript.EnergyState)data.energyState;

                    a.id = data.id;
                    a.gameObject.SetActive(data.active);
                    a.canMove = data.canMove;
                    if (a.inCombat) {
                        cs.AIEndCombat();
                    } else {
                        a.EnterCombat();
                    }
                    a.inCombat = data.inCombat;
                    a.SetStoppingDistance();
                    a.currentHealth = data.currentHealth;
                    a.currentStamina = data.currentStamina;
                    a.merchantMoney = data.merchantMoney;
                    a.combatCooldown = data.combatCooldown;
                    a.timeStoppedCombat = data.cooldown;

                    s.intuition = data.intuition;
                    s.intelligence = data.intelligence;
                    s.strength = data.strength;
                    s.charisma = data.charisma;
                    s.precision = data.precision;
                    s.dexterity = data.dexterity;
                    s.perception = data.perception;
                    s.attitude = data.attitude;
                    s.fear = data.fear;
                    s.danger = data.danger;
                    s.spirituality = data.spirituality;

                    h.currentLine = data.currentLine;
                    h.currentSet = data.currentSet;

                    a.merchantInventory.maxSize = data.maxSize;

                    a.merchantInventory.ClearInventory();
                    foreach (ItemManagerScript.InventorySlotData invSlotData in data.inventory) {
                        a.merchantInventory.AddItemCount(ItemManagerScript.ins.GetItemFromID(invSlotData.id), invSlotData.count);
                    }

                    a.movementType = (NPC.MovementType)data.moveType;

                    a.destination = new Vector3(data.areaPointX, data.areaPointY, 0);

                    foreach (GameObject p in new List<GameObject>(a.patrolPoints)) {
                        if (p.transform.position.x != data.patrolPointX || p.transform.position.y != data.patrolPointY) {
                            a.patrolPoints.Remove(p);
                            a.patrolPoints.Add(p);
                        } else {
                            if (!a.inCombat) {
                                if (a.movementType == NPC.MovementType.PATROL) {
                                    a.polyNav.SetDestination(a.patrolPoints[0].transform.position);
                                }
                            }
                            break;
                        }
                    }
                    if (a.isWaiting) {
                        //a.polyNav.Stop();
                        if(a.waitCoroutine != null) {
                            a.StopCoroutine(a.waitCoroutine);
                        }
                    }
                    a.isWaiting = data.isWaiting;
                    a.isMoving = data.isMoving;
                    if (a.isWaiting) {
                        a.polyNav.Stop();
                        switch (a.movementType) {
                            case NPC.MovementType.AREA:
                                a.waitCoroutine = a.StartCoroutine(a.StartWaitingArea());
                                break;
                            case NPC.MovementType.PATROL:
                                a.waitCoroutine = a.StartCoroutine(a.StartWaitingPatrol());
                                break;
                            default:
                                break;
                        }

                    }
                    break;
                }
            }
        }
    }

    void AddToData(NPCData data) {
        foreach(NPCData npcData in allNPCData) {
            if(npcData.id == data.id) {
                allNPCData.Remove(npcData);
                break;
            }
        }
        allNPCData.Add(data);
    }

    public GameObject GetNPCInSceneFromID(uint i) {
        foreach(GameObject c in allNPCsInScene) {
            if ((c.tag.ToString() == "NPC" || c.tag.ToString() == "Player")) {
                if (c.GetComponent<CharacterInfo>().id == i) {
                    return c;
                }
            }
        }
        return GameManagerScript.ins.player;
    }

    public PlayerData GetPlayerData() {
        PlayerData pd = new PlayerData();
        PlayerInfo a = GameManagerScript.ins.playerInfo;
        Stats s = GameManagerScript.ins.playerInfo.stats;

        pd.charName = a.characterName;
        pd.x = GameManagerScript.ins.player.transform.position.x;
        pd.y = GameManagerScript.ins.player.transform.position.y;
        pd.direction = (int)a.direction;
        pd.state = (int)a.state;

        pd.id = a.id;
        pd.active = GameManagerScript.ins.player.activeSelf;
        pd.canMove = a.canMove;
        pd.inCombat = a.inCombat;

        pd.castProgress = a.progress;
        pd.currentHealth = a.currentHealth;
        pd.currentStamina = a.currentStamina;
        pd.money = a.money;
        pd.polyNavActive = a.polyNav.isActiveAndEnabled;

        pd.intuition = s.intuition;
        pd.intelligence = s.intelligence;
        pd.strength = s.strength;
        pd.charisma = s.charisma;
        pd.precision = s.precision;
        pd.dexterity = s.dexterity;
        pd.perception = s.perception;
        pd.attitude = s.attitude;
        pd.fear = s.fear;
        pd.danger = s.danger;
        pd.spirituality = s.spirituality;

        pd.inventory = new List<ItemManagerScript.InventorySlotData>();
        foreach (Inventory.InventorySlot invSlot in a.inventory.inventory) {
            ItemManagerScript.InventorySlotData invSlotData = new ItemManagerScript.InventorySlotData();
            invSlotData.id = invSlot.item.ID;
            invSlotData.count = invSlot.count;
            pd.inventory.Add(invSlotData);
        }

        pd.maxSize = a.inventory.maxSize;

        return pd;
    }

    public void LoadPlayerData(PlayerData pd) {
        PlayerInfo a = GameManagerScript.ins.playerInfo;
        Stats s = GameManagerScript.ins.playerInfo.stats;

        GameManagerScript.ins.player.transform.position = new Vector3(pd.x, pd.y, 0);
        a.UpdateColliders();
        a.characterName = pd.charName;
        a.direction = (CharacterInfo.Direction)pd.direction;
        a.state = (CharacterInfo.MovementState)pd.state;
        a.id = pd.id;
        GameManagerScript.ins.player.SetActive(pd.active);
        a.canMove = pd.canMove;
        a.inCombat = pd.inCombat;

        a.progress = pd.castProgress;
        a.currentHealth = pd.currentHealth;
        a.currentStamina = pd.currentStamina;
        a.money = pd.money;
        a.polyNav.enabled = pd.polyNavActive;

        s.intuition = pd.intuition;
        s.intelligence = pd.intelligence;
        s.strength = pd.strength;
        s.charisma = pd.charisma;
        s.precision = pd.precision;
        s.dexterity = pd.dexterity;
        s.perception = pd.perception;
        s.attitude = pd.attitude;
        s.fear = pd.fear;
        s.danger = pd.danger;
        s.spirituality = pd.spirituality;

        a.inventory.maxSize = pd.maxSize;

        a.inventory.ClearInventory();
        foreach (ItemManagerScript.InventorySlotData invSlotData in pd.inventory) {
            a.inventory.AddItemCount(ItemManagerScript.ins.GetItemFromID(invSlotData.id), invSlotData.count);
        }
    }

}

