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

    public enum FireMode { //types of modes that we can fire
        FREE, //free directional
        POINT, //locked onto a point on the map
        TARGET //locked onto a target
    }


    public class Attack : CombatHUDLog.Move {
        public Spell selectedSpell;
        public Vector3 attackPoint;
        public Vector3 attackDirection;
        public GameObject attackObject;
        public RectTransform loggedInfo;
        public int hash;
        public GameObject attackTarget;

        void Start() {
            mt = MoveType.Attack;
        }
        public string ReturnMsg() {
            return "Cast " + selectedSpell.name;
        }
    }


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
        }
    }

    // Use this for initialization
    void Start () {
        combatHUDLog = GetComponent<CombatHUDLog>();
        combatMovePosition = GetComponent<PlayerDrawCombatMovePosition>();
	}

    void Update() {

        if (!isSelected) {
            return;
        }

        if (selectedSpell) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                SwitchFireMode();
            }
            SetMouse();
        }

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


    public void AddAttackToLayout(Attack a) {
        a.loggedInfo = (RectTransform)Instantiate(combatHUDLog.logPrefab, combatHUDLog.gridlayout);
        a.loggedInfo.GetComponent<TMPro.TextMeshProUGUI>().text = a.ReturnMsg();
        a.loggedInfo.GetComponentInChildren<CancelSpellScript>().parent = a;
    }

    public void RemoveAttackFromLayout(Attack a) {
        foreach(Transform child in combatHUDLog.gridlayout) {
            if(child == a.loggedInfo.transform) {
                Destroy(a.attackObject.gameObject);
                Destroy(a.loggedInfo.gameObject);
                loggedAttacks.Remove(a);
            }
        }
    }

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

    void RightClick() {
        UIManager.ins.ShowLogPanel();
        hasClicked = false;
        isFinished = false;
        CombatManager.ins.combatDrawMovePosition.ResetAttackValue();
        if (selectedSpell) {
            selectedSpell = false;
            Destroy(tempAttackDirectional.gameObject);
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
                        
                        break;
                    case FireMode.TARGET:

                        break;
                }
                break;
                
        }

    }

    //setting mouse position on movement line
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
            loggedAttacks[loggedAttacks.Count - 1].attackDirection = dir;
            AddAttackToLayout(loggedAttacks[loggedAttacks.Count - 1]);
            UIManager.ins.ShowLogPanel();
            ResetValues();
            DrawAttackPositions();
        }
    }


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

    void SwitchFireMode() {
        switch (fireMode) {
            case FireMode.FREE:
                fireMode = FireMode.POINT;
                if(tempAttackDirectional != null) {
                    Destroy(tempAttackDirectional);
                    fireModePointObject = Instantiate(attackOnLinePrefab);
                    fireModePointObject.name = "IM HERE";
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
                if(Vector3.Distance(GameManagerScript.ins.GetPlayerFeetPosition(), a.attackPoint) <= 0.2f) {
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




}
