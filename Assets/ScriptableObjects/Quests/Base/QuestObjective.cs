using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjective : ScriptableObject {

    [CreateAssetMenu(fileName = "New Assualt Objective", menuName = "Assualt")]
    public class Assualt : QuestObjective {
    }

    [CreateAssetMenu(fileName = "New Collection Objective", menuName = "Collection")]
    public class Collection : QuestObjective {
        public Item targetItem;
        public int targetCount;
    }

    [CreateAssetMenu(fileName = "New Defend Objective", menuName = "Defend")]
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

        [System.Serializable]
        public class Wave {
            public GameObject[] waveSpawnList;
        }

        public List<Wave> spawnList; //spawnlist for the current wave
        public GameObject[] enemySpawnLocations; //spawn locations for when the wave is in progress
    }

    [CreateAssetMenu(fileName = "New Elimination Objective", menuName = "Elimination")]
    public class Elimination : QuestObjective {
        public GameObject targetNPC;
        public int targetCount;
        public int currentCount;
    }

    [CreateAssetMenu(fileName = "New Delivery Objective", menuName = "Delivery")]
    public class Delivery : QuestObjective {
        public Item deliveryItem;
        public NPC deliveryTarget;
    }

    [CreateAssetMenu(fileName = "New Escort Objective", menuName = "Escort")]
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
}
