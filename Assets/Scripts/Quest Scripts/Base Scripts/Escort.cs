using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escort : Quest {

    public GameObject[] positionsInRoute; //position to move to in the escort
    private int currentRoute; //positionsInRoute index

    public float followDistance; //distance npc wants you to follow; if you exceed, they get mad
    public float walkSpeed; //speed npc moves from point to point
    public float timeBetweenPointsInRoute; //time npc waits between points; 0 is no time
    private float waitTime; //interally to see time between points

    public GameObject targetEscort; //what npc we are escorting

    public float distanceFromRouteToSpawn; //how far away the enemy shoudl spawn and then move towards the route
    public float timeBetweenEnemySpawn; //time to set for each enemy spawn
    private float spawnTime; //time used to internally check for spawn time

    public List<GameObject> spawnList; //spawnlist for the current wave

    public void DoRoute() {
        SetNPCValues();
        MoveToFirstPoint();
    }

    void SpawnEnemy() {
        if (CanSpawnEnemy()) {
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
            SetSpawnTime();
        }
    }

    Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;

        return new Vector2(x, y);

    }

    bool CanSpawnEnemy() {
        if (Time.time >= spawnTime) {
            return true;
        }
        return false;
    }

    void SetSpawnTime() {
        spawnTime = Time.time + timeBetweenEnemySpawn;
    }

    void MoveToFirstPoint() {
        targetEscort.GetComponent<NPCInfo>().polyNav.SetDestination(positionsInRoute[currentRoute].transform.position);
    }


    bool hasReachedDestinition() {
        if(targetEscort.transform.position == positionsInRoute[currentRoute].transform.position) {
            return true;
        }
        return false;
    }

    bool hasFinishedRoute() {
        if(hasReachedDestinition() && (currentRoute == (positionsInRoute.Length - 1))) {
            return true;
        }
        return false;
    }

    void MoveToNextPoint() {
        currentRoute += 1;
        targetEscort.GetComponent<NPCInfo>().polyNav.SetDestination(positionsInRoute[currentRoute].transform.position);
    }

    void SetWaitTime() {
        waitTime = Time.time + timeBetweenPointsInRoute;
    }

    bool CanMoveToNextPoint() {
        if(Time.time >= waitTime) {
            return true;
        }
        return false;
    }

    void SetNPCValues() {
        targetEscort.GetComponent<NPCInfo>().polyNav.maxSpeed = walkSpeed; //set agents speed
    }

    bool IsPlayerTooFar() {
        if(Vector2.Distance(player.transform.position, targetEscort.transform.position) > followDistance) {
            return true;
        }
        return false;
    }




}
