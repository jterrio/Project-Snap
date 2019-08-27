using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escort : QuestObjective {

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
}
