using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class QuestManagerScript : MonoBehaviour {

    public static QuestManagerScript ins;
    public List<Quest> allQuests;
    public List<Quest> existingQuests;

    // Use this for initialization
    void Start() {
        //singelton
        if (ins == null) {
            ins = this;
        } else {
            if (ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        AddAllQuests();
    }

    [System.Serializable]
    public class PlayerQuestData {

        [XmlArray("CompletedQuests")]
        [XmlArrayItem("Quest")]
        public List<QuestData> completedQuests;

        [XmlArray("InProgressQuests")]
        [XmlArrayItem("Quest")]
        public List<QuestData> inProgressQuests;
    }

    [System.Serializable]
    public class QuestData {
        public int ID;
        public int currentObjective;
    }

    public PlayerQuestData SavePlayerQuestData() {
        PlayerQuestData pqd = new PlayerQuestData();
        pqd.inProgressQuests = new List<QuestData>();
        pqd.completedQuests = new List<QuestData>();
        foreach(Quest q in GameManagerScript.ins.playerQuests.completedQuests) {
            QuestData qd = new QuestData();
            qd.ID = q.questID;
            qd.currentObjective = q.currentObjective;
            pqd.completedQuests.Add(qd);
        }
        foreach (Quest q in GameManagerScript.ins.playerQuests.inProgressQuests) {
            QuestData qd = new QuestData();
            qd.ID = q.questID;
            qd.currentObjective = q.currentObjective;
            pqd.inProgressQuests.Add(qd);
        }

        return pqd;
    }

    public void LoadPlayerQuestData(PlayerQuestData pqd) {
        GameManagerScript.ins.playerQuests.completedQuests.Clear();
        GameManagerScript.ins.playerQuests.inProgressQuests.Clear();
        DestroyAllExistingQuests();
        foreach (QuestData qd in pqd.completedQuests) {
            Quest q = CreateQuestFromID(qd.ID);
            q.currentObjective = qd.currentObjective;
            GameManagerScript.ins.playerQuests.completedQuests.Add(q);
        }
        foreach (QuestData qd in pqd.inProgressQuests) {
            Quest q = CreateQuestFromID(qd.ID);
            q.currentObjective = qd.currentObjective;
            GameManagerScript.ins.playerQuests.inProgressQuests.Add(q);
        }
    }

    

    public Quest CreateQuestFromID(int i) {
        foreach(Quest q in allQuests) {
            if(q.questID == i) {
                Quest quest = Instantiate(q);
                quest.gameObject.transform.SetParent(this.gameObject.transform);
                existingQuests.Add(quest);
                return quest;
            }
        }
        return allQuests[0];
    }

    public void DestroyAllExistingQuests() {
        foreach(Quest q in new List<Quest>(existingQuests)) {
            existingQuests.Remove(q);
            Destroy(q.gameObject);
        }
    }

    public Quest GetExistingQuestFromID(int i) {
        foreach(Quest q in existingQuests) {
            if(q.questID == i) {
                return q;
            }
        }
        return existingQuests[0];
    }

    public bool DoesQuestExist(int i) {
        foreach(Quest q in existingQuests) {
            if(q.questID == i) {
                return true;
            }
        }
        return false;
    }
	
    public void AddAllQuests() {
        allQuests = new List<Quest>(Resources.LoadAll<Quest>("Quests"));
    }


    //
    // ESCORT
    //

    /// <summary>
    /// Spawns an enemy wave
    /// </summary>
    /// <param name="spawnList">Enemies to spawn</param>
    /// <param name="targetEscort">Target to spawn around</param>
    /// <param name="distanceFromRouteToSpawn">Distance to spawn them from the target</param>
    /// <param name="spawnTime">Internal timer for the spawns</param>
    /// <param name="timeBetweenEnemySpawn">Time between enemy spawns</param>
    /// <returns>Time to set internal timer to</returns>
    public void SpawnEnemyWave(List<GameObject> spawnList, GameObject targetEscort, float distanceFromRouteToSpawn, ref float spawnTime, float timeBetweenEnemySpawn) {
        if (CanSpawnEnemy(spawnTime)) {
            int TOTAL_CHANCE = 0;
            int TEMP_CHANCE = 0;
            GameObject spawnNPC = null;
            //get random npc
            foreach (GameObject e in spawnList) {
                TOTAL_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance; //get the total spawn chance so we can calculate which npc to spawn
            }
            int i = Random.Range(1, TOTAL_CHANCE);
            foreach (GameObject e in spawnList) { //check each spawn chance and get the npc we are spawning
                TEMP_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance;
                if (i <= TEMP_CHANCE) {
                    spawnNPC = e;
                    break;
                }
            }
            //get random location
            Vector2 pos = RandomPointOnUnitCircle(distanceFromRouteToSpawn);
            GameObject s = Instantiate(spawnNPC);
            s.transform.position = pos + new Vector2(targetEscort.transform.position.x, targetEscort.transform.position.y);

            //reset spawn timer
            spawnTime = Time.time + timeBetweenEnemySpawn;
        }
    }

    Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;

        return new Vector2(x, y);

    }

    public bool CanSpawnEnemy(float spawnTime) {
        if (Time.time >= spawnTime) {
            return true;
        }
        return false;
    }

    public void MoveToFirstPoint(GameObject targetEscort, GameObject[] positionsInRoute, int currentRoute) {
        targetEscort.GetComponent<NPCInfo>().polyNav.SetDestination(positionsInRoute[currentRoute].transform.position);
    }

    public bool hasReachedDestinition(GameObject targetEscort, GameObject[] positionsInRoute, int currentRoute) {
        if (targetEscort.transform.position == positionsInRoute[currentRoute].transform.position) {
            return true;
        }
        return false;
    }

    public bool hasFinishedRoute(GameObject targetEscort, GameObject[] positionsInRoute, int currentRoute) {
        if (hasReachedDestinition(targetEscort, positionsInRoute, currentRoute) && (currentRoute == (positionsInRoute.Length - 1))) {
            return true;
        }
        return false;
    }

    public void MoveToNextPoint(GameObject targetEscort, GameObject[] positionsInRoute, ref int currentRoute) {
        currentRoute += 1;
        targetEscort.GetComponent<NPCInfo>().polyNav.SetDestination(positionsInRoute[currentRoute].transform.position);
    }

    public void SetWaitTime(ref float waitTime, float timeBetweenPointsInRoute) {
        waitTime = Time.time + timeBetweenPointsInRoute;
    }

    public bool CanMoveToNextPoint(float waitTime) {
        if (Time.time >= waitTime) {
            return true;
        }
        return false;
    }

    public void SetNPCValues(GameObject targetEscort, float walkSpeed) {
        targetEscort.GetComponent<NPCInfo>().polyNav.maxSpeed = walkSpeed; //set agents speed
    }

    public bool IsPlayerTooFar(GameObject targetEscort, GameObject[] positionsInRoute, int currentRoute, float followDistance) {
        if (Vector2.Distance(GameManagerScript.ins.player.transform.position, targetEscort.transform.position) > followDistance) {
            return true;
        }
        return false;
    }

    public void DoEscortRoute(GameObject targetEscort, GameObject[] positionsInRoute, int currentRoute, float walkSpeed) {
        SetNPCValues(targetEscort, walkSpeed);
        MoveToFirstPoint(targetEscort, positionsInRoute, currentRoute);
    }

    //
    // ELIMINATION
    //

    public bool IsElimTarget(GameObject test, GameObject targetNPC) {
        if (test.GetComponent<NPCInfo>().npc == targetNPC.GetComponent<NPCInfo>().npc) {
            return true;
        }
        return false;
    }

    public void AddKill(ref int currentCount) {
        currentCount += 1;
    }

    public bool HasKilledEnough(int currentCount, int targetCount) {
        if (currentCount >= targetCount) {
            return true;
        }
        return false;
    }

    //
    // DELIVERY
    //

    //add the key item to the players inventory
    public void GivePlayerItem(Item deliveryItem) {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.AddItem(deliveryItem);
    }


    //remove the item from their inventory
    public void RemovePlayerItem(Item deliveryItem) {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.RemoveAllItem(deliveryItem);
    }

    //check to see if the key item is in the player's inventory
    public bool IsKeyItemInInventory(Item deliveryItem) {
        return GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.HasItemInInventory(deliveryItem);
    }

    //check to see if the target npc is the npc for the quest
    public bool HasReachedDeliveryTarget(NPC target, NPC deliveryTarget) {
        if (target == deliveryTarget) {
            return true;
        }
        return false;
    }

    //
    // DEFEND
    //

    public void SpawnDefendEnemyWave(List<GameObject[]> spawnList, GameObject[] enemySpawnLocations, int currentWave, ref float spawnTime, float timeBetweenEnemySpawn) {
        if (CanSpawnEnemy(spawnTime)) {
            int TOTAL_CHANCE = 0;
            int TEMP_CHANCE = 0;
            GameObject spawnNPC = null;
            //get random npc
            foreach (GameObject e in spawnList[currentWave]) {
                TOTAL_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance; //get the total spawn chance so we can calculate which npc to spawn
            }
            int i = Random.Range(1, TOTAL_CHANCE);
            foreach (GameObject e in spawnList[currentWave]) { //check each spawn chance and get the npc we are spawning
                TEMP_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance;
                if (i <= TEMP_CHANCE) {
                    spawnNPC = e;
                    break;
                }
            }
            //get random location
            int u = Random.Range(0, enemySpawnLocations.Length - 1);
            GameObject s = Instantiate(spawnNPC);
            s.transform.position = enemySpawnLocations[u].transform.position;

            //reset spawn timer
            spawnTime = Time.time + timeBetweenEnemySpawn;
        }
    }

    //move the wave up one, assuming we checked beforehand that we can progress and the waves are not complete
    public void SetNextWave(ref int currentWave, float breakTime) {
        if (IsBreakTimeOver(breakTime)) {
            currentWave += 1;
        }
    }

    //check to see if breaktime is over so we can begin a new wave
    public bool IsBreakTimeOver(float breakTime) {
        if (Time.time >= breakTime) {
            return true;
        }
        return false;
    }

    //set the breaktime for when a wave is complete
    public void SetBreakTime(ref float breakTime, float timeBetweenWaves) {
        breakTime = Time.time + timeBetweenWaves;
    }

    //check to see if we have gone through all the waves
    public bool AreWavesOver(int currentWave, int waves) {
        if (currentWave >= waves) {
            return true;
        }
        return false;
    }

    //check to see if the current wave is over or is still continuing
    public bool IsWaveOver(float finishTime) {
        if (Time.time >= finishTime) {
            return true;
        }
        return false;
    }

    //set the time for the wave
    public void SetTime(ref float finishTime, float timeToDefend) {
        finishTime = Time.time + timeToDefend;
    }

    //
    // COLLECTION
    //

    public void RemoveItemsFromPlayerInventory(Item targetItem, int targetCount) {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.RemoveItem(targetItem, targetCount);
    }

    public bool DoesPlayerHaveItems(Item targetItem, int targetCount) {
        return GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.HasItemCountInInventory(targetItem, targetCount);
    }

    //
    // ASSAULT
    //
}
