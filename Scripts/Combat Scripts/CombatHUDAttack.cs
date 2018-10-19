using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatHUDAttack : MonoBehaviour {

    private CombatHUDLog combatHUDLog;
    private PlayerDrawCombatMovePosition combatMovePosition;
    public List<Attack> loggedAttacks = new List<Attack>();
    public bool isSelected;
    public bool hasClicked;
    public bool selectedSpell;
    public bool isFinished;
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

        void Start() {
            mt = MoveType.Attack;
        }
        public string ReturnMsg() {
            return "Cast " + selectedSpell.name;
        }
    }

    //reser values back to default (differs from right clicking as this happens after finished selecting everything)
    public void ResetValues() {
        CombatManager.ins.combatDrawMovePosition.ResetAttackValue();
        
        if((!isFinished || !selectedSpell) && hasClicked) {
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
        if (!isSelected) {
            return;
        }

        //if we have selected a spell, check for firemode swich call
        if (selectedSpell) {
            if (Input.GetKeyDown(KeyCode.Q)) {
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

    //add to the menu log
    public void AddAttackToLayout(Attack a) {
        a.loggedInfo = (RectTransform)Instantiate(combatHUDLog.logPrefab, combatHUDLog.gridlayout);
        a.loggedInfo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = a.ReturnMsg();
        a.loggedInfo.GetComponentInChildren<CancelSpellScript>().parent = a;
    }

    //remove from the menu log
    public void RemoveAttackFromLayout(Attack a) {
        foreach(Transform child in combatHUDLog.gridlayout) {
            if(child == a.loggedInfo.transform) {
                Destroy(a.attackObject.gameObject);
                Destroy(a.loggedInfo.gameObject);
                loggedAttacks.Remove(a);
            }
        }
    }

    //dont know
    void MouseClick() {
        if(tempAttack != null) {
            hasClicked = true;
            Attack att = new Attack();
            att.attackObject = tempAttack;
            att.attackPoint = tempAttack.transform.position;
            att.hash = tempHash;
            loggedAttacks.Add(att);
            tempAttack = null;
            tempHash = -1;
            UIManager.ins.ShowAttackPanel();
        }
    }


    //reset values
    void RightClick() {
        UIManager.ins.ShowLogPanel();
        hasClicked = false;
        isFinished = false;
        CombatManager.ins.combatDrawMovePosition.ResetAttackValue();
        if (selectedSpell) {
            selectedSpell = false;
            Destroy(tempAttackDirectional.gameObject);
            Destroy(fireModePointObject);
            if(selectedNPC != null) {
                selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                selectedNPC = null;
            }
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

    //set the firemode object positions accordingly
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
                        } else {
                            Vector2 mouse2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero);
                            if (hit.collider != null) {
                                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")) {

                                    if (!IsVisible(hit.collider.gameObject)) {
                                        break;
                                    }

                                    if (selectedNPC != null) {
                                        selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                                    }
                                    selectedNPC = hit.collider.gameObject;
                                    selectedNPC.GetComponent<Renderer>().material.color = Color.yellow;
                                    CheckTargetClick();
                                }
                            } else {
                                if (selectedNPC != null) {
                                    selectedNPC.GetComponent<Renderer>().material.color = Color.white;
                                    selectedNPC = null;
                                }
                            }
                        }
                        break;
                }
                break;
                
        }

    }

    //setting target attack
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

    //update each object in memory
    void UpdateMemory() {
        List<GameObject> characters = new List<GameObject>(memory.Keys);
        foreach(GameObject c in characters) {
            if (IsVisible(c)) {
                memory[c] = c.transform.position;
            }
        }
    }

    //add or update to memory
    void MemoryAdd(GameObject c) {
        if (memory.ContainsKey(c)) {
            memory[c] = c.transform.position;
        } else {
            memory.Add(c, c.transform.position);
        }
    }

    //checks whether we can directly see them
    public bool IsVisible(GameObject target) {
        int oldLayer = gameObject.layer;
        gameObject.layer = 2; //change to ignore raycast
        Vector3 feet = new Vector3(transform.position.x, transform.position.y - 0.45f, 0);
        RaycastHit2D hit = Physics2D.Raycast(feet, target.transform.position - feet, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
        //Debug.DrawRay(gameObject.transform.position, target.transform.position - gameObject.transform.position, Color.white, Mathf.Infinity);
        gameObject.layer = oldLayer; //Set layer back to normal
        if (hit.collider != null) {
            if (hit.collider.gameObject == target) {
                return true;
            }
        }
        return false;
    }

    //setting point attack
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

    //setting directional attack
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

    //for line detection for the attack
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
                        float percentage = mouseStartDir.magnitude / lineDir.magnitude;
                        if (tempAttack == null) {
                            tempAttack = Instantiate(attackOnLinePrefab);
                        }
                        if(percentage >= 0.975) {
                            tempAttack.transform.position = endPoint;
                        }else if(percentage <= 0.025) {
                            tempAttack.transform.position = startPoint;
                        } else {
                            tempAttack.transform.position = ((1 - percentage) * startPoint) + (percentage * endPoint);
                        }
                        tempHash = combatHUDLog.loggedMoves[x].hash;
                        return tempAttack.transform.position;
                    }
                    tempHash = -1;

                }
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

    public void CheckAttacks() {
        if(CombatManager.ins.combatHUDLog.loggedMoves.Count == 0) {
            return;
        }
        foreach (Attack a in loggedAttacks) {
            if(a.hash == CombatManager.ins.combatHUDLog.loggedMoves[0].hash) {
                //CHECK IF THE SPELL IS CLOSE ENOUGH TO BE CASTED AND IF hasCLICKED IS FALSE TO PREVENT CLICKING AND ADDING TO QUEUE
                if(Vector3.Distance(GameManagerScript.ins.GetPlayerFeetPosition(), a.attackPoint) <= 0.2f && hasClicked == false) {
                    Destroy(a.attackObject);
                    loggedAttacks.Remove(a);
                    //send to a queue in the player and then a queue to spell manager
                    //SpellManagerScript.ins.CastSpell(a, GameManagerScript.ins.player);
                    GameManagerScript.ins.playerInfo.spellQueue.Add(a);
                    return;
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
