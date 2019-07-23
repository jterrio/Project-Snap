using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerDrawCombatMovePosition : MonoBehaviour {

    public bool isSelected;
    public LineRenderer lr;
    public PolyNav.PolyNavAgent playerAgent;
    private CombatHUDLog log;
    private CombatHUDAttack attackLog;
    private Vector3[] loggedPath;
    private bool canMove = false;
    private CombatHUDLog.Movement selectedLine;

    void Start() {
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        playerAgent = GameManagerScript.ins.player.GetComponent<PolyNav.PolyNavAgent>();
        log = GetComponent<CombatHUDLog>();
        attackLog = GetComponent<CombatHUDAttack>();
    }


    void Update() {
        //DisplayLoggedPath();

        if (!isSelected) {
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl)) {
            bool isTouchingLine = false;
            Vector3 startPoint = GameManagerScript.ins.player.transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
            foreach (CombatHUDLog.Movement lines in CombatManager.ins.combatHUDLog.loggedMoves) {
                foreach (Vector3 v in lines.destination) {
                    Vector3 endPoint = v;
                    Vector3 lineDir = (endPoint - startPoint);
                    Vector3 mouseEndDir = (endPoint - mousePosition);
                    Vector3 mouseStartDir = (mousePosition - startPoint);
                    float a = Vector3.Dot(mouseStartDir, lineDir);
                    float b = (mousePosition.x - startPoint.x) * (mousePosition.x - startPoint.x) + (mousePosition.y - startPoint.y) * (mousePosition.y - startPoint.y);

                    if ((Vector3.SqrMagnitude(Vector3.Cross(lineDir, mouseEndDir)) < 0.02f) && (a >= 0) && (a >= (b - 0.1f))) {
                        isTouchingLine = true;
                        if (selectedLine == null) {
                            selectedLine = lines;
                            selectedLine.pathLR.startColor = Color.red;
                            selectedLine.pathLR.endColor = Color.red;
                        } else if (selectedLine != lines) {
                            selectedLine.pathLR.startColor = Color.white;
                            selectedLine.pathLR.endColor = Color.white;
                            selectedLine = lines;
                            selectedLine.pathLR.startColor = Color.red;
                            selectedLine.pathLR.endColor = Color.red;
                        }
                        if (Input.GetMouseButtonDown(0)) {
                            PointerEventData pointerData = new PointerEventData(EventSystem.current);
                            pointerData.position = Input.mousePosition;
                            List<RaycastResult> results = new List<RaycastResult>();
                            EventSystem.current.RaycastAll(pointerData, results);
                            foreach(RaycastResult r in results) {
                                if(r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                                    return;
                                }
                            }
                            float percentage = mouseStartDir.magnitude / lineDir.magnitude;
                            Vector3 n = ((1 - percentage) * startPoint) + (percentage * endPoint);
                            MouseClickMiddle(startPoint, endPoint, n, lines.destination, lines.hash);
                            selectedLine = null;
                            if (!Input.GetKey(KeyCode.LeftShift) && canMove) {
                                ChangeMovementValue();
                            }
                            return; 
                        }
                        break;
                    }
                    startPoint = v;
                }
                if (isTouchingLine) {
                    break;
                }
            }
            if (!isTouchingLine) {
                if (selectedLine != null) {
                    selectedLine.pathLR.startColor = Color.white;
                    selectedLine.pathLR.endColor = Color.white;
                    selectedLine = null;
                }
            }
        } else {
            if(selectedLine != null) {
                selectedLine.pathLR.startColor = Color.white;
                selectedLine.pathLR.endColor = Color.white;
                selectedLine = null;
            }
        }

        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult r in results) {
                if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                    return;
                }
            }
            MouseClick();
            if (!Input.GetKey(KeyCode.LeftShift) && canMove) {
                ChangeMovementValue();
            }
            return;
        }

        if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftControl)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult r in results) {
                if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                    return;
                }
            }
            RemoveLastMovementLog();
            return;
        }

        if (!log.HasPosition()) {
            playerAgent.map.FindPath(GameManagerScript.ins.GetPlayerFeetPosition(), Camera.main.ScreenToWorldPoint(Input.mousePosition), DisplayPath);
        } else {
            playerAgent.map.FindPath(log.GetLastPosition(), Camera.main.ScreenToWorldPoint(Input.mousePosition), DisplayPath);
        }
    }

    void RemoveLastMovementLog() {
        log.RemoveLastLogMovementPosition();
        VerifyAttacksOnPath();
    }

    void DisplayLoggedPath() {
        loggedPath = log.LoggedPath();
        lr.positionCount = loggedPath.Length;
        lr.SetPositions(loggedPath);
    }

    void MouseClick() {
        if (!log.HasPosition()) {
            playerAgent.map.FindPath(GameManagerScript.ins.player.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), LogPath);
        } else {
            playerAgent.map.FindPath(log.GetLastPosition(), Camera.main.ScreenToWorldPoint(Input.mousePosition), LogPath);
        }
    }

    void MouseClickMiddle(Vector3 start, Vector3 end, Vector3 middle, Vector3[] des, int desHash) {
        log.InsertNode(middle, System.Array.IndexOf(des, start), System.Array.IndexOf(des, end), des, desHash);
    }

    public void LogPath(Vector2[] path) {
        if (path == null || path.Length == 0) {
            canMove = false;
            return;
        }
        canMove = true;
        int count = path.Length;
        log.LogMovePosition(path);
    }

    void DisplayPath(Vector2[] path) {
        if (path == null || path.Length == 0) {
            lr.positionCount = 0;
            return;
        }
        if(!log.CanReachFirstPosition(path[path.Length - 2], path[path.Length - 1])){
            lr.positionCount = 0;
            return;
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            lr.positionCount = 0;
            return;
        }
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (RaycastResult r in results) {
            if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                lr.positionCount = 0;
                return;
            }
        }
        lr.positionCount = path.Length;
        Vector3[] pointList = new Vector3[lr.positionCount];
        for (int i = 0; i < path.Length; i++) {
            pointList[i] = new Vector3(path[i].x, path[i].y, 0);
        }
        lr.SetPositions(pointList);
    }

    public void ChangeMovementValue() {
        ColorBlock temp = UIManager.ins.ControllerPanel_MovementButton.colors;
        if (isSelected) {
            temp.normalColor = new Color(1f, 1f, 1f);
            temp.highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f);
            UnSelectPlayerMovement();
        } else {
            temp.normalColor = temp.pressedColor;
            temp.highlightedColor = temp.normalColor;
            SelectPlayerMovement();
        }
        UIManager.ins.ControllerPanel_MovementButton.colors = temp;
    }


    public void SelectPlayerMovement() {
        isSelected = true;
        lr.enabled = true;
        //disable attacking
        if (attackLog.isSelected) {
            ChangeAttackValue();
            attackLog.ResetValues();
        }
        //disable speeching
        if (CombatManager.ins.combatSpeech.isSelected) {
            ChangeSpeechValue();
        }
        UIManager.ins.DisableLogPanelHover();
        UIManager.ins.DisableLogPanelHoverSpeech();
    }

    public void UnSelectPlayerMovement() {
        isSelected = false;
        lr.enabled = false;
        UIManager.ins.EnableLogPanelHover();
        UIManager.ins.EnableLogPanelHoverSpeech();
    }

    public void ChangeAttackValue() {
        ColorBlock temp = UIManager.ins.ControllerPanel_AttackButton.colors;
        if (attackLog.isSelected) {
            temp.normalColor = new Color(1f, 1f, 1f);
            temp.highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f);
            UnSelectPlayerAttack();
        } else {
            temp.normalColor = temp.pressedColor;
            temp.highlightedColor = temp.normalColor;
            SelectPlayerAttack();
        }
        UIManager.ins.ControllerPanel_AttackButton.colors = temp;
    }


    public void ChangeSpeechValue() {
        ColorBlock temp = UIManager.ins.ControllerPanel_SpeechButton.colors;
        if (CombatManager.ins.combatSpeech.isSelected) {
            temp.normalColor = new Color(1f, 1f, 1f);
            temp.highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f);
            CombatManager.ins.combatSpeech.RightClick();
        } else {
            temp.normalColor = temp.pressedColor;
            temp.highlightedColor = temp.normalColor;
            CombatManager.ins.combatSpeech.Select();
        }
        UIManager.ins.ControllerPanel_SpeechButton.colors = temp;
    }

    public void ResetAttackValue() {
        ColorBlock temp = UIManager.ins.ControllerPanel_AttackButton.colors;
        temp.normalColor = new Color(1f, 1f, 1f);
        temp.highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f);
        UIManager.ins.ControllerPanel_AttackButton.colors = temp;
        attackLog.isSelected = false;
    }

    public void SelectPlayerAttack() {
        //disable movement
        if (isSelected) {
            ChangeMovementValue();
        }
        //disable speeching
        if (CombatManager.ins.combatSpeech.isSelected) {
            ChangeSpeechValue();
        }
        attackLog.isSelected = true;
        UIManager.ins.EnableLogPanelBase();
        UIManager.ins.DisableLogPanelHover();
        UIManager.ins.DisableLogPanelHoverSpeech();
        UIManager.ins.DisableSpeechCombat();
    }

    public void UnSelectPlayerAttack() {
        attackLog.isSelected = false;
        UIManager.ins.DisableLogPanelBase();
        UIManager.ins.EnableLogPanelHover();
        UIManager.ins.EnableLogPanelHoverSpeech();
        attackLog.ResetValues();
    }

    public void VerifyAttacksOnPath() {
        List<CombatHUDAttack.Attack> toRemove = new List<CombatHUDAttack.Attack>();
        foreach(CombatHUDAttack.Attack a in attackLog.loggedAttacks) {
            bool found = false;
            for(int i = 0; i < log.loggedMoves.Count; i++) {
                if(a.hash == log.loggedMoves[i].hash) {
                    found = true;
                }
            }
            if (!found) {
                toRemove.Add(a);
            }
        }
        foreach(CombatHUDAttack.Attack b in toRemove) {
            Destroy(attackLog.loggedAttacks[attackLog.loggedAttacks.IndexOf(b)].attackObject);
            attackLog.RemoveAttackFromLayout(b);
            attackLog.loggedAttacks.Remove(b);
        }
    }

}
