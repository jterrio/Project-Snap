using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour {

    private NPCInfo npcInfo; //ai info
    private Coroutine turnCoroutine; //ai's turn coroutine
    public Stats myStats; //ai stats

    public bool isReady = false; //ready means that combat handler should give us a turn
    public bool endTurn = false; //send to combat handler to know that we are done

    public List<GameObject> targets; //list of all targets that we can currently see in combat
    private GameObject focusTarget; //target that we are focus on; the one that the spell will be cast in relation to
    private Dictionary<GameObject, Vector3> memory = new Dictionary<GameObject, Vector3>(); //keep track of the npc's memory of last seen locations
    private Dictionary<Spell, Vector3> spellMemory = new Dictionary<Spell, Vector3>(); //keep track of spells and their last seen location
    private List<Spell> usedSpellMemory = new List<Spell>(); //keep track of spells that have been casted to know what to expect
    public List<Vector3> movementQueue = new List<Vector3>(); //queue in movements for the ai to move in a single turn, if possible
    public GameObject debugPoint; //points that can be instantiated for debuging purposes
    private GameObject[] testRotationPoints = new GameObject[40]; //list of debug points for movementQueue
    private float lastAngle; //last angle we moved from (angle tests)
    private Vector3 lastRunAwayPosition = Vector3.zero; //last position

    private Vector3 lastPosition = Vector3.zero; //last position, used for keeping track of being stuck
    private float stuckTimer = 0f; //timer to keep track of when it began a thought process of being stuck
    private float stuckTimerBase = 2f; //timer value before the ai says it is stuck
    private bool isStuck = false; //if the ai can't move or is trying to move to impossible spot bool

    private int turnsToClose = 0; //turns we are too close to an enemy
    private float tooCloseDistance = 1f;
    private float safeDistance = 2f;

    public int difficulty = 1; //difficulty doesnt do anything (it did do something on v1 of the AI)
    public bool hasAttackedRecently; //has attacked recently bool; used for cooldowns on casting spells
    public bool inCover; //in cover bool
    public bool inPartialCover; //in partial cover bool
    

    private int rotationDirection = -1; //0 is counter clockwise, 1 is clockwise. If on -1, then is it not set to anything
    private Spell selectedSpell; //spell that is being casted (or attempted to)
    private Coroutine spellCoroutine; //coroutine for casting spell
    private float progress = 0; //progress from 0 to 1 of the spell being cast
    public CombatState state; //current state in combat for the ai
    public EnergyState energyState; //current state for energy in combat for the ai

    public void AIEndCombat() {
        targets.Clear();
        focusTarget = null;
        movementQueue.Clear();
        memory.Clear();
        spellMemory.Clear();
        isReady = false;
        endTurn = false;
        usedSpellMemory.Clear();
        lastPosition = Vector3.zero;
        selectedSpell = null;
        progress = 0;
        state = CombatState.ATTACKING;
        if(spellCoroutine != null) {
            StopCoroutine(spellCoroutine);
        }
    }

    /// <summary>
    /// Called at the beginning of an AI's turn (NOT THE PLAYER)
    /// </summary>
    /// <param name="charactersInCombat"></param>
    /// <param name="isPlayerInCombat"></param>
    public void AI(List<GameObject> charactersInCombat, bool isPlayerInCombat) {
        //print("AI STARTING!");
        TryStuck(); //test to see if we are stuck trying to get to a point
        EnergyCheck(); //check the state of the ai's energy
        GetTargets(charactersInCombat); //get all active targets
        TryAvoid(); //test if we have been too close to an enemy for too long
        CoverTest(); //tests to see if there are any changes to cover and partial cover
        SelectTarget(); //selects a target
        GetSpell(); //get a spell to perform
        SetMove(); //sets movement based on target and spell selected
        TrySpell(); //Trys to see if it can cast, if not, begins casting
        EnergyRecharge(); //recharge energy, if possible
        EndTurn(); //end turn
    }



    public enum CombatState {
        ATTACKING, //ai is seeking to be aggresive
        RUNNING, //ai is running from an active threat, with intent to leave the fight
        DEFENDING, //ai is trying to defend itself
        AVOIDING, //ai is running from an active threat, with intent to stay in the fight
        DYING, //ai is dead; hp = 0; animation needs to play
        DEAD //animation for dying is done
    }

    public enum EnergyState {
        FREE, //use as much as they would like
        CONSERVATIVE, //use less than normal
        REPRESS //cannot use any energy
    }

    //called for init
    void Start() {
        if(gameObject == GameManagerScript.ins.player) {
            return;
        }
        npcInfo = GetComponent<NPCInfo>(); //get its own info
        /*
        for (int i = 0; i < 40; i++) { //instantiate debug points
            testRotationPoints[i] = Instantiate(debugPoint) as GameObject;

        } */
    }

    //called every frame
    void Update() {
        if (gameObject == GameManagerScript.ins.player) {
            return;
        }

        if (npcInfo == null) { //check to see if initilization went wrong
            Start(); //re-init
            return;
        }else if (!npcInfo.inCombat) {
            return;
        }
        SetWalkState();
        CheckOutOfEnergy();

        UpdateMemory();
        SetState(); //get info about self and world and set the ai state accordingly
        if (state != CombatState.DYING) { //check if it isnt dead
            Move(); //move the ai every frame
            if (selectedSpell != null && progress >= 1) { //if we have a selected and spell and it is ready, cast it
                Cast();
            }
        } else { //the ai is dead
            Dead();
        }
    }



    void CheckOutOfEnergy() {
        if(npcInfo.currentStamina < 1) {
            npcInfo.IsWalking = true;
        }
    }

    void SetWalkState() {
        switch (state) {
            case CombatState.ATTACKING:
                switch (energyState) {
                    case EnergyState.FREE:
                        if (Random.Range(1, 100) <= 5) {
                            npcInfo.IsWalking = !npcInfo.IsWalking;
                        }
                        break;
                    case EnergyState.CONSERVATIVE:
                        if (npcInfo.IsWalking) {
                            if (Random.Range(1, 100) <= 1) {
                                npcInfo.IsWalking = false;
                            }
                        } else {
                            if (Random.Range(1, 100) <= 50) {
                                npcInfo.IsWalking = true;
                            }
                        }
                        break;
                    case EnergyState.REPRESS:
                        npcInfo.IsWalking = true;
                        break;
                }
                        break;
    }
    }

    void EnergyRecharge() {
        if(movementQueue.Count == 0) {
            npcInfo.RecoverStamina();
            npcInfo.RecoverStamina();
            npcInfo.RecoverStamina();
        }else if (npcInfo.IsWalking) {
            npcInfo.RecoverStamina();
            npcInfo.RecoverStamina();
        }
    }


    void EnergyCheck() {
        float energyPercentage = npcInfo.GetStaminaPercentage();
        switch (energyState) {
            case EnergyState.FREE:
                if (energyPercentage <= 0.2f) {
                    energyState = EnergyState.REPRESS;
                }else if(energyPercentage <= 0.65f) {
                    energyState = EnergyState.CONSERVATIVE;
                }
                break;
            case EnergyState.CONSERVATIVE:
                if (energyPercentage <= 0.2f) {
                    energyState = EnergyState.REPRESS;
                } else if (energyPercentage >= 0.85f) {
                    energyState = EnergyState.FREE;
                }
                break;
            case EnergyState.REPRESS:
                if (energyPercentage >= 0.85f) {
                    energyState = EnergyState.FREE;
                } else if (energyPercentage >= 0.5f) {
                    energyState = EnergyState.CONSERVATIVE;
                }
                break;
        }

    }

    /// <summary>
    /// Called when the AI has died
    /// </summary>
    void Dead() {
        GetComponent<Renderer>().material.color = new Color(0, 0, 0); //debug color change
        npcInfo.polyNav.enabled = false; //disable ai
        if(spellCoroutine != null) { //disable any spell coroutine if there are any active
            StopCoroutine(spellCoroutine);
            spellCoroutine = null;
        }
        progress = 0; //set spell progress to zero
        selectedSpell = null; //set selected spell to nothing
    }

    /// <summary>
    /// Called when a selected spell has finished its progress (= 1)
    /// </summary>
    void Cast() {
        if (SpellStaminaCheck(selectedSpell.energyToCast * 0.25f) && GetComponent<CharacterInfo>().IsVisible(focusTarget)) { //check to see if the target is visible and if we have enough energy to finish the cast
            switch (selectedSpell.type) {
                case Spell.Type.Projectile:
                    Projectile a = selectedSpell as Projectile;
                    //if the spell is a projectile, also check to see if target is close enough for the spell to hit, with small buffer
                    if(Vector3.Distance(memory[focusTarget], gameObject.transform.position) <= a.distance * 0.9) {
                        Vector3 dir = memory[focusTarget] - transform.position; //get direction to target
                        SpellManagerScript.ins.CastSpell(selectedSpell, transform.position, dir, gameObject); //send the information to the spell manager
                        progress = 0; //reset values
                        selectedSpell = null;
                    }
                    break;
            }
        }

    }

    /// <summary>
    /// Called at the start of a fight and when a(n) player/AI finished their turn
    /// </summary>
    public void StartWaiting() {
        //print(gameObject + " has started waiting...");
        endTurn = false; //end turn
        turnCoroutine = StartCoroutine(TurnCoroutine()); //start the turn coroutine, which will auto ready when it is their turn
    }

    /// <summary>
    /// Turn Coroutine called in StartWaiting()
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnCoroutine() {
        float s = (0.75f - (myStats.perception * 0.025f)); //get the seconds to wait based on stats
        //print(gameObject + " wait time is " + s);
        yield return new WaitForSeconds(s); //wait for seconds
        isReady = true; //ready the unit up so the combat handler knows it should be their turn
        turnCoroutine = null; //reset the turnCoroutine
        //print(gameObject + " has stopped waiting!");
    }

    /// <summary>
    /// Ends the unit's turn
    /// </summary>
    public void EndTurn() {
        //print(gameObject + " has ended their turn at " + Time.time);
        endTurn = true; //combat handler will handle the rest
    }

    /// <summary>
    /// Begins casting the selected spell, if we have not already
    /// </summary>
    void TrySpell() {

        if(selectedSpell !=null && spellCoroutine == null && progress == 0) { //checks to make sure we have a selected spell and we havent started casting
            if (SpellStaminaCheck()) { //check to see if we have enough potential stamina to cast
                spellCoroutine = StartCoroutine(SpellEnumerator()); //start the spellCoroutine to cast
            }

        }

    }

    /// <summary>
    /// Checks to see if we have enough stamina
    /// </summary>
    /// <returns></returns>
    bool SpellStaminaCheck() {
        if(npcInfo.currentStamina <= 0) { //if we run out of stamina, cancel the spell we are trying to cast
            if(spellCoroutine != null) {
                StopCoroutine(spellCoroutine);
                spellCoroutine = null;
            }
            progress = 0;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check to see if we have enough stamina, given a value A that will be used
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    bool SpellStaminaCheck(float a) {
        if ((npcInfo.currentStamina - a) < 0) { //if we dont have at least value A in stamina, cancel spell
            if (spellCoroutine != null) {
                StopCoroutine(spellCoroutine);
                spellCoroutine = null;
            }
            progress = 0;
            return false;
        }
        return true;

    }

    /// <summary>
    /// SpellEnumerator that drains energy
    /// </summary>
    /// <returns></returns>
    IEnumerator SpellEnumerator() {
        for (int i = 0; i < (selectedSpell.castTime * 100); i++) { //run for loop based on seconds (* 100 to get precision)
            progress = (i / (selectedSpell.castTime * 100)); //update progress
            SpellStaminaCheck(); //check stamina

            npcInfo.DrainStamina( ((selectedSpell.energyToCast - (selectedSpell.energyToCast * 0.25f)) / (selectedSpell.castTime * 100))); //drain stamina
            //print(((selectedSpell.energyToCast - (selectedSpell.energyToCast * 0.25f)) / (selectedSpell.castTime * 100)));
            yield return new WaitForSeconds(0.01f); //wait a 0.01s; this is to ensure its not based on framerate
        }
        progress = 1; //we have done the for-loop, so we are ready to cast
        spellCoroutine = null; //end the spellCoroutine
    }

    /// <summary>
    /// Get a new spell
    /// </summary>
    void GetSpell() {
        if(selectedSpell == null && state == CombatState.ATTACKING) { //check to make sure we dont have a spell selected and that we are ready to attack
            //get a new spell from npc's spell list
            List<Spell> allSpells = npcInfo.spellInventory.Spells();
            //iterate through all spells, if needed
            while (allSpells.Count > 0) {
                //get a random spell
                int i = Random.Range(0, allSpells.Count - 1);
                //check to see if we can cast the spell, which some buffer room for movement or other things
                if(allSpells[i].energyToCast < npcInfo.currentStamina * 0.75f) {
                    //if we can, select the spell and return
                    selectedSpell = allSpells[i];
                    return;
                } else {
                    //if we cant, remove the spell
                    allSpells.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Tests to see if we are in partial cover or full cover or none at all
    /// </summary>
    void CoverTest() {
        //print("Target Size: " + memory.Count);
        if(memory.Count > 0) { //check to see if we have a memory
            if(targets.Count == 1) { //if there is only one target, we have a special case for calculating cover and partial cover
                foreach(GameObject c in memory.Keys) {
                    if (GetComponent<CharacterInfo>().IsVisible(c)) { //checks if we can see; if we can, we are in partial cover; if not, full cover
                        inCover = false;
                        inPartialCover = true;
                    } else {
                        inCover = true;
                        inPartialCover = false;
                    }
                }
                

            } else {
                int count = 0;
                foreach(GameObject c in memory.Keys) { //check how many of our memory objects are NOT visible to us currently
                    if (!GetComponent<CharacterInfo>().IsVisible(c)) {
                        count += 1;
                    }
                }
                if(count == memory.Count) { //if NONE are visible, FULL COVER
                    inCover = true;
                    inPartialCover = false;
                }else if(count > 0) { //if SOME are visible, PARTIAL COVER
                    inCover = false;
                    inPartialCover = true;
                } else { //if ALL are visible, OPEN
                    inCover = false;
                    inPartialCover = false;
                }
            }


        } else { //if we have no memory, then there is nothing to test, so give the AI the best case
            inCover = true;
            inPartialCover = true;
        }



    }

    /// <summary>
    /// Get targets that are in combat with the unit
    /// </summary>
    /// <param name="charactersInCombat"></param>
    void GetTargets(List<GameObject> charactersInCombat) {
        //check if someone is visible, if they are, update their position in the ai's memory
        targets.Clear(); //clear old targets for new test
        foreach(GameObject c in charactersInCombat) { //check everyone in combat
            if(c == gameObject) {
                continue;
            }
            if(c == GameManagerScript.ins.player) { //if the object is the player, check ai's attitude to see if it should be hostile
                if(npcInfo.stats.attitude > -30f) {
                    continue;
                }
            } else {
                if(!FactionManagerScript.ins.DoesDislike(c.GetComponent<NPCInfo>().faction,npcInfo.faction) && !FactionManagerScript.ins.DoesHate(c.GetComponent<NPCInfo>().faction, npcInfo.faction)) { //if the object is another unit, check factions' to see if it should be hostile
                    continue;
                }
            }
            //unit should be hostile to current object
            if (GetComponent<CharacterInfo>().IsVisible(c)) { //if they are visible, add them to current targets and update/add their position in the memory
                targets.Add(c);
                if (memory.ContainsKey(c)) {
                    memory[c] = c.transform.position;
                } else {
                    memory.Add(c, c.transform.position);
                }
            }

        }
        //validation check to remove characters from memory if they are not in combat. Need to change to make sure AI knows they are dead (for instance)
        foreach(GameObject c in new List<GameObject>(memory.Keys)) {
            if (!charactersInCombat.Contains(c)) {
                memory.Remove(c);
            }

        }



    }

    /// <summary>
    /// check whether two points have unbroken LOS to each other
    /// </summary>
    void Move() {
        
        Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0); //get position of feet
        //print(Vector3.Distance(feet, movementQueue[0]));
        if(movementQueue.Count > 0) { //if we have points to move
            /*
            for (int i = 0; i < movementQueue.Count; i++) { //set debug points positions
                testRotationPoints[i].transform.position = movementQueue[i];
                lastRunAwayPosition = feet;
            } */
            if (Vector3.Distance(feet, movementQueue[0]) <= 0.2f) { //check to see if we have reached
                movementQueue.RemoveAt(0); //remove reached point
                Move(); //recursive call
            } else { //if we haven't reached, set destination
                npcInfo.polyNav.SetDestination(movementQueue[0]);
            }
        } else {
            npcInfo.polyNav.Stop(); //if we dont have points to move to, stop the ai from moving
            return;
        }
            


    }

    /// <summary>
    /// Set a movement pattern based on the unit's state
    /// </summary>
    void SetMove() {
        switch (state) { //switch statement for movement state so we know how to move
            case CombatState.ATTACKING:

                movementQueue.Clear(); //clear our movement
                if (selectedSpell == null || focusTarget == null) { //if we don't have a spell, we cant move; if we have no target who is are enemy, we cant move
                    return;
                }
                Vector3 dir = Vector3.zero; //init dir vec3
                switch (selectedSpell.type) { //switch statement for spell type to know what to do
                    case Spell.Type.Projectile: //projectile

                        Projectile p = selectedSpell as Projectile; //create instance of the spell as our type
                        Vector3 f = new Vector3(transform.position.x, transform.position.y - 0.45f, 0); //get feet position (point of AI)
                        float disFromTarget = Vector3.Distance(memory[focusTarget], f); //get current distance from focusTarget
                        Vector3 directionFromTargetToAI = f - memory[focusTarget]; //get current direction from focusTarget to us (feet)
                        float targetDistance = disFromTarget;

                        //raycast from focusTarget to us by p distance, which is the distance of the max range of the projectile
                        RaycastHit2D hit = Physics2D.Raycast(memory[focusTarget], f - memory[focusTarget], p.distance * 1.05f, CombatManager.ins.obstacleTest);
                        float wallDistance = Vector3.Distance(memory[focusTarget], hit.point) * 0.95f;
                        if (disFromTarget >= p.distance) {
                            targetDistance = p.distance * 0.9f;
                        } else if (disFromTarget <= p.distance * 0.35f) {
                            targetDistance = p.distance * 0.45F;
                        }
                        if (hit.collider != null) {
                            if (targetDistance >= wallDistance) {
                                targetDistance = wallDistance;
                            }
                        }

                        if (true) {
                            float angle = Vector2.Angle(Vector2.right, directionFromTargetToAI); 
                            if (transform.position.y - 0.45f < memory[focusTarget].y) { //we are bottom half of the y-axis, so add 180 since it is absolute
                                angle = 360 - angle;
                            }
                            //angle = Mathf.Round(angle);
                            //print(angle);
                            int counterClockwiseResult = 0;
                            int clockwiseResult = 0;

                            /*
                             * Raycast counter clockwise and test results  to get a rotation around the focusTarget
                             * Record how many successful paths
                             * */
                            for (int i = 0; i < 90; i++) {
                                float counterClockwiseAngle = angle + i; //increment the angle
                                if (counterClockwiseAngle > 360) { //validation check
                                    counterClockwiseAngle -= 360;
                                }

                                //check the point on the edge of the circle
                                float x1 = memory[focusTarget].x + targetDistance * Mathf.Cos(Mathf.Deg2Rad * counterClockwiseAngle);
                                float y1 = memory[focusTarget].y + targetDistance * Mathf.Sin(Mathf.Deg2Rad * counterClockwiseAngle);
                                //test the point
                                bool[] counterClockwiseTest = TryAttackPoint(x1, y1, targetDistance);
                                //if we cant reach the point or the focusTarget cant see, we have reached the farthest point
                                if (!counterClockwiseTest[0] || !counterClockwiseTest[1]) {
                                    break;
                                } else {
                                    counterClockwiseResult += 1;
                                }
                            }

                            /*
                             * Raycast clockwise and test results  to get a rotation around the focusTarget
                             * Record how many successful paths
                             * */
                            for (int i = 0; i < 90; i++) {
                                float clockwiseAngle = angle - i; //increment
                                if (clockwiseAngle > 360) { //validation check
                                    clockwiseAngle -= 360;
                                }

                                //create point on circle
                                float x2 = memory[focusTarget].x + targetDistance * Mathf.Cos(Mathf.Deg2Rad * clockwiseAngle);
                                float y2 = memory[focusTarget].y + targetDistance * Mathf.Sin(Mathf.Deg2Rad * clockwiseAngle);
                                //test the point
                                bool[] clockwiseTest = TryAttackPoint(x2, y2, targetDistance);
                                //if we cant reach the point or the focusTarget cant see, we have reached the farthest point
                                if (!clockwiseTest[0] || !clockwiseTest[1]) {
                                    break;
                                } else {
                                    clockwiseResult += 1;
                                }
                            }
                            //set direction or give chance to change. need to change later to account for spells and such
                            if (rotationDirection == 1) {
                                if (Random.Range(1, 10) == 1) { //10% chance to change direction
                                    rotationDirection = 0;
                                }
                            } else if (rotationDirection == 0) {
                                if (Random.Range(1, 10) == 1) { //10% chance to change direction
                                    rotationDirection = 1;
                                }
                            } else { //if we have not set a direction to go; base case
                                //check which results are greater and go that way. If they are the same, we random.
                                if (clockwiseResult < counterClockwiseResult) {
                                    rotationDirection = 0; //go cc
                                } else if (counterClockwiseResult > clockwiseResult) {
                                    rotationDirection = 1; // go c
                                } else {
                                    rotationDirection = Random.Range(0, 1); //random
                                }
                            }

                            //go after the last point, if first, then go the direction we were told.
                            //do check, because first may be turna round, which means first = second. So always do a check to see if have at least one point recorded.
                            //if not, then we can just move normally, because that means we have moved, so just move the normal way and record that, if possible

                            //we want 40 points in our queue
                            for (int i = 0; i < 40; i++) {
                                float newAngle = angle; //init
                                if (rotationDirection == 1) { //if we are going clockwise, increment
                                    if (movementQueue.Count != 0) {
                                        newAngle = lastAngle - 1;
                                    } else {
                                        newAngle -= 1;
                                    }
                                    if (newAngle < 0) {
                                        newAngle += 360;
                                    }

                                } else {
                                    if (movementQueue.Count != 0) { //if we are going counter-clockwise, increment
                                        newAngle = lastAngle + 1;
                                    } else {
                                        newAngle += 1;
                                    }
                                    if (newAngle > 360) {
                                        newAngle -= 360;
                                    }

                                }

                                //create new point on circle

                                //randomize the movement slightly
                                if(Random.Range(0, 4) == 0) {
                                    targetDistance = targetDistance + Random.Range(targetDistance * -0.05f, targetDistance * 0.05f);
                                    if (targetDistance >= p.distance) {
                                        targetDistance = p.distance * 0.9f;
                                    } else if (disFromTarget <= p.distance * 0.35f) {
                                        targetDistance = p.distance * 0.45F;
                                    }
                                    if (hit.collider != null) {
                                        if (targetDistance >= wallDistance) {
                                            targetDistance = wallDistance;
                                        }
                                    }
                                }
                                
                                float newX = memory[focusTarget].x + targetDistance * Mathf.Cos(Mathf.Deg2Rad * newAngle);
                                float newY = memory[focusTarget].y + targetDistance * Mathf.Sin(Mathf.Deg2Rad * newAngle);

                                //try the point
                                bool[] turnAroundTest = TryAttackPoint(newX, newY, targetDistance);

                                //if the someone cant see it, then we need to turn around and continue
                                if (!turnAroundTest[1]) {
                                    //turn around
                                    if (rotationDirection == 0) {
                                        rotationDirection = 1;
                                    } else {
                                        rotationDirection = 0;
                                    }
                                    continue;
                                } else {
                                    //we dont need to turn around, add the point and continue
                                    movementQueue.Add(new Vector3(newX, newY, 0));
                                    lastAngle = newAngle;
                                }


                            }

                        }
                        break;
                    case Spell.Type.Beam:
                        break;
                    case Spell.Type.Enhancement:
                        break;
                    case Spell.Type.Forbidden:
                        break;
                    case Spell.Type.Ring:
                        break;
                    case Spell.Type.Utility:
                        break;
                    case Spell.Type.Wall:
                        break;
                }
                break;
            case CombatState.RUNNING: //running
                int selfQuad = SetQuad(transform.position, memory[focusTarget]); //get our current quadrant in reference to focusTarget
                Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0); //get feet position (AI)
                Vector3 lastDirection = feet - lastRunAwayPosition; //get lastRunAwayPosition to feet direction
                float currentAngle = Vector2.Angle(Vector2.right, lastDirection); //get current Angle we are running
                currentAngle = AngleCheck(currentAngle, lastRunAwayPosition, feet); //validation
                List<Vector3> pointsInCover = new List<Vector3>(); //points in cover
                List<Vector3> pointsPossibleCover = new List<Vector3>(); //points that may lead us to cover, such as against walls
                List<Vector3> pointsOpen = new List<Vector3>(); //points that are not cover and will probably not lead to cover; in the open

                Vector3 lastPoint = Vector3.zero;

                //test every angle around us
                for (int i = 0; i < 360; i++) {
                    //make the circle
                    float x1 = transform.position.x + 1f * Mathf.Cos(Mathf.Deg2Rad * i);
                    float y1 = transform.position.y + 1f * Mathf.Sin(Mathf.Deg2Rad * i);
                    Vector3 point = new Vector3(x1, y1, 0);
                    if (i == 0) { //base case
                        lastPoint = point; //set base case
                        //do a raycast
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, point - transform.position, 10f, CombatManager.ins.obstacleTest);
                        //get point from hit (if there is one)
                        Vector3 testPoint = new Vector3(hit.point.x, hit.point.y, 0);
                        //add a little buffer room from the wall to our position/direction
                        testPoint += (transform.position - testPoint).normalized;
                        if (hit.collider != null) { //if we hit something
                            if (!GetComponent<CharacterInfo>().IsVisible(focusTarget, testPoint)) { //check to see if we are in cover
                                pointsInCover.Add(testPoint); //in cover
                            } else {
                                pointsOpen.Add(testPoint); //in the open (in the future tests, this one will be iffy)
                            }
                        } else {
                            pointsOpen.Add(point);
                        }
                    } else { //recursive
                        //raycast for cover
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, point - transform.position, 10f, CombatManager.ins.obstacleTest);
                        //create a point from the hit (if any)
                        Vector3 testPoint = new Vector3(hit.point.x, hit.point.y, 0);
                        //add our direction/position to the hit as a buffer
                        testPoint += (transform.position - testPoint).normalized / 2;
                        if (hit.collider != null) { //we hit something
                            if (!GetComponent<CharacterInfo>().IsVisible(focusTarget, testPoint)) { //check to see if we are in cover
                                pointsInCover.Add(testPoint);
                            } else {
                                /*If we are NOT in cover, check:
                                 *-if this test point and the last test point can see each other
                                 * -if the distance between this point and us is greater than the distance between last point and us
                                 * */
                                if (!GetComponent<CharacterInfo>().IsVisible(testPoint, lastPoint) && (Vector3.Distance(testPoint, transform.position) > Vector3.Distance(lastPoint, transform.position))) {
                                    pointsPossibleCover.Add(testPoint); //in possible cover
                                } else { //in cover
                                    pointsOpen.Add(point);
                                }
                            }
                        } else { //the point is out in the open
                            testPoint = point + ((point - transform.position).normalized * 10f); //distance of 10 in the direction of point (the current degree)
                            if (!GetComponent<CharacterInfo>().IsVisible(focusTarget, testPoint)) { //check to see if in cover
                                pointsInCover.Add(point + ((point - transform.position.normalized) * 3f)); //in cover
                            } else {
                                /*If we are NOT in cover, check:
                                 * -if this test point and the last test point can see each other
                                 * -if the distance between this point and us is greater than the distance between last point and us
                                 * -if the point is within 90 degree of our LOS (so we dont just make 180 flip judgements)
                                 * */
                                if (GetComponent<CharacterInfo>().IsVisible(testPoint, lastPoint) && (Vector3.Distance(testPoint, transform.position) > Vector3.Distance(lastPoint, transform.position)) && !RangeTest(transform.position, memory[focusTarget], testPoint, 90)) {
                                    pointsPossibleCover.Add(point + ((point - transform.position.normalized) * 3f)); //in possible cover
                                } else {
                                    pointsOpen.Add(point); //open
                                }

                            }
                        }
                    }
                    lastPoint = point; //set up for next iteration
                } //end for loop
                /*
                print("Cover: " + pointsInCover.Count);
                print("Possible: " + pointsPossibleCover.Count);
                print("Open: " + pointsOpen.Count); */

                //if we have points in cover, use them first
                if (pointsInCover.Count != 0) {
                    //print("Selected Cover!");
                    movementQueue.Clear(); //clear past movementqueue
                    Vector3 selectedPoint = pointsInCover[0]; //set base case
                    foreach (Vector3 p in pointsInCover) {
                        if (p == selectedPoint) { //if on base case, ignore
                            continue;
                        } else {
                           /*Check:
                            * -Distance is at least 2 (Arbitary number, just for movement purposes)
                            * -Distance is smaller than point we are currently using
                            * -It is within our LOS of 90 degrees (NOT DIRECTLY BEHIND US)
                            * -We don't have to pass through the focusTarget
                            * */
                            if (Vector3.Distance(p, transform.position) >= 2f && Vector3.Distance(p, transform.position) < Vector3.Distance(selectedPoint, transform.position) && RangeTest(transform.position, selectedPoint, p, 90) && QuadTest(SetQuad(focusTarget.transform.
                                position, p), SetQuad(memory[focusTarget], transform.position))) {
                                selectedPoint = p;
                            }
                        }
                    }
                    //add to movementqueue
                    movementQueue.Add(selectedPoint);
                //if there are no points in cover, use possible points in cover next
                } else if (pointsPossibleCover.Count != 0) {
                    //print("Selected Possible!");
                    movementQueue.Clear(); //clear our queue
                    Vector3 selectedPoint = pointsPossibleCover[0]; //set the base case
                    foreach (Vector3 p in pointsPossibleCover) { 
                        if (p == selectedPoint) { //skip base case
                            continue;
                        } else {
                            /*Check:
                            * -Distance is at least 2 (Arbitary number, just for movement purposes)
                            * -Distance is smaller than point we are currently using
                            * -It is within our LOS of 90 degrees (NOT DIRECTLY BEHIND US)
                            * -We don't have to pass through the focusTarget
                            * */
                            if (Vector3.Distance(p, transform.position) >= 2f && Vector3.Distance(p, transform.position) < Vector3.Distance(selectedPoint, transform.position) && RangeTest(transform.position, selectedPoint, p, 90) && QuadTest(SetQuad(focusTarget.transform.
                                position, p), SetQuad(memory[focusTarget], transform.position))) {
                                selectedPoint = p;
                            }
                        }
                    }
                    //add to movementqueue
                    movementQueue.Add(selectedPoint);

                //if no points in cover or possible cover, then we have to use a point out in the open
                } else if (pointsOpen.Count != 0) {
                    //print("Selected Open!");
                    movementQueue.Clear(); //clear the movement queue
                    int index = Random.Range(0, pointsOpen.Count - 1); //get a random index value
                    movementQueue.Add(pointsOpen[index]); //add a random point using the index
                
                //if somehow, no points were valid, then we may as well have the ai recover stamina
                }

                break;
            case CombatState.AVOIDING: //avoiding
                Vector3 movementVector = Vector3.zero;
                foreach(GameObject t in targets) {
                    if (GetComponent<CharacterInfo>().IsVisible(t)) {
                        if (Vector3.Distance(t.transform.position, gameObject.transform.position) <= safeDistance) {
                            movementVector += gameObject.transform.position - t.transform.position;
                        }
                    }
                }
                movementQueue.Add(gameObject.transform.position + movementVector);
                break;
            case CombatState.DEFENDING: //defending
                break;
            case CombatState.DYING: //dying
                //do nothing, you are dead!
                movementQueue.Clear();
                break;
            case CombatState.DEAD: //dead
                //do nothing, you are dead!
                movementQueue.Clear();
                break;
            default:
                print("idk");
                break;
        }

    }






    /// <summary>
    /// /return angle in full 360, opposed to 180, on basis that you used vector2.right
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="originObject"></param>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    float AngleCheck(float angle, Vector3 originObject, Vector3 targetObject) {
        if(targetObject.y < originObject.y) {
            return 360 - angle;
        }
        return angle;

    }

    /// <summary>
    /// check to see if an angle is over 360 or below 0 when adding or subtracting angles
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    float AngleMath(float angle) {
        if(angle > 360) {
            return angle - 360;
        }else if(angle < 0) {
            return angle + 360;
        } else {
            return angle;
        }

    }
    /// <summary>
    /// Returns information about cover and partial given a distance and coordinates x and y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    bool[] TryAttackPoint(float x, float y, float distance) {
        bool[] returnBools = new bool[2]; //0 = if the ai can see it; 1 = can the target see it
        Vector3 testVector = new Vector3(x, y, 0);
        RaycastHit2D targetWallHit = Physics2D.Raycast(memory[focusTarget], testVector - memory[focusTarget], distance, CombatManager.ins.obstacleTest);
        if(targetWallHit.collider == null) { //the target  can see it

            returnBools[1] = true;

        } else { //the target cant see it

            returnBools[1] = false;

        }

        RaycastHit2D selfWallHit = Physics2D.Raycast(testVector, transform.position - testVector, Vector3.Distance(testVector, transform.position), CombatManager.ins.obstacleTest);
        if (selfWallHit.collider == null) { //we can see it

            returnBools[0] = true;

        } else { //we cant see it

            returnBools[0] = false;

        }


        return returnBools;
    }

    /// <summary>
    /// Select a target from our list of targets
    /// </summary>
    void SelectTarget() {
        foreach (GameObject c in targets) { //run through each target
            if(focusTarget == null || focusTarget == c) { //get base case, or skip we we have already selected
                focusTarget = c;
            } else { //check distance, if the target is closer than the focus target, they become the new focus target
                if(Vector3.Distance(memory[focusTarget], transform.position) >= Vector3.Distance(c.transform.position, transform.position)) {
                    focusTarget = c;

                }
            }
        }

    }

    /// <summary>
    /// Set the state of the ai given self and world state
    /// </summary>
    void SetState() {
        if(npcInfo.currentHealth <= 0 && state != CombatState.DYING && state != CombatState.DEAD) { //if we are no health, we are dead
            state = CombatState.DYING;
        }
        //check what to do given our current state
        switch (state) {
            case CombatState.ATTACKING:
                //check to see if we need to not be attacking
                break;
            case CombatState.AVOIDING:
                state = CombatState.ATTACKING;
                foreach(GameObject t in targets) {
                    if (GetComponent<CharacterInfo>().IsVisible(t)) {
                        if(Vector3.Distance(t.transform.position, gameObject.transform.position) <= safeDistance) {
                            state = CombatState.AVOIDING;
                            break;
                        }
                    }
                }
                break;
            case CombatState.DEFENDING:
                break;
            case CombatState.DYING:
                //PLAY ANIMATION FOR DYING
                state = CombatState.DEAD;
                break;
            case CombatState.RUNNING:
                //we need to check to see if we need to stop running
                break;
            case CombatState.DEAD:
                print("I have died!");
                Destroy(gameObject);
                break;
            default:
                //nothing of note here in the default case
                print("do something -michael");
                break;

        }


    }

    /// <summary>
    /// Return what quadrant the given target is in reference to the origin
    /// </summary>
    /// <param name="target"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    int SetQuad(Vector3 target, Vector3 origin) {
        if(target.x >= origin.x) {
            if(target.y >= origin.y) {
                return 1; //+x, +y
            } else {
                return 4; //+x, -y
            }


        } else {
            if(target.y >= origin.y) {
                return 2; //-x, +y
            } else {
                return 3; //-x, -y
            }

        }
    }

    /// <summary>
    /// test to see if two given quadrants are opposite of each other (1-3, 2-4)
    /// </summary>
    /// <param name="self"></param>
    /// <param name="test"></param>
    /// <returns></returns>
    bool QuadTest(int self, int test) {
        if(Mathf.Abs(self - test) == 2) {
            return false; //failed
        }
        return true; //passed
    }

    /// <summary>
    /// Given an origin, check a range of two points angles
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="basePoint"></param>
    /// <param name="testPoint"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool RangeTest(Vector3 origin, Vector3 basePoint, Vector3 testPoint, int range) {
        float angle = Vector3.Angle(basePoint - origin, testPoint - origin); //get angle for basepoint and testpoint with reference to origin
        
        if(angle > range) { //check to see if it passes
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check to see if the AI is stuck and cannot move
    /// </summary>
    void TryStuck() {
        if(state == CombatState.DEFENDING || state == CombatState.DYING) { //exclude these movement state when moving
            return;
        }

        //test precision of last point and current point
        if(Mathf.FloorToInt(transform.position.x * 100) == Mathf.FloorToInt(lastPosition.x * 100) && Mathf.FloorToInt(transform.position.y * 100) == Mathf.FloorToInt(lastPosition.y * 100)) {
            if (isStuck && Time.time >= stuckTimer) { //if we are stuck and it has been X ammount of seconds, clear our movement to start fresh
                isStuck = false;
                movementQueue.Clear();
                if (state == CombatState.AVOIDING) {
                    state = CombatState.ATTACKING;
                } else {
                    state = CombatState.AVOIDING;
                }
            } else { //if not stuck, set the timer and set stuck
                print("isStuck");
                isStuck = true;
                stuckTimer = Time.time + stuckTimer;
                movementQueue.Clear();
                if (state == CombatState.AVOIDING) {
                    state = CombatState.ATTACKING;
                } else {
                    state = CombatState.AVOIDING;
                }
            }


        } else { //we are not stuck, so reset isStuck and lastPos
            isStuck = false;
            lastPosition = transform.position;
        }

    }

    void TryAvoid() {
        foreach(GameObject t in targets) {
            if (GetComponent<CharacterInfo>().IsVisible(t)) {
                if(Vector3.Distance(t.transform.position, gameObject.transform.position) <= tooCloseDistance) {
                    turnsToClose += 1;
                    break;
                }
            }
        }
        if(turnsToClose >= 3) {
            turnsToClose = 0;
            state = CombatState.AVOIDING;
        }
    }

    /// <summary>
    /// Update the memory dictionary with relevant character positions visible to the AI
    /// </summary>
    void UpdateMemory() {
        //if we have something in memory, check
        if(memory.Count != 0) {
            //iterate through each character in memory
            List<GameObject> memoryKeys = new List<GameObject>(memory.Keys);
            foreach(GameObject c in memoryKeys) {
                //if we can still see the character in question, we should update its position into the memory
                if (GetComponent<CharacterInfo>().IsVisible(c)) {
                    memory[c] = c.transform.position;
                }
            }          
        }
    }

}
