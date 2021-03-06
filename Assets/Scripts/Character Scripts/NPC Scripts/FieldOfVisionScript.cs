﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVisionScript : MonoBehaviour {

    public NPCInfo npcInfo;
    public Stats myStats;
    public GameObject selfNpc;
    private GameObject player;
    public bool canSee;
    private Vector2 degreeVector;
    private bool needToCheck = true;

	// Use this for initialization
	void Start () {
        player = GameManagerScript.ins.player;
	}

    public bool NeedToCheck {
        set {
            needToCheck = value;
        }
        get {
            return needToCheck;
        }
    }

    void Update() {
        if (!needToCheck) {
            return;
        }
        SetDirection();
        CheckSight();
        CheckCombat();
    }

    void CheckSight() {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, npcInfo.ViewDistance, CombatManager.ins.characterTest);
        foreach(Collider2D c in hitColliders) {
            if(c.gameObject == player) {
                canSee = CanSeeTarget(player);
            } else {
                if (CanSeeTarget(c.gameObject) && FactionManagerScript.ins.DoesHate(npcInfo.faction, c.gameObject.GetComponent<NPCInfo>().faction) && !npcInfo.inCombat) {
                    CombatManager.ins.InitCombat(gameObject);
                }
            }
        }

    }

    public List<GameObject> GetVisibleSpells() {
        List<GameObject> returnList = new List<GameObject>();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, npcInfo.ViewDistance, CombatManager.ins.spellTest);
        foreach (Collider2D c in hitColliders) {
            if (CanSeeTarget(c.gameObject)) {
                returnList.Add(c.gameObject);
            }
        }
        return returnList;
    }

    void CheckCombat() {
        if (!canSee || npcInfo.combatCooldown) {
            return;
        }
        if(myStats.attitude <= -25) {
            print("Entering combat!");
            needToCheck = false;
            CombatManager.ins.InitCombat(selfNpc);
        }
 

    }


    public bool CanSeeTarget(GameObject target) {
        Vector3 targetDir = target.transform.position - transform.position;
        float distance = Vector3.Distance(target.transform.position, transform.position);
        float angle = Vector3.Angle(targetDir, degreeVector);
        if(angle <= (npcInfo.FOV / 2) && distance <= npcInfo.ViewDistance) {
            return true;
        } else {
            return false;
        }
    }


    void SetDirection() {
        switch (npcInfo.direction) {
            case (CharacterInfo.Direction.FRONT):
                transform.eulerAngles = new Vector3(0, 0, 0);
                degreeVector = Vector2.down;
                break;
            case (CharacterInfo.Direction.FRONTRIGHT):
                transform.eulerAngles = new Vector3(0, 0, 45);
                degreeVector = (Vector2.down + Vector2.right).normalized;
                break;
            case (CharacterInfo.Direction.RIGHT):
                transform.eulerAngles = new Vector3(0, 0, 90);
                degreeVector = Vector2.right;
                break;
            case (CharacterInfo.Direction.BACKRIGHT):
                transform.eulerAngles = new Vector3(0, 0, 135);
                degreeVector = (Vector2.up + Vector2.right).normalized;
                break;
            case (CharacterInfo.Direction.BACK):
                transform.eulerAngles = new Vector3(0, 0, 180);
                degreeVector = Vector2.up;
                break;
            case (CharacterInfo.Direction.BACKLEFT):
                transform.eulerAngles = new Vector3(0, 0, 225);
                degreeVector = (Vector2.up + Vector2.left).normalized;
                break;
            case (CharacterInfo.Direction.LEFT):
                transform.eulerAngles = new Vector3(0, 0, 270);
                degreeVector = Vector2.left;
                break;
            case (CharacterInfo.Direction.FRONTLEFT):
                transform.eulerAngles = new Vector3(0, 0, 315);
                degreeVector = (Vector2.down + Vector2.left).normalized;
                break;
        }
    }

}
