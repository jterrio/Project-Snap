using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInfo : CharacterInfo {

    [Header("NPC")]
    public NPC npc;
    [HideInInspector] private NPCSpeechHolder speech;
    private Rigidbody2D rb;
    private CombatScript cs;

    [Header("Features")]
    public Inventory merchantInventory;
    public float merchantMoney;
    public bool isTalkable; //access via NPC class
    public bool isMerchant; //access via NPC class
    [HideInInspector] public bool isKeyNPC; //access via NPC class
    [HideInInspector] public bool hasQuests; //access via NPC class
    [HideInInspector] public FactionManagerScript.Faction faction; //access via NPC class
    public NPC.MovementType movementType;
    private NPC.MovementType movementTypeOld;
    public Coroutine waitCoroutine;

    [Header("Movement - Area")]
    public GameObject areaPoint; //for the movementtype.area
    public float radiusOfArea; //radius from area to search // area will be a square
    public Vector3 destination; //target position

    [Header("Movement - Patrol")]
    public List<GameObject> patrolPoints = new List<GameObject>(); //for the movementtype.patrol

    [Header("Movement - Stationary")]
    public GameObject stationaryPoint; //for the movementtype.stationary
    public Direction stationaryDirection; //direction to face when in the stationary mode

    [Header("Movement Conditions")]
    public bool isMoving; //checks to see whether the npc is moving to destination
    public bool isWaiting; //checks to see if the npc is waiting before moving again
    public float waitTime; //the timer for waiting
    public bool isTalking; //if they are talking or not, which helps determine if they can move

    [Header("Mechanics")]
    public float runDistance; //distance required to be able to flee combat
    public float FOV = 200; //view distance on sides
    public float ViewDistance; //distance from face

    [Header("After Combat")]
    public bool combatCooldown;
    public float timeStoppedCombat;

    void Start() {
        isTalkable = npc.isTalkable;
        characterName = npc.characterName;
        characterAge = npc.characterAge;
        isKeyNPC = npc.isKeyNPC;
        isTalkable = npc.isTalkable;
        isMerchant = npc.isMerchant;
        hasQuests = npc.hasQuests;
        faction = npc.faction;
        speech = GetComponent<NPCSpeechHolder>();
        sr = GetComponent<SpriteRenderer>();
        polyNav = GetComponent<PolyNav.PolyNavAgent>();
        rb = GetComponent<Rigidbody2D>();
        cs = GetComponent<CombatScript>();
        InitPosition();

        speed = defaultSpeed;
        runSpeed = defaultRunSpeed;
    }

    public void InitPosition() {
        if (movementTypeOld == movementType) {
            return;
        }
        movementTypeOld = movementType;
        switch (movementType) {
            case NPC.MovementType.AREA:
                //
                polyNav.OnDestinationReached -= ResetStationary;
                polyNav.OnDestinationReached -= RemoveLastPatrolPointReached;
                polyNav.OnDestinationReached += GetNewAreaPoint;
                break;
            case NPC.MovementType.PATROL:
                //
                polyNav.OnDestinationReached -= GetNewAreaPoint;
                polyNav.OnDestinationReached -= ResetStationary;
                polyNav.OnDestinationReached += RemoveLastPatrolPointReached;
                break;
            case NPC.MovementType.STATIONARY:
                //set start direction
                ResetStationary();
                polyNav.OnDestinationReached -= GetNewAreaPoint;
                polyNav.OnDestinationReached -= RemoveLastPatrolPointReached;
                polyNav.OnDestinationReached += ResetStationary;
                break;
        }
    }

    void Update() {
        //DO MOVEMENT FOR THE NPC BASED ON THE TYPE THEY ARE SET TO
        if (!inCombat) {
            bool canContinue = true;
            if (combatCooldown) {
                if(Time.time < CombatManager.ins.combatCooldownTime + timeStoppedCombat) {
                    canContinue = false;
                } else {
                    combatCooldown = false;
                }
            }
            if (canContinue) {
                switch (movementType) {
                    case NPC.MovementType.AREA:
                        AreaMovement();

                        break;
                    case NPC.MovementType.PATROL:
                        PatrolMovement();
                        break;
                    case NPC.MovementType.STATIONARY:
                        StationaryMovement();
                        break;
                }
            }
            SetDirection();
            SetSprite();
            InitPosition();
        }

    }

    public void SetMovementType(NPC.MovementType ms) {
        movementType = ms;
        InitPosition();
    }

   


    //movement for the area type
    void AreaMovement() {
        if(!isWaiting && !isMoving && !isTalking) {
            GetNewAreaPoint();
        }
    }

    public void EnterCombat() {
        polyNav.Stop();
        polyNav.stoppingDistance = 0f;
        inCombat = true;
        if (waitCoroutine != null) {
            StopCoroutine(waitCoroutine);
        }
    }

    public void SetStoppingDistance() {
        if (inCombat) {
            polyNav.stoppingDistance = 0f;
        } else {
            polyNav.stoppingDistance = 0.1f;
        }
    }

    public void LeaveCombat() {
        polyNav.Stop();
        polyNav.stoppingDistance = 0.1f;
        inCombat = false;
        isWaiting = false;
        isMoving = false;
        IsWalking = true;
        spellQueue.Clear();
        if(spellCastCoroutine != null) {
            StopCoroutine(spellCastCoroutine);
        }
        GetComponent<CombatScript>().AIEndCombat();
        GetComponentInChildren<FieldOfVisionScript>().NeedToCheck = true;
        combatCooldown = true;
        timeStoppedCombat = Time.time;
    }

    public void Talk() {
        isTalking = true;
        isMoving = false;
        isWaiting = false;
        polyNav.Stop();
        if(waitCoroutine != null) {
            StopCoroutine(waitCoroutine);
        }
    }

    public void StopTalk() {
        isTalking = false;
    }

    void GetNewAreaPoint() {
        isMoving = false;
        waitCoroutine = StartCoroutine(StartWaitingArea());
    }

    public IEnumerator StartWaitingArea() {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        if (!isTalking) {
            float x = Random.Range(areaPoint.transform.position.x - radiusOfArea, areaPoint.transform.position.x + radiusOfArea);
            float y = Random.Range(areaPoint.transform.position.y - radiusOfArea, areaPoint.transform.position.y + radiusOfArea);
            isMoving = true;
            polyNav.SetDestination(new Vector2(x, y));
            isWaiting = false;
            waitCoroutine = null;
        }
        //StopCoroutine(waitCoroutine);
    }

    public IEnumerator StartWaitingPatrol() {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        if (!isTalking) {
            isMoving = true;
            polyNav.SetDestination(patrolPoints[0].transform.position);
            isWaiting = false;
            waitCoroutine = null;
        }
        //StopCoroutine(waitCoroutine);
    }

    /// <summary>
    /// movement for the patrol type
    /// </summary>
    void PatrolMovement() {
        if (!isWaiting && !isMoving && !isTalking) {
            GetNextPatrolPoint();
        }
    }

    /// <summary>
    /// For when we reach the patrol point
    /// </summary>
    void RemoveLastPatrolPointReached() {
        patrolPoints.Add(patrolPoints[0]);
        patrolPoints.Remove(patrolPoints[0]);
        GetNextPatrolPoint();
    }

    /// <summary>
    /// Begin waiting
    /// </summary>
    public void GetNextPatrolPoint() {
        isMoving = false;
        waitCoroutine = StartCoroutine(StartWaitingPatrol());
    }


    /// <summary>
    /// movement for the stationary type
    /// </summary>
    void StationaryMovement() {
        if (transform.position != stationaryPoint.transform.position && !isTalking) {
            polyNav.SetDestination(stationaryPoint.transform.position);
        }
    }

    void ResetStationary() {
        if (direction != stationaryDirection) {
            direction = stationaryDirection;
            SetSprite();
        }
    }







}
