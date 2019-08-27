using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : QuestObjective {

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


    
}
