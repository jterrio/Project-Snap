using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatSpeech : MonoBehaviour{

    public bool isSelected = false; //selected by button
    public bool hasClicked = false; //has clicked on selectedNPC
    public GameObject selectedNPC; //selected by mouse hovering over
    public RectTransform buttonPrefab;
    public RectTransform gridlayout;

    public enum Order {
        GUARD, //0
        WATCH, //1
        SENTRY, //2
        TAUNT, //3
        BUY, //4
        INTIMIDATE //5
    }

    // Update is called once per frame
    void Update(){
        if (!isSelected) {
            return;
        }

        NPCSelectCheck();

        VerifySelectedNPC();
        ClickCheck();
    }

    void ClickCheck() {
        //check if we click
        if (selectedNPC != null && Input.GetMouseButtonDown(0)) {
            if((Vector3.Distance(selectedNPC.transform.position, GameManagerScript.ins.player.transform.position) > 10)) {
                return;
            }
            hasClicked = true;
            PopulateChoices();
        }
    }

    void NPCSelectCheck() {
        //check if we need to select an NPC
        if (isSelected) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult r in results) {
                if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                    return;
                }
            }
            Vector2 mouse2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero);
            if(hit.collider != null) {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")) {
                    ChangeSelectedNPC(hit.collider.gameObject);
                    return;
                }
            }
            ChangeSelectedNPC(null);

        }
    }


    public void ChangeSelection(bool v) {
        isSelected = v;
    }

    public void ReverseSelection() {
        isSelected = !isSelected;
    }

    public void ChangeSelectedNPC(GameObject t) {
        if(selectedNPC == t) {
            VerifySelectedNPC(); 
        }
        if(selectedNPC != null) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
        }
        selectedNPC = t;
        if(selectedNPC != null) {
            if (Vector3.Distance(t.transform.position, GameManagerScript.ins.player.transform.position) > 10) {
                selectedNPC.GetComponent<Renderer>().material.color = Color.red;
            } else {
                selectedNPC.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    public void VerifySelectedNPC() {
        if(selectedNPC == null) {
            return;
        }
        if (Vector3.Distance(selectedNPC.transform.position, GameManagerScript.ins.player.transform.position) > 10) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void DestroyChoices() {

    }

    public void PopulateChoices() {
        NPCInfo info = selectedNPC.GetComponent<NPCInfo>();
        Stats s = selectedNPC.GetComponent<Stats>();
        if(s.attitude < 0) {
            
        } else {

        }
    }

    public void Select() {
        isSelected = true;
        //disable movement
        if (CombatManager.ins.combatDrawMovePosition.isSelected) {
            CombatManager.ins.combatDrawMovePosition.ChangeMovementValue();
        }
        //disable attacking
        if (CombatManager.ins.combatHUDAttack.isSelected) {
            CombatManager.ins.combatDrawMovePosition.ChangeAttackValue();
            CombatManager.ins.combatHUDAttack.ResetValues();
        }
        UIManager.ins.EnableLogPanelSpeech();
        UIManager.ins.DisableLogPanelHoverSpeech();
        UIManager.ins.DisableLogPanelHover();
        UIManager.ins.DisableLogPanelBase();
    }

    public void RightClick() {
        if(selectedNPC != null) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
            selectedNPC = null;
        }
        isSelected = false;
        hasClicked = false;
        DestroyChoices();
        UIManager.ins.DisableLogPanelSpeech();
        UIManager.ins.DisableLogPanelBase();
        UIManager.ins.EnableLogPanelHover();
        UIManager.ins.EnableLogPanelHoverSpeech();
    }

    public void ChoiceSelected(int i) {
        Order o = (Order)i;
    }
}
