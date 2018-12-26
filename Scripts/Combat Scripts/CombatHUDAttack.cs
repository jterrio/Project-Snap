using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatHUDAttack : MonoBehaviour {

    private CombatHUDLog combatHUDLog;
    private PlayerDrawCombatMovePosition combatMovePosition;
    public List<Attack> loggedAttacks = new List<Attack>();
    public bool isSelected; //has clicked the button to Attack ("A"); Step 1
    public bool hasClicked; //clicked on self or the line; Step 2
    public bool selectedSpell; //selected a spell; Step 3
    public bool isFinished; //clicked with a firemode; Step 4
    public GameObject attackOnLinePrefab;
    public GameObject attackDirectionalPrefab;
    private GameObject tempAttack;
    private GameObject tempAttackDirectional;
    private GameObject spellObject;
    private Spell spell;
    private int tempHash;
    private int count = -1;
    public FireMode fireMode = FireMode.FREE;
    private GameObject fireModePointObject;
    private GameObject selectedNPC;
    private bool selectedSelf;
    public Dictionary<GameObject, Vector3> memory = new Dictionary<GameObject, Vector3>(); //keep track of the player's memory of last seen locations

    public enum FireMode { //types of modes that we can fire
        FREE, //free directional
        POINT, //locked onto a point on the map
        TARGET //locked onto a target
    }


    public class Attack : CombatHUDLog.Move {
        public Spell selectedSpell; //selected spell associated with this
        public Vector3 attackPoint; //point on the movement line for the player
        public Vector3 attackDirection; //for FREE fire mode direction (SAVED AS DIRECTION)
        public Vector3 attackPointModePoint; //for POINT fire mode direction (SAVED AS POINT)
        public GameObject attackObject; //object of the spell
        public RectTransform loggedInfo; //info in the log
        public int hash; //random hash
        public GameObject attackTarget; 
        public FireMode fireMode; //fire mode of the spell
        public bool selfCast = false; //if the spell is cast from self
        public bool isCasting = false; //if the spell is currently in spell queue

        void Start() {
            mt = MoveType.Attack;
        }
        public string ReturnMsg() {
            return "Cast " + selectedSpell.name;
        }
    }

    /// <summary>
    /// reset values back to default (differs from right clicking as this happens after finished selecting everything)
    /// </summary>
    public void ResetValues() {
        CombatManager.ins.combatDrawMovePosition.ResetAttackValue();
        GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
        selectedSelf = false;
        if ((!isFinished || !selectedSpell) && hasClicked) {
            RightClick();
        } else {
            isFinished = false;
            hasClicked = false;
            selectedSpell = false;
            if(spellObject != null) {
                spellObject.GetComponent<SelectAttackScript>().IsSelected = false;
                spellObject = null;
            }
            spell = null;
            if(tempAttackDirectional != null) {
                Destroy(tempAttackDirectional.gameObject);
            }
            if(fireModePointObject != null) {
                Destroy(fireModePointObject.gameObject);
            }
        }
    }

    // Use this for initialization
    void Start () {
        combatHUDLog = GetComponent<CombatHUDLog>();
        combatMovePosition = GetComponent<PlayerDrawCombatMovePosition>();
	}

    //called every frame
    void Update() {
        UpdateMemory(); //update positions in memory
        VerifyAttacksOnLine(); //verify attacks on the line (could happen from re-mapping or error)
        if (!isSelected) {
            return;
        }

        //if we have selected a spell, check for firemode swich call
        if (selectedSpell) {
            if (Input.GetKeyDown(GameManagerScript.ins.ToggleFireMode)) {
                SwitchFireMode();
            }
            SetMouse(); //set selectspell's firemode object accordingly
        }

        //check to see if we have clicked something (something selected) and then we right click
        if (hasClicked) {
            if (Input.GetMouseButtonDown(1)) {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                if (results.Count > 0) {
                    if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                        return;
                    }
                }
                RightClick();
            }
            return;
        } //hasclicked

        //if we have clicked something in the ui or not
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count > 0) {
                if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                    return;
                }
            }
            MouseClick();
            return;
        }

        CalculateMousePosition();

    }

    /// <summary>
    /// add to the menu log
    /// </summary>
    /// <param name="a"></param>
    public void AddAttackToLayout(Attack a) {
        a.loggedInfo = (RectTransform)Instantiate(combatHUDLog.logPrefab, combatHUDLog.gridlayout);
        a.loggedInfo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = a.ReturnMsg();
        a.loggedInfo.GetComponentInChildren<CancelSpellScript>().parent = a;
        if(a.fireMode != FireMode.TARGET) {
            a.loggedInfo.GetComponentInChildren<Slider>().gameObject.SetActive(false);
        }
        SortAttackLayout();
    }

    /// <summary>
    /// remove from the menu log
    /// </summary>
    /// <param name="a"></param>
    public void RemoveAttackFromLayout(Attack a) {
        foreach(Transform child in combatHUDLog.gridlayout) {
            if(child == a.loggedInfo.transform) {
                Destroy(a.attackObject.gameObject);
                Destroy(a.loggedInfo.gameObject);
                loggedAttacks.Remove(a);
                return;
            }
        }
    }

    /// <summary>
    /// Sort logged attacks in order to be cast
    /// </summary>
    public void SortAttackLayout() {

        //get copies of the gridlayout
        List<Transform> gridLayoutChildren = new List<Transform>();
        foreach (Transform child in combatHUDLog.gridlayout) {
            gridLayoutChildren.Add(child);
        }
        //make the parent free for re-assignment
        combatHUDLog.gridlayout.DetachChildren();

        //get all spells that are in queue and being cast
        foreach(Attack a in GameManagerScript.ins.player.GetComponent<PlayerInfo>().spellQueue) {
            foreach(Transform t in new List<Transform>(gridLayoutChildren)) {
                if(t == a.loggedInfo.transform) {
                    a.loggedInfo.transform.SetParent(combatHUDLog.gridlayout);
                    gridLayoutChildren.Remove(t);
                }
            }
        }

        //get all other spells
        List<Attack> toSortAttacks = new List<Attack>(loggedAttacks);
        List<Attack> onLineAttacks = new List<Attack>();
        foreach(CombatHUDLog.Movement m in combatHUDLog.loggedMoves) {
            foreach(Attack a in new List<Attack>(toSortAttacks)) {
                //its on the line/path
                if(a.hash == m.hash) {
                    onLineAttacks.Add(a);
                }
            }
            Vector3 lastPosition = GameManagerScript.ins.player.transform.position;
            List<Attack> toCompareAttacks = new List<Attack>();
            foreach(Vector3 v in m.destination) {
                foreach (Attack b in new List<Attack>(onLineAttacks)) {
                    if (IsPointOnLine(lastPosition, v, b.attackPoint)) {
                        toCompareAttacks.Add(b);
                        onLineAttacks.Remove(b);
                    }
                }

                while(toCompareAttacks.Count > 0) {
                    Attack closest = null;
                    foreach(Attack c in toCompareAttacks) {
                        if(closest == null) {
                            closest = c;
                        }else if(Vector3.Distance(closest.attackPoint, lastPosition) > Vector3.Distance(c.attackPoint, lastPosition)) {
                            closest = c;
                        }
                    }
                    toCompareAttacks.Remove(closest);
                    closest.loggedInfo.SetParent(combatHUDLog.gridlayout);
                }
                lastPosition = v;
            }

        }

    }


    /// <summary>
    /// Test whether a given point is on a given line between two points
    /// </summary>
    /// <param name="startP">Start of the line</param>
    /// <param name="endP">End of the line</param>
    /// <param name="testP">Point to test against</param>
    /// <returns>Returns true if the point is on the line</returns>
    bool IsPointOnLine(Vector3 startP, Vector3 endP, Vector3 testP) {
        bool toReturn = false;
        Vector3 startToEnd = (endP - startP);
        Vector3 testToEnd = (endP - testP);
        Vector3 startToTest = (testP - startP);
        float a = Vector3.Dot(startToTest, startToEnd);
        float b = (testP.x - startP.x) * (testP.x - startP.x) + (testP.y - startP.y) * (testP.y - startP.y);

        if ((Vector3.SqrMagnitude(Vector3.Cross(startToEnd, testToEnd)) < 0.1f) && (a >= 0) && (a >= (b - 0.1f))) {
            toReturn = true;
        }
        return toReturn;

    }

    /// <summary>
    /// Called when clicking a point on the line or when selecting self
    /// </summary>
    void MouseClick() {
        if(tempAttack != null || selectedSelf == true) {
            hasClicked = true;
            Attack att = new Attack();
            if (!selectedSelf) {
                att.attackObject = tempAttack;
                att.attackPoint = tempAttack.transform.position;
            } else {
                att.attackPoint = GameManagerScript.ins.player.transform.position;
                att.selfCast = true;
            }
            att.hash = tempHash;
            loggedAttacks.Add(att);
            tempAttack = null;
            tempHash = -1;
            UIManager.ins.ShowAttackPanel();
        }
    }


    /// <summary>
    /// reset values
    /// </summary>
    void RightClick() {
        UIManager.ins.ShowLogPanel();
        hasClicked = false;
        isFinished = false;
        CombatManager.ins.combatDrawMovePosition.ResetAttackValue();
        GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
        selectedSelf = false;
        if (selectedNPC != null) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
            selectedNPC = null;
        }
        if (selectedSpell) {
            selectedSpell = false;
            spell = null;
            spellObject = null;
            Destroy(tempAttackDirectional.gameObject);
            Destroy(fireModePointObject);
        }
        if (loggedAttacks.Count == 0) {
            return;
        }
        if(loggedAttacks[loggedAttacks.Count - 1].loggedInfo != null) {
            RemoveAttackFromLayout(loggedAttacks[loggedAttacks.Count - 1]);
        }
        Destroy(loggedAttacks[loggedAttacks.Count - 1].attackObject);
        loggedAttacks.Remove(loggedAttacks[loggedAttacks.Count - 1]);
    }

    /// <summary>
    /// set the firemode object positions accordingly
    /// </summary>
    void SetMouse() {
        switch (spell.type) {
            case Spell.Type.Projectile:
                //fireMode = FireMode.POINT; //default for projectiles
                switch (fireMode) {
                    case FireMode.FREE:
                        Vector3 startPos = loggedAttacks[loggedAttacks.Count - 1].attackPoint;
                        Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        endPos = new Vector3(endPos.x, endPos.y, 0);
                        Vector3 dir = endPos - startPos;
                        endPos = startPos + dir.normalized * 1f;
                        tempAttackDirectional.transform.position = startPos;
                        Vector3[] points = new Vector3[2];
                        points[0] = startPos;
                        points[1] = endPos;
                        tempAttackDirectional.GetComponent<LineRenderer>().positionCount = 2;
                        tempAttackDirectional.GetComponent<LineRenderer>().SetPositions(points);
                        tempAttackDirectional.GetComponent<LineRenderer>().startWidth = 0.05f;
                        tempAttackDirectional.GetComponent<LineRenderer>().endWidth = 0.05f;
                        CheckDirectionalClick(dir.normalized);
                        break;
                    case FireMode.POINT:
                        //fireModePointObject = Instantiate(attackOnLinePrefab);
                        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        fireModePointObject.transform.position = new Vector3(mouse.x, mouse.y, 0);
                        CheckPointClick();
                        break;
                    case FireMode.TARGET:
                        PointerEventData pointerData = new PointerEventData(EventSystem.current);
                        pointerData.position = Input.mousePosition;
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointerData, results);
                        if (results.Count > 0) {
                            if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                                break;
                            }
                        }
                            Vector2 mouse2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero);
                        if (hit.collider != null) {
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")) {
                                if (selectedNPC != null) {
                                    if (selectedNPC != hit.collider.gameObject) {
                                        selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                                    }
                                }
                                selectedNPC = hit.collider.gameObject;
                                if (!IsVisible(hit.collider.gameObject, loggedAttacks[loggedAttacks.Count - 1].attackPoint)) {
                                    selectedNPC.GetComponent<Renderer>().material.color = Color.red;
                                } else {
                                    selectedNPC.GetComponent<Renderer>().material.color = Color.yellow;
                                    CheckTargetClick();
                                }
                            }
                        } else {
                            if (selectedNPC != null) {
                                selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                                selectedNPC = null;
                            }
                        }
                        break;
                }
                break;
                
        }

    }

    /// <summary>
    /// setting target attack
    /// </summary>
    void CheckTargetClick() {
        if(selectedNPC == null) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count > 0) {
                if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                    return;
                }
            }
            isFinished = true;
            loggedAttacks[loggedAttacks.Count - 1].selectedSpell = spell;
            loggedAttacks[loggedAttacks.Count - 1].fireMode = FireMode.TARGET;
            loggedAttacks[loggedAttacks.Count - 1].attackTarget = selectedNPC;
            AddAttackToLayout(loggedAttacks[loggedAttacks.Count - 1]);
            MemoryAdd(selectedNPC);
            UIManager.ins.ShowLogPanel();
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
            selectedNPC = null;
            ResetValues();
            DrawAttackPositions();
        }
    }

    /// <summary>
    /// update each object in memory
    /// </summary>
    void UpdateMemory() {
        foreach(GameObject c in new List<GameObject>(memory.Keys)) {
            if(c == null) {
                memory.Remove(c);
            }
            if (IsVisible(c)) {
                memory[c] = c.transform.position;
            }
        }
    }

    /// <summary>
    /// add or update to memory
    /// </summary>
    /// <param name="c"></param>
    void MemoryAdd(GameObject c) {
        if (memory.ContainsKey(c)) {
            memory[c] = c.transform.position;
        } else {
            memory.Add(c, c.transform.position);
        }
    }

    /// <summary>
    /// checks whether we can directly see them
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsVisible(GameObject target) {
        if(target == null) {
            return false;
        }
        int selfOldLayer = GameManagerScript.ins.player.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //change to ignore raycast
        int targetOldLayer = target.layer;
        target.layer = LayerMask.NameToLayer("SightTest"); //change to what we test
        RaycastHit2D hit = Physics2D.Raycast(GameManagerScript.ins.player.transform.position, target.transform.position - GameManagerScript.ins.player.transform.position, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
        //Debug.DrawRay(GameManagerScript.ins.player.transform.position, target.transform.position - GameManagerScript.ins.player.transform.position, Color.green);
        GameManagerScript.ins.player.layer = selfOldLayer; //Set layer back to normal
        target.layer = targetOldLayer; //Set layer back to normal
        if (hit.collider != null) {
            if (hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// checks whether the location of the attack can see them
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackPosition"></param>
    /// <returns></returns>
    public bool IsVisible(GameObject target, Vector3 attackPosition) {
        if(target == null) {
            return false;
        }
        int targetOldLayer = target.layer;
        target.layer = LayerMask.NameToLayer("SightTest"); //change to what we test
        RaycastHit2D hit = Physics2D.Raycast(attackPosition, target.transform.position - attackPosition, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
        target.layer = targetOldLayer; //Set layer back to normal
        if (hit.collider != null) {
            if (hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// setting point attack
    /// </summary>
    void CheckPointClick() {
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count > 0) {
                if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                    return;
                }
            }
            isFinished = true;
            loggedAttacks[loggedAttacks.Count - 1].selectedSpell = spell;
            loggedAttacks[loggedAttacks.Count - 1].fireMode = FireMode.POINT;
            loggedAttacks[loggedAttacks.Count - 1].attackPointModePoint = fireModePointObject.transform.position;
            AddAttackToLayout(loggedAttacks[loggedAttacks.Count - 1]);
            UIManager.ins.ShowLogPanel();
            ResetValues();
            DrawAttackPositions();
        }

    }

    /// <summary>
    /// setting directional attack
    /// </summary>
    /// <param name="dir"></param>
    void CheckDirectionalClick(Vector3 dir) {
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count > 0) {
                if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                    return;
                }
            }
            isFinished = true;
            loggedAttacks[loggedAttacks.Count - 1].selectedSpell = spell;
            loggedAttacks[loggedAttacks.Count - 1].fireMode = FireMode.FREE;
            loggedAttacks[loggedAttacks.Count - 1].attackDirection = dir;
            AddAttackToLayout(loggedAttacks[loggedAttacks.Count - 1]);
            UIManager.ins.ShowLogPanel();
            ResetValues();
            DrawAttackPositions();
        }
    }

    /// <summary>
    /// for line detection for the attack
    /// </summary>
    /// <returns></returns>
    Vector3 CalculateMousePosition() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        if (combatHUDLog.loggedMoves.Count != 0) {
            int x = -1;
            foreach (CombatHUDLog.Movement lines in combatHUDLog.loggedMoves) {
                x += 1;
                Vector3 startPoint = Vector3.zero;
                Vector3 endPoint = Vector3.zero;
                for (int i = 0; i < combatHUDLog.loggedMoves[x].destination.Length - 1; i++) {
                    startPoint = combatHUDLog.loggedMoves[x].destination[i];
                    endPoint = combatHUDLog.loggedMoves[x].destination[i + 1];
                    Vector3 lineDir = (endPoint - startPoint);
                    Vector3 mouseEndDir = (endPoint - mousePosition);
                    Vector3 mouseStartDir = (mousePosition - startPoint);
                    float a = Vector3.Dot(mouseStartDir, lineDir);
                    float b = (mousePosition.x - startPoint.x) * (mousePosition.x - startPoint.x) + (mousePosition.y - startPoint.y) * (mousePosition.y - startPoint.y);
                    
                    if ((Vector3.SqrMagnitude(Vector3.Cross(lineDir, mouseEndDir)) < 0.1f) && (a >= 0) && (a >= (b - 0.1f))) {
                        selectedSelf = false;
                        float percentage = mouseStartDir.magnitude / lineDir.magnitude;
                        if (tempAttack == null) {
                            tempAttack = Instantiate(attackOnLinePrefab);
                        }
                        GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
                        tempAttack.GetComponent<SpriteRenderer>().enabled = true;
                        if (percentage >= 0.975) {
                            tempAttack.transform.position = endPoint;
                        } else if (percentage <= 0.025) {
                            tempAttack.transform.position = startPoint;
                        } else {
                            tempAttack.transform.position = ((1 - percentage) * startPoint) + (percentage * endPoint);
                        }
                        tempHash = combatHUDLog.loggedMoves[x].hash;
                        return tempAttack.transform.position;
                    } else {
                        Vector2 mouse2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                        RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero);
                        if (hit.collider != null) {
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                                selectedSelf = true;
                                if (tempAttack != null) {
                                    tempAttack.GetComponent<SpriteRenderer>().enabled = false;
                                }
                                GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.yellow;
                                tempHash = combatHUDLog.loggedMoves[x].hash;
                                return GameManagerScript.ins.player.transform.position;
                            } else {
                                selectedSelf = false;
                                GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
                            }
                        } else {
                            selectedSelf = false;
                            GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                    }
                    tempHash = -1; //means set to nothing (i tihnk)

                }
            }

        } else {
            Vector2 mouse2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero);
            if (hit.collider != null) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    selectedSelf = true;
                    GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.yellow;
                    return GameManagerScript.ins.player.transform.position;
                } else {
                    selectedSelf = false;
                    GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
                }
            } else {
                selectedSelf = false;
                GameManagerScript.ins.player.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if (tempAttack != null) {
            Destroy(tempAttack);
        }
        return Vector3.zero;
    }


    public void ChangeSelection() {
        if (isSelected) {
            UnSelect();
        } else {
            Select();
        }
    }
    
    public void Select() {
        isSelected = true;
        hasClicked = false;
    }

    public void UnSelect() {
        isSelected = false;
        hasClicked = false;
    }

    public void SelectSpell(Spell spell, GameObject parent) {
        if(this.spell != null) {
            spellObject.GetComponent<SelectAttackScript>().IsSelected = false;
        }
        this.spell = spell;
        spellObject = parent;
        selectedSpell = true;
        switch (spell.type) {
            case Spell.Type.Projectile:
                fireMode = FireMode.FREE;
                tempAttackDirectional = Instantiate(attackDirectionalPrefab);
                break;
        }
    }

    public void UnSelectSpell(Spell spell, GameObject parent) {
        this.spell = null;
        spellObject = null;
        selectedSpell = false;
        if(tempAttackDirectional != null) {
            Destroy(tempAttackDirectional.gameObject);
        }
        if(fireModePointObject != null) {
            Destroy(fireModePointObject.gameObject);
        }
    }

    void SwitchFireMode() {
        switch (fireMode) {
            case FireMode.FREE:
                fireMode = FireMode.POINT;
                if(tempAttackDirectional != null) {
                    Destroy(tempAttackDirectional);
                    fireModePointObject = Instantiate(attackOnLinePrefab);
                    //fireModePointObject.name = "IM HERE";
                }
                break;
            case FireMode.POINT:
                fireMode = FireMode.TARGET;
                if (fireModePointObject != null) {
                    Destroy(fireModePointObject);
                }
                break;
            case FireMode.TARGET:
                fireMode = FireMode.FREE;
                if(selectedNPC != null) {
                    selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                    selectedNPC = null;
                }
                tempAttackDirectional = Instantiate(attackDirectionalPrefab);
                break;
        }
    }

    public void DrawAttackPositions() {
        foreach(Attack a in loggedAttacks) {
            if(a.attackObject == null) {
                a.attackObject = Instantiate(attackOnLinePrefab);
                a.attackObject.transform.position = a.attackPoint;
            }
        }
    }

    /// <summary>
    /// Verifys if each attack point is on the movement path, if not, it deletes it and removes from queue
    /// </summary>
    public void VerifyAttacksOnLine() {
        List<Attack> toVerify = new List<Attack>();
        List<Attack> spellQueue = GameManagerScript.ins.player.GetComponent<PlayerInfo>().spellQueue;
        foreach (CombatHUDLog.Movement m in combatHUDLog.loggedMoves) {
            foreach (Attack a in loggedAttacks) {
                //its on the line/path
                if (a.hash == m.hash && !spellQueue.Contains(a)) {
                    toVerify.Add(a);
                }
            }
            Vector3 lastPosition = GameManagerScript.ins.player.transform.position;
            bool onLine = false;
            foreach (Attack b in new List<Attack>(toVerify)) {
                foreach (Vector3 v in m.destination) {
                    if (IsPointOnLine(lastPosition, v, b.attackPoint)) {
                        onLine = true;
                    }
                    lastPosition = v;
                }
                lastPosition = GameManagerScript.ins.player.transform.position;
                if (!onLine) {
                    toVerify.Remove(b);
                    RemoveAttackFromLayout(b);
                }
            }

        }
    }

    public void CheckAttacks() {
        if(hasClicked == true) {
            return;
        }
        foreach (Attack a in new List<Attack>(loggedAttacks)) {
            if (a.selfCast) {
                a.isCasting = true;
                Destroy(a.attackObject);
                loggedAttacks.Remove(a);
                GameManagerScript.ins.playerInfo.spellQueue.Add(a);
            } else if(a.hash == CombatManager.ins.combatHUDLog.loggedMoves[0].hash) {
                //CHECK IF THE SPELL IS CLOSE ENOUGH TO BE CASTED AND IF hasCLICKED IS FALSE TO PREVENT CLICKING AND ADDING TO QUEUE
                if (Vector3.Distance(GameManagerScript.ins.GetPlayerFeetPosition(), a.attackPoint) <= 0.2f && hasClicked == false) {
                    a.isCasting = true;
                    Destroy(a.attackObject);
                    loggedAttacks.Remove(a);
                    //send to a queue in the player and then a queue to spell manager
                    //SpellManagerScript.ins.CastSpell(a, GameManagerScript.ins.player);
                    GameManagerScript.ins.playerInfo.spellQueue.Add(a);
                }
            }
        }
    }

    public void RemoveAllAttacks() {
        List<Attack> att = new List<Attack>(loggedAttacks);
        foreach(Attack a in att) {
            RemoveAttackFromLayout(a);
        }
    }




}
