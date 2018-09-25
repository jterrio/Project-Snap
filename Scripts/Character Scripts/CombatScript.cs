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
    public GameObject debugPoint;
    private GameObject[] testRotationPoints = new GameObject[40];
    private float lastAngle;
    private Vector3 lastRunAwayPosition = Vector3.zero;
    private bool hasRunAway = false;
    private float runAwayDistance = 0.75f;


    public int difficulty = 1;
    public bool hasAttackedRecently;
    public bool inCover;
    public bool inPartialCover;

    private int rotationDirection = -1; //0 is c-c, 1 is c. If on -1, then is it not set to anything
    private Spell selectedSpell;
    private Coroutine spellCoroutine;
    private float progress = 0;
    public CombatState state;

    private bool wallToLeft = false;
    private bool wallToRight = false;

    public class FocusTarget {
        public GameObject focusTarget {
            get {
                return focusTarget;
            }
            set {
                focusTarget = value;
                
            }
        }

        public Vector3 position {
            get {
                return focusTarget.transform.position;
            }
            set {
                position = value;
            }
        }

    }


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
        for (int i = 0; i < 40; i++) {
            testRotationPoints[i] = Instantiate(debugPoint) as GameObject;

        }
    }

    void Update() {
        SetState();
        if (state != CombatState.DYING) {
            Move();
            if (selectedSpell != null && progress >= 1) {
                Cast();
            }
        } else {
            Dead();
        }
    }


    void Dead() {
        GetComponent<Renderer>().material.color = new Color(0, 0, 0);
        npcInfo.polyNav.enabled = false;
        if(spellCoroutine != null) {
            StopCoroutine(spellCoroutine);
            spellCoroutine = null;
        }
        progress = 0;
        selectedSpell = null;
    }

    void Cast() {
        if (SpellStaminaCheck(selectedSpell.energyToCast * 0.25f) && IsVisible(focusTarget)) {
            Vector3 dir = focusTarget.transform.position - transform.position;
            SpellManagerScript.ins.CastSpell(selectedSpell, transform.position, dir, gameObject);
            progress = 0;
            selectedSpell = null;
        }

    }


    public void StartWaiting() {
        //print(gameObject + " has started waiting...");
        endTurn = false;
        turnCoroutine = StartCoroutine(TurnCoroutine());
    }


    IEnumerator TurnCoroutine() {
        float s = (0.75f - (myStats.perception * 0.025f));
        //print(gameObject + " wait time is " + s);
        yield return new WaitForSeconds(s);
        isReady = true;
        turnCoroutine = null;
        //print(gameObject + " has stopped waiting!");
    }

    public void EndTurn() {
        print(gameObject + " has ended their turn at " + Time.time);
        endTurn = true;
    }

    public void AI(List<GameObject> charactersInCombat, bool isPlayerInCombat) {
        //print("AI STARTING!");
        GetTargets(charactersInCombat);
        CoverTest();
        SelectTarget();
        GetSpell();
        SetMove();
        TrySpell(); //add dis check; if running, cancel
        EndTurn();
    }


    void TrySpell() {

        if(selectedSpell !=null && spellCoroutine == null && progress == 0) {
            if (SpellStaminaCheck()) {
                spellCoroutine = StartCoroutine(SpellEnumerator());
            }

        }

    }

    bool SpellStaminaCheck() {
        if(npcInfo.currentStamina <= 0) {
            if(spellCoroutine != null) {
                StopCoroutine(spellCoroutine);
                spellCoroutine = null;
            }
            progress = 0;
            return false;
        }
        return true;
    }

    bool SpellStaminaCheck(float a) {
        if ((npcInfo.currentStamina - a) < 0) {
            if (spellCoroutine != null) {
                StopCoroutine(spellCoroutine);
                spellCoroutine = null;
            }
            progress = 0;
            return false;
        }
        return true;

    }


    IEnumerator SpellEnumerator() {
        for (int i = 0; i < (selectedSpell.castTime * 100); i++) {
            progress = (i / (selectedSpell.castTime * 100));
            SpellStaminaCheck();

            npcInfo.DrainStamina( ((selectedSpell.energyToCast - (selectedSpell.energyToCast * 0.25f)) / (selectedSpell.castTime * 100)));
            //print(((selectedSpell.energyToCast - (selectedSpell.energyToCast * 0.25f)) / (selectedSpell.castTime * 100)));
            yield return new WaitForSeconds(0.01f);
        }
        progress = 1;
        spellCoroutine = null;
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

    void CoverTest() {
        //print("Target Size: " + memory.Count);
        if(memory.Count > 0) {
            if(targets.Count == 1) {
                foreach(GameObject c in memory.Keys) {
                    if (IsVisible(c)) {
                        inCover = false;
                        inPartialCover = true;
                    } else {
                        inCover = true;
                        inPartialCover = false;
                    }
                }
                

            } else {
                int count = 0;
                foreach(GameObject c in memory.Keys) {
                    if (!IsVisible(c)) {
                        count += 1;
                    }
                }
                if(count == memory.Count) {
                    inCover = true;
                    inPartialCover = false;
                }else if(count > 0) {
                    inCover = false;
                    inPartialCover = true;
                } else {
                    inCover = false;
                    inPartialCover = false;
                }
            }


        } else {
            inCover = true;
            inPartialCover = true;
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
        Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
        RaycastHit2D hit = Physics2D.Raycast(feet, target.transform.position - feet, Mathf.Infinity, CombatManager.ins.characterVisibleTest);
        gameObject.layer = 8; //set back to npc
        //Debug.DrawRay(gameObject.transform.position, target.transform.position - gameObject.transform.position, Color.white, Mathf.Infinity);
        if(hit.collider != null) {
            if(hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }

    public bool IsVisible(GameObject target, Vector3 point) {
        gameObject.layer = 2; // change to ignore raycast
        RaycastHit2D hit = Physics2D.Raycast(point, target.transform.position - point, Mathf.Infinity, CombatManager.ins.characterVisibleTest);
        gameObject.layer = 8; //set back to npc
        if(hit.collider != null) {
            if(hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }

    public bool IsVisible(Vector3 point1, Vector3 point2) {
        RaycastHit2D hit = Physics2D.Raycast(point1, point2 - point1, Vector3.Distance(point1, point2), CombatManager.ins.obstacleTest);
        if(hit.collider == null) {
            return true;
        }
        return false;

    }


    void Move() {
        Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
        //print(Vector3.Distance(feet, movementQueue[0]));
        if(movementQueue.Count > 0) {
            for (int i = 0; i < movementQueue.Count; i++) {
                testRotationPoints[i].transform.position = movementQueue[i];
                lastRunAwayPosition = feet;
            }
            if (Vector3.Distance(feet, movementQueue[0]) <= 0.2f) {
                movementQueue.RemoveAt(0);
                Move();
            } else {
                npcInfo.polyNav.SetDestination(movementQueue[0]);
            }
        } else {
            npcInfo.polyNav.Stop();
            return;
        }


    }


    void SetMove() {
        if (state == CombatState.ATTACKING) {
            movementQueue.Clear();
            if (selectedSpell == null) {
                return;
            }
            Vector3 dir = Vector3.zero;
            if (selectedSpell.type == Spell.Type.Projectile) {
                Projectile p = selectedSpell as Projectile;
                Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
                float disFromTarget = Vector3.Distance(focusTarget.transform.position, feet);
                Vector3 directionFromTargetToAI = feet - focusTarget.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(focusTarget.transform.position, feet - focusTarget.transform.position, p.distance * 1.05f, CombatManager.ins.obstacleTest);

                if (hit.collider == null) { //we have room to move back
                    float angle = Vector2.Angle(Vector2.right, directionFromTargetToAI);
                    if (transform.position.y - 0.45f < focusTarget.transform.position.y) { //we are bottom half of the y-axis, so add 180 since it is absolute
                        angle = 360 - angle;
                    }
                    //angle = Mathf.Round(angle);
                    //print(angle);



                    int counterClockwiseResult = 0;
                    int clockwiseResult = 0;

                    for (int i = 0; i < 90; i++) {
                        float counterClockwiseAngle = angle + i;
                        if (counterClockwiseAngle > 360) {
                            counterClockwiseAngle -= 360;
                        }
                        float x1 = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * counterClockwiseAngle);
                        float y1 = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * counterClockwiseAngle);
                        bool[] counterClockwiseTest = TryAttackPoint(x1, y1, p.distance);
                        if (!counterClockwiseTest[0] || !counterClockwiseTest[1]) {
                            break;
                        } else {
                            counterClockwiseResult += 1;
                        }
                    }

                    for (int i = 0; i < 90; i++) {
                        float clockwiseAngle = angle - 10f;
                        if (clockwiseAngle > 360) {
                            clockwiseAngle -= 360;
                        }
                        float x2 = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * clockwiseAngle);
                        float y2 = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * clockwiseAngle);
                        bool[] clockwiseTest = TryAttackPoint(x2, y2, p.distance);
                        if (!clockwiseTest[0] || !clockwiseTest[1]) {
                            break;
                        } else {
                            clockwiseResult += 1;
                        }
                    }
                    //print(gameObject);
                    //print("C: " + clockwiseResult);
                    //print("CC: " + counterClockwiseResult);
                    //set direction or give chance to change. need to change later to account for spells and such
                    if (rotationDirection == 1) {
                        if (Random.Range(1, 10) == 1) { //1% chance to change direction
                            rotationDirection = 0;
                        }
                    } else if (rotationDirection == 0) {
                        if (Random.Range(1, 10) == 1) { //1% chance to change direction
                            rotationDirection = 1;
                        }
                    } else {
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

                    for (int i = 0; i < 40; i++) {
                        float newAngle = angle;
                        if (rotationDirection == 1) {
                            if (movementQueue.Count != 0) {
                                newAngle = lastAngle - 1;
                            } else {
                                newAngle -= 1;
                            }
                            if (newAngle < 0) {
                                newAngle += 360;
                            }

                        } else {
                            if (movementQueue.Count != 0) {
                                newAngle = lastAngle + 1;
                            } else {
                                newAngle += 1;
                            }
                            if (newAngle > 360) {
                                newAngle -= 360;
                            }

                        }
                        float newX = focusTarget.transform.position.x + p.distance * Mathf.Cos(Mathf.Deg2Rad * newAngle);
                        float newY = focusTarget.transform.position.y + p.distance * Mathf.Sin(Mathf.Deg2Rad * newAngle);
                        bool[] turnAroundTest = TryAttackPoint(newX, newY, p.distance);
                        if (!turnAroundTest[1]) {
                            //
                            if (rotationDirection == 0) {
                                rotationDirection = 1;
                            } else {
                                rotationDirection = 0;
                            }
                            continue;
                        } else {
                            movementQueue.Add(new Vector3(newX, newY, 0));
                            lastAngle = newAngle;
                        }


                    }






                } else { //we dont have enough room behind us
                    float disFromWall = Vector3.Distance(focusTarget.transform.position, hit.point);



                }

            } else { //proj

            }
        } else if (state == CombatState.RUNNING) {
            int selfQuad = SetQuad(transform.position, focusTarget.transform.position);
            Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
            Vector3 lastDirection = feet - lastRunAwayPosition;
            float currentAngle = Vector2.Angle(Vector2.right, lastDirection);
            currentAngle = AngleCheck(currentAngle, lastRunAwayPosition, feet);
            List<Vector3> pointsInCover = new List<Vector3>();
            List<Vector3> pointsPossibleCover = new List<Vector3>();
            List<Vector3> pointsOpen = new List<Vector3>();

            Vector3 lastPoint = Vector3.zero;
            for (int i = 0; i < 360; i++) {
                float x1 = transform.position.x + 1f * Mathf.Cos(Mathf.Deg2Rad * i);
                float y1 = transform.position.y + 1f * Mathf.Sin(Mathf.Deg2Rad * i);
                Vector3 point = new Vector3(x1, y1, 0);
                if (i == 0) { //base case
                    lastPoint = point;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, point - transform.position, 10f, CombatManager.ins.obstacleTest);
                    Vector3 testPoint = new Vector3(hit.point.x, hit.point.y, 0) + new Vector3(hit.normal.x, hit.normal.y, 0);
                    if(hit.collider != null) {
                        if (!IsVisible(focusTarget, testPoint)) {
                            pointsInCover.Add(testPoint);
                        } else {
                            pointsOpen.Add(testPoint);
                        }
                    } else {
                        pointsOpen.Add(point);
                    }
                } else { //recursive
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, point - transform.position, 10f, CombatManager.ins.obstacleTest);
                    Vector3 testPoint = new Vector3(hit.point.x, hit.point.y, 0) + new Vector3(hit.normal.x, hit.normal.y, 0);
                    if (hit.collider != null) { //we hit something
                        if (!IsVisible(focusTarget, testPoint)) {
                            pointsInCover.Add(testPoint);
                        } else {
                            if (!IsVisible(testPoint, lastPoint) && (Vector3.Distance(testPoint, transform.position) > Vector3.Distance(lastPoint, transform.position))) {
                                pointsPossibleCover.Add(testPoint);
                            } else {
                                pointsOpen.Add(point);
                            }
                        }
                    } else { //the point is out in the open
                        testPoint = point + ((point - transform.position).normalized * 15f);
                        if(!IsVisible(focusTarget, testPoint)) { //point is in cover
                            pointsInCover.Add(testPoint);
                        } else {

                            if(IsVisible(testPoint, lastPoint)  &&  (Vector3.Distance(testPoint, transform.position) > Vector3.Distance(lastPoint, transform.position))) {
                                pointsPossibleCover.Add(testPoint);
                            } else {
                                pointsOpen.Add(point);
                            }

                        }
                    }
                }
                lastPoint = point;
            } //end for loop

            print("Cover: " + pointsInCover.Count);
            print("Possible: " + pointsPossibleCover.Count);
            print("Open: " + pointsOpen.Count);

            if(pointsInCover.Count != 0) {
                print("Selected Cover!");
                movementQueue.Clear();
                Vector3 selectedPoint = pointsInCover[0];
                foreach(Vector3 p in pointsInCover) {
                    if(p == selectedPoint) {
                        continue;
                    } else {
                        if (Vector3.Distance(p, transform.position) < Vector3.Distance(selectedPoint, transform.position) && RangeTest(transform.position, selectedPoint, p, 90) && QuadTest(SetQuad(focusTarget.transform.
                            position, p), SetQuad(focusTarget.transform.position, transform.position))) {
                            selectedPoint = p;
                        }
                    }
                }
                movementQueue.Add(selectedPoint);

            }else if(pointsPossibleCover.Count != 0) {
                print("Selected Possible!");
                movementQueue.Clear();
                Vector3 selectedPoint = pointsPossibleCover[0];
                foreach (Vector3 p in pointsPossibleCover) {
                    if (p == selectedPoint) {
                        continue;
                    } else {
                        if (Vector3.Distance(p, transform.position) < Vector3.Distance(selectedPoint, transform.position) && RangeTest(transform.position, selectedPoint, p, 90) && QuadTest(SetQuad(focusTarget.transform.
                            position, p), SetQuad(focusTarget.transform.position, transform.position))) {
                            selectedPoint = p;
                        }
                    }
                }
                movementQueue.Add(selectedPoint);


            } else if(pointsOpen.Count != 0) {
                print("Selected Open!");
                movementQueue.Clear();
                int index = Random.Range(0, pointsOpen.Count - 1);
                movementQueue.Add(pointsOpen[index]);

            } else {
                npcInfo.RecoverStamina();
            }


        }else if (state == CombatState.RECHARGING) {
            npcInfo.RecoverStamina();
        }

    }
                
            

            
       


    float AngleCheck(float angle, Vector3 originObject, Vector3 targetObject) {
        if(targetObject.y < originObject.y) {
            return 360 - angle;
        }
        return angle;

    }

    float AngleMath(float angle) {
        if(angle > 360) {
            return angle - 360;
        }else if(angle < 0) {
            return angle + 360;
        } else {
            return angle;
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
        if(npcInfo.currentHealth <= 0) {
            state = CombatState.DYING;
        }
        switch (state) {
            case CombatState.ATTACKING:
                if(npcInfo.currentStamina <= npcInfo.maxStamina * 0.25f) {
                    state = CombatState.RUNNING;
                }
                break;
            case CombatState.AVOIDING:
                break;
            case CombatState.DEFENDING:
                break;
            case CombatState.DYING:
                break;
            case CombatState.RECHARGING:
                movementQueue.Clear();
                if (!inCover || (memory.Count == 1 && inPartialCover)) {
                    state = CombatState.RUNNING;
                }
                if(npcInfo.currentStamina >= npcInfo.maxStamina * 0.8f) {
                    state = CombatState.ATTACKING;
                }
                break;
            case CombatState.RUNNING:
                if (inCover || npcInfo.currentStamina >= npcInfo.maxStamina * 0.8f) {
                    state = CombatState.RECHARGING;
                }
                break;
            default:
                print("do something -michael");
                break;

        }


    }

    int SetQuad(Vector3 target, Vector3 origin) {
        if(target.x >= origin.x) {
            if(target.y >= origin.y) {
                return 1;
            } else {
                return 4;
            }


        } else {
            if(target.y >= origin.y) {
                return 2;
            } else {
                return 3;
            }

        }
    }

    bool QuadTest(int self, int test) {
        if(Mathf.Abs(self - test) == 2) {
            return false; //failed
        }
        return true;
    }

    bool RangeTest(Vector3 origin, Vector3 basePoint, Vector3 testPoint, int range) {
        float angle = Vector3.Angle(basePoint - origin, testPoint - origin);
        
        if(angle > range) {
            return false;
        }
        return true;
    }

}
