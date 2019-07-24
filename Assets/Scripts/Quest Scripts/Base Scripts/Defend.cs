using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Quest {

    public GameObject defendArea; //area of defend
    public float timeToDefend; //time to set for each wave
    private float finishTime; //time used internally for the wave to end and stop spawning enemies
    public int waves; //number of waves
    private int currentWave; //current wave we are fighting
    public float timeBetweenWaves; //time to set for each break
    private float breakTime; //time used internally to check breaktime
    public float timeBetweenEnemySpawn; //time to set for each enemy spawn
    private float spawnTime; //time used to internally check for spawn time

    public List<GameObject[]> spawnList; //spawnlist for the current wave
    public GameObject[] enemySpawnLocations; //spawn locations for when the wave is in progress

    void SpawnEnemy() {
        if (CanSpawnEnemy()) {
            int TOTAL_CHANCE = 0;
            int TEMP_CHANCE = 0;
            GameObject spawnNPC = null;
            //get random npc
            foreach(GameObject e in spawnList[currentWave]) {
                TOTAL_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance; //get the total spawn chance so we can calculate which npc to spawn
            }
            int i = Random.Range(1, TOTAL_CHANCE);
            foreach(GameObject e in spawnList[currentWave]) { //check each spawn chance and get the npc we are spawning
                TEMP_CHANCE += e.GetComponent<NPCInfo>().npc.spawnChance;
                if(i <= TEMP_CHANCE) {
                    spawnNPC = e;
                    break;
                }
            }
            //get random location
            int u = Random.Range(0, enemySpawnLocations.Length - 1);
            GameObject s = Instantiate(spawnNPC);
            s.transform.position = enemySpawnLocations[u].transform.position;

            //reset spawn timer
            SetSpawnTime();
        }
    }

    //move the wave up one, assuming we checked beforehand that we can progress and the waves are not complete
    void SetNextWave() {
        if (IsBreakTimeOver()) {
            currentWave += 1;
        }
    }

    bool CanSpawnEnemy() {
        if(Time.time >= spawnTime) {
            return true;
        }
        return false;
    }

    void SetSpawnTime() {
        spawnTime = Time.time + timeBetweenEnemySpawn;
    }

    //check to see if breaktime is over so we can begin a new wave
    bool IsBreakTimeOver() {
        if(Time.time >= breakTime) {
            return true;
        }
        return false;
    }

    //set the breaktime for when a wave is complete
    void SetBreakTime() {
        breakTime = Time.time + timeBetweenWaves;
    }

    //check to see if we have gone through all the waves
    bool AreWavesOver() {
        if(currentWave >= waves) {
            return true;
        }
        return false;
    }

    //check to see if the current wave is over or is still continuing
    bool IsWaveOver() {
        if(Time.time >= finishTime) {
            return true;
        }
        return false;
    }

    //set the time for the wave
    void SetTime() {
        finishTime = Time.time + timeToDefend;
    }

}
