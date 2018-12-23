using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerDrawCombatMovePosition : MonoBehaviour {

    public bool isSelected;
    public LineRenderer lr;
    private PolyNav.PolyNavAgent playerAgent;
    private CombatHUDLog log;
    private CombatHUDAttack attackLog;
    private Vector3[] loggedPath;
    private bool canMove = false;

    void Start() {
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        playerAgent = GameManagerScript.ins.player.GetComponent<PolyNav.PolyNavAgent>();
        log = GetComponent<CombatHUDLog>();
        attackLog = GetComponent<CombatHUDAttack>();
    }


    void Update() {
        DisplayLoggedPath();

        if (!isSelected) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if(results.Count > 0) {
                if(results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
                    return;
                }
            }
            MouseClick();
            if (!Input.GetKey(KeyCode.LeftShift) && canMove) {
                ChangeMovementValue();
            }
            return;
        }

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

    void LogPath(Vector2[] path) {
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
            return;
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
        if (attackLog.isSelected) {
            ChangeAttackValue();
            attackLog.ResetValues();
        }
    }

    public void UnSelectPlayerMovement() {
        isSelected = false;
        //lr.enabled = false;
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

    public void ResetAttackValue() {
        ColorBlock temp = UIManager.ins.ControllerPanel_AttackButton.colors;
        temp.normalColor = new Color(1f, 1f, 1f);
        temp.highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f);
        UIManager.ins.ControllerPanel_AttackButton.colors = temp;
        attackLog.isSelected = false;
    }

    public void SelectPlayerAttack() {
        if (isSelected) {
            ChangeMovementValue();
        }
        attackLog.isSelected = true;
    }

    public void UnSelectPlayerAttack() {
        attackLog.isSelected = false;
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
            attackLog.RemoveAttackFromLayout(b);
            Destroy(attackLog.loggedAttacks[attackLog.loggedAttacks.IndexOf(b)].attackObject);
            attackLog.loggedAttacks.Remove(b);
        }
    }

}
