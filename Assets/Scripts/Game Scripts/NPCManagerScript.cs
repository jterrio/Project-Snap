using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

public class NPCManagerScript : MonoBehaviour {

    public static NPCManagerScript ins;
    public List<NPCData> allNPCData = new List<NPCData>();

    [System.Serializable]
    public class NPCData {

        public uint id;
        public bool active;

        public float x;
        public float y;

        public int direction;
        public int state;

        public bool canMove;
        public bool inCombat;
        public int castProgress;
        public int selectedSpellID;
        public int combatState;
        public int energyState;

        public float currentHealth;
        public float currentStamina;

        public float merchantMoney;

        public bool combatCooldown;
        public float cooldown;

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

        public int currentLine;
        public int currentSet;

        public int moveType;

        public float areaPointX;
        public float areaPointY;

        public bool hasPatrol;
        public float patrolPointX;
        public float patrolPointY;

        public bool isWaiting;


        [XmlArray("Inventory")]
        [XmlArrayItem("Slot")]
        public List<InventorySlotData> inventory;
        public int maxSize;

    }

    public class InventorySlotData {
        public int id;
        public int count;
    }


    // Start is called before the first frame update
    void Start() {
        //singleton
        if (ins == null) {
            ins = this;
        } else if (ins != this) {
            Destroy(this);
        }
        //allNPCData = new List<NPCData>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {

    }

    public List<NPCData> CollectCurrentData() {
        foreach (GameObject c in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            if(c.tag.ToString() != "NPC") {
                continue;
            }

            NPCData temp = new NPCData();
            NPCInfo a = c.GetComponent<NPCInfo>();
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
            if (a.patrolPoints.Count > 0) {
                temp.hasPatrol = true;
                temp.patrolPointX = a.patrolPoints[0].transform.position.x;
                temp.patrolPointY = a.patrolPoints[0].transform.position.y;
            } else {
                temp.hasPatrol = false;
                temp.patrolPointX = 0;
                temp.patrolPointY = 0;
            }

            temp.inventory = new List<InventorySlotData>();
            foreach(Inventory.InventorySlot invSlot in a.merchantInventory.inventory) {
                InventorySlotData invSlotData = new InventorySlotData();
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
        foreach (GameObject c in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
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
                    a.direction = (CharacterInfo.Direction)data.direction;
                    a.state = (CharacterInfo.MovementState)data.state;


                    cs.ProgressI = data.castProgress;
                    if (data.selectedSpellID != -1) {
                        cs.selectedSpell = SpellManagerScript.ins.GetSpellFromID(data.selectedSpellID);
                    }
                    cs.state = (CombatScript.CombatState)data.combatState;
                    cs.energyState = (CombatScript.EnergyState)data.energyState;

                    a.id = data.id;
                    a.gameObject.SetActive(data.active);
                    a.canMove = data.canMove;
                    a.inCombat = data.inCombat;
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
                    foreach (InventorySlotData invSlotData in data.inventory) {
                        a.merchantInventory.AddItemCount(ItemManagerScript.ins.GetItemFromID(invSlotData.id), invSlotData.count);
                    }

                    a.movementType = (NPC.MovementType)data.moveType;
                    a.isWaiting = data.isWaiting;
                    a.destination = new Vector3(data.areaPointX, data.areaPointY, 0);

                    foreach (GameObject p in new List<GameObject>(a.patrolPoints)) {
                        if (p.transform.position.x != data.patrolPointX || p.transform.position.y != data.patrolPointY) {
                            a.patrolPoints.Remove(p);
                            a.patrolPoints.Add(p);
                        } else {
                            if (!a.inCombat) {
                                a.polyNav.SetDestination(a.patrolPoints[0].transform.position);
                            }
                            break;
                        }
                    }
                    if (a.isWaiting) {
                        a.polyNav.Stop();
                        a.StopCoroutine(a.waitCoroutine);
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
        foreach(GameObject c in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            if (c.tag.ToString() != "NPC" || c.tag.ToString() != "Player") {
                continue;
            }
            if(c.GetComponent<CharacterInfo>().id == i) {
                return c;
            }
        }
        return GameManagerScript.ins.player;
    }



}

