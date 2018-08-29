using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour {

    private NPCInfo npcInfo;
    private Coroutine turnCoroutine;
    public Stats myStats;

    public bool isReady = false;
    public bool endTurn = false;

    public List<GameObject> targets;
    private GameObject focusTarget;
    private Dictionary<GameObject, Vector3> memory = new Dictionary<GameObject, Vector3>(); //keep track of the npc's memory of last seen locations
    private Dictionary<Spell, Vector3> spellMemory = new Dictionary<Spell, Vector3>(); //keep track of spells and their last seen location
    private List<Spell> usedSpellMemory = new List<Spell>(); //keep track of spells that have been casted to know what to expect
    public List<Vector3> movementQueue = new List<Vector3>(); //queue in movements for the ai to move in a single turn, if possible

    public int difficulty = 1;
    public bool hasAttackedRecently;
    public bool inCover;
    public bool inPartialCover;

    private int rotationDirection = -1; //0 is c-c, 1 is c. If on -1, then is it not set to anything
    private Spell selectedSpell;
    public CombatState state;

    public enum CombatState {
        ATTACKING,
        RUNNING,
        DEFENDING,
        RECHARGING,
        AVOIDING,
        DYING
    }


    void Awake() {
        npcInfo = GetComponent<NPCInfo>();
    }

    void Update() {
        Move();

    }


    public void StartWaiting() {
        endTurn = false;
        turnCoroutine = StartCoroutine(TurnCoroutine());
    }


    IEnumerator TurnCoroutine() {
        float s = (0.75f - (myStats.perception * 0.025f));
        yield return new WaitForSeconds(s);
        isReady = true;
    }

    public void EndTurn() {
        endTurn = true;
    }

    public void AI(List<GameObject> charactersInCombat, bool isPlayerInCombat) {
        GetTargets(charactersInCombat);
        SelectTarget();
        SetState();
        GetSpell();
        SetMove();
        EndTurn();
    }

    void GetSpell() {
        if(selectedSpell == null && state == CombatState.ATTACKING) {
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

    void GetTargets(List<GameObject> charactersInCombat) {
        //check if someone is visible, if they are, update their position in the ai's memory
        targets.Clear();
        foreach(GameObject c in charactersInCombat) {
            if(c == GameManagerScript.ins.player) {
                if(npcInfo.stats.attitude > -30f) {
                    continue;
                }
            } else {
                if(c.GetComponent<NPCInfo>().faction == npcInfo.faction) {
                    continue;
                }
            }
            if (IsVisible(c)) {
                targets.Add(c);
                if (memory.ContainsKey(c)) {
                    memory[c] = c.transform.position;
                } else {
                    memory.Add(c, c.transform.position);
                }
            }

        }
        //validation check
        foreach(GameObject c in memory.Keys) {
            if (!charactersInCombat.Contains(c)) {
                memory.Remove(c);
            }

        }



    }
    
    //checks whether we can directly see them
    public bool IsVisible(GameObject target) {
        gameObject.layer = 2; //change to ignore raycast
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, target.transform.position - gameObject.transform.position, Mathf.Infinity, CombatManager.ins.characterVisibleTest);
        gameObject.layer = 8; //set back to npc
        //Debug.DrawRay(gameObject.transform.position, target.transform.position - gameObject.transform.position, Color.white, Mathf.Infinity);
        if(hit.collider != null) {
            if(hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }


    void Move() {
        Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
        if(movementQueue.Count > 0) {
            if(Vector3.Distance(feet, movementQueue[0]) <= 0.2f) {
                movementQueue.RemoveAt(0);
                Move();
            } else {
                npcInfo.polyNav.SetDestination(movementQueue[0]);
            }
        } else {
            return;
        }


    }


    void SetMove() {
        movementQueue.Clear(); //remove old points that we wanted to move to
        if (state == CombatState.ATTACKING) {
            if(selectedSpell == null) {
                return;
            }
            Vector3 dir = Vector3.zero;
            if (selectedSpell.type == Spell.Type.Projectile) {
                Projectile p = selectedSpell as Projectile;
                float disFromTarget = Vector3.Distance(focusTarget.transform.position, transform.position);
                Vector3 directionFromTargetToAI = transform.position - focusTarget.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(focusTarget.transform.position, transform.position - focusTarget.transform.position, p.distance * 1.05f, CombatManager.ins.obstacleTest);

                if (hit.collider == null) { //we have room to move back
                    float angle = Vector2.Angle(Vector2.down, directionFromTargetToAI);
                    if (transform.position.x < focusTarget.transform.position.x) { //we are left half of the x-axis, so add 180 since it is absolute
                        angle = 360 - angle;
                    }
                    print(angle);

                  

                    int counterClockwiseResult = 0;
                    int clockwiseResult = 0;

                    for(int i = 0; i < 90; i++) {
                        float counterClockwiseAngle = angle + i;
                        if (counterClockwiseAngle > 360) {
                            counterClockwiseAngle -= 360;
                        }
                        float x1 = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * counterClockwiseAngle);
                        float y1 = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * counterClockwiseAngle);
                        bool[] counterClockwiseTest = TryAttackPoint(x1, y1, p.distance);
                        if(!counterClockwiseTest[0] || !counterClockwiseTest[1]) {
                            break;
                        } else {
                            counterClockwiseResult += 1;
                        }
                    }

                    for(int i = 0; i< 90; i++) {
                        float clockwiseAngle = angle - 10f;
                        if (clockwiseAngle > 360) {
                            clockwiseAngle -= 360;
                        }
                        float x2 = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * clockwiseAngle);
                        float y2 = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * clockwiseAngle);
                        bool[] clockwiseTest = TryAttackPoint(x2, y2, p.distance);
                        if(!clockwiseTest[0] || !clockwiseTest[1]) {
                            break;
                        } else {
                            clockwiseResult += 1;
                        }
                    }
                    print(gameObject);
                    print("C: " + clockwiseResult);
                    print("CC: " + counterClockwiseResult);
                   if(rotationDirection == 0 && counterClockwiseResult <= 5) {
                        for(int i = 0; i < counterClockwiseResult; i++) {
                            float ccAngle = (angle + i);
                            if (ccAngle > 360) {
                                ccAngle -= 360;
                            }
                            float newX = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * ccAngle);
                            float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * ccAngle);
                            movementQueue.Add(new Vector3(newX, newY, 0));
                        }
                        for(int i = 0; i < counterClockwiseResult; i++) {
                            float cAngle = angle - i;
                            if(cAngle < 0) {
                                cAngle += 360;
                            }
                            float newX = focusTarget.transform.position.x + p.distance * (Mathf.Cos(Mathf.Deg2Rad * cAngle));
                            float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * cAngle);
                            movementQueue.Add(new Vector3(newX, newY, 0));

                        }
                    }else if(rotationDirection == 1 && clockwiseResult <= 5) {
                        for (int i = 0; i < clockwiseResult; i++) {
                            float cAngle = (angle - i);
                            if (cAngle < 0) {
                                cAngle += 360;
                            }
                            float newX = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * cAngle);
                            float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * cAngle);
                            movementQueue.Add(new Vector3(newX, newY, 0));
                        }
                        for (int i = 0; i < clockwiseResult; i++) {
                            float ccAngle = angle + i;
                            if (ccAngle > 360) {
                                ccAngle -= 360;
                            }
                            float newX = focusTarget.transform.position.x + p.distance * (Mathf.Cos(Mathf.Deg2Rad * ccAngle));
                            float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * ccAngle);
                            movementQueue.Add(new Vector3(newX, newY, 0));

                        }
                    } else {
                        //set direction or give chance to change. need to change later to account for spells and such
                        if (rotationDirection == 1) {
                            if(Random.Range(1, 100) == 1) { //1% chance to change direction
                                rotationDirection = 0;
                            }
                        }else if(rotationDirection == 0) {
                            if(Random.Range(1, 100) == 1) { //1% chance to change direction
                                rotationDirection = 1;
                            }
                        } else {
                            //rotationDirection = Random.Range(0, 1); // 50% chance to pick a direction;
                            rotationDirection = 1;
                        }

                        if(rotationDirection == 1) { //clockwise
                            for(int i = 0; i < 10; i++) {
                                float cAngle = (angle - i);
                                if (cAngle < 0) {
                                    cAngle += 360;
                                }
                                float newX = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * cAngle);
                                float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * cAngle);
                                movementQueue.Add(new Vector3(newX, newY, 0));
                                Debug.DrawLine(transform.position, new Vector3(newX, newY, 0), Color.white);
                            }


                        } else { //counter clockwise
                            for(int i = 0; i< 10; i++) {
                                float ccAngle = angle + i;
                                if (ccAngle > 360) {
                                    ccAngle -= 360;
                                }
                                float newX = focusTarget.transform.position.x + p.distance * (Mathf.Cos(Mathf.Deg2Rad * ccAngle));
                                float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * ccAngle);
                                movementQueue.Add(new Vector3(newX, newY, 0));
                                Debug.DrawLine(transform.position, new Vector3(newX, newY, 0), Color.white);
                            }


                        }





                    }

                    




                } else { //we dont have enough room behind us
                    float disFromWall = Vector3.Distance(focusTarget.transform.position, hit.point);



                }

            } else { //proj

            }
        }

    }

    bool[] TryAttackPoint(float x, float y, float distance) {
        bool[] returnBools = new bool[2]; //0 = if the ai can see it; 1 = can the player see it
        Vector3 testVector = new Vector3(x, y, 0);
        RaycastHit2D targetWallHit = Physics2D.Raycast(focusTarget.transform.position, testVector - focusTarget.transform.position, distance, CombatManager.ins.obstacleTest);
        if(targetWallHit.collider == null) {

            returnBools[1] = true;

        } else {

            returnBools[1] = false;

        }

        RaycastHit2D selfWallHit = Physics2D.Raycast(testVector, transform.position - testVector, Vector3.Distance(testVector, transform.position), CombatManager.ins.obstacleTest);
        if (selfWallHit.collider == null) {

            returnBools[0] = true;

        } else {

            returnBools[0] = false;

        }


        return returnBools;
    }


    void SelectTarget() {
        foreach (GameObject c in targets) {
            if(focusTarget == null || focusTarget == c) {
                focusTarget = c;
            } else {
                if(Vector3.Distance(focusTarget.transform.position, transform.position) >= Vector3.Distance(c.transform.position, transform.position)) {
                    focusTarget = c;

                }
            }
        }

    }

    void SetState() {
        if (npcInfo.currentHealth <= 0) {
            state = CombatState.DYING;
        } else if (npcInfo.currentStamina <= npcInfo.maxStamina * 0.25f) {
            if (state == CombatState.ATTACKING) {
                state = CombatState.RUNNING;
            } else if (state == CombatState.RUNNING) {
                //do cover check and set when appropriate

            }

        } else if (npcInfo.currentStamina >= npcInfo.maxStamina * 0.8f) {
            state = CombatState.ATTACKING;
        }

        //do spell check to see if we need to avoid or defend


    }

}
