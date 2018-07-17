using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {

    public static UIManager ins;

    public RectTransform TabPanel;
    public RectTransform InvPanel;
    public RectTransform InvBackground, InvForeground, InvGridLayout, InventorySlotPrefab, InvItemInfoPanel;
    public RectTransform ChoicePanel;
    public RectTransform MerchantPanel;
    private float displayTimeOffset = 0.25f;
    private float displayTime;
    private bool infoIsDisplayed;
    private bool beganTimer;

    public GameObject player;
    private PlayerMovementScript playerMovement;


    void Start() {

        //make singleton
        if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }

        //set the player object and his movement script
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovementScript>();
    }


    void Update() {

        //check to open/close menu elements
        if (Input.GetKeyDown(KeyCode.Tab)) {
            TabPanel.gameObject.SetActive(!TabPanel.gameObject.activeInHierarchy);
        } //end

        //perform checks to display the item info
        if (InvPanel.gameObject.activeInHierarchy) {
            //if item stats panel is not active, check to see if we can
            if (!InvItemInfoPanel.gameObject.activeInHierarchy) {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);

                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                bool found = false;
                GameObject itemFound = null;
                //if it hits a layer, check if it is not UI, then close inventory; close in else if not hitting layer
                if (results.Count > 0) {
                    foreach (RaycastResult hit in results) {
                        if (hit.gameObject.name == (InventorySlotPrefab.gameObject.name + "(Clone)")) {
                            found = true;
                            itemFound = hit.gameObject;
                        }
                    }
                    if (found) {
                        DisplayItemInfo(itemFound.GetComponentInChildren<DisplayItemInInventory>().item, itemFound.GetComponent<RectTransform>());
                    } else {
                        HideInfo();
                    }
                }
            } else { //if item stats panel is active, check to see if move off it
                PointerEventData pointerData = new PointerEventData(EventSystem.current);

                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                bool found = false;
                //if it hits a layer, check if it is not UI, then close inventory; close in else if not hitting layer
                if (results.Count > 0) {
                    foreach (RaycastResult hit in results) {
                        if (hit.gameObject.name == (InventorySlotPrefab.gameObject.name + "(Clone)")) {
                            found = true;
                        }
                    }
                    if (!found) {
                        HideInfo();
                    }
                }
            }
        } //end

    }

    //close the inventory panel
    public void CloseInventory() {
        InvPanel.gameObject.SetActive(false);
    }

    //re-populate the inventory screen and show it
    public void ShowInventory() {
        DePopulateInventoryScreen();
        PopulateInventoryScreen();
        TabPanel.gameObject.SetActive(false);
        InvPanel.gameObject.SetActive(true);
    }

    //remove every item from the inventory screen so we don't get duplicates
    public void DePopulateInventoryScreen() {
        foreach(Transform child in InvGridLayout.transform) {
            Destroy(child.gameObject);
        }
    }

    //add every item in the players inventory to the screen
    public void PopulateInventoryScreen() {
        foreach(Item item in player.GetComponent<PlayerInfo>().inventory) {
            RectTransform newItem = (RectTransform)Instantiate(InventorySlotPrefab, InvGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(item, newItem);
        }
    }

    //display the items info in the info panel
    public void DisplayItemInfo(Item item, RectTransform parent) {
        //check to see if the players mouse hovered over long enough
        if (Timer()) {
            //set the panel's position
            InvItemInfoPanel.GetComponent<DisplayItemInfo>().MakePanelChild(parent);
            //set the panel's info
            InvItemInfoPanel.GetComponent<DisplayItemInfo>().DisplayInfo(item);
            //set panel active
            InvItemInfoPanel.gameObject.SetActive(true);
        }
    }

    //determine if the mouse hovered over the item long enough
    bool Timer() {
        //check to see if the mouse has started to hover
        if (!beganTimer) {
            //if not, begin the timer
            beganTimer = true;
            displayTime = Time.time + displayTimeOffset;
        } else if (Time.time >= displayTime) {
            //after beginining the timer, check to see if enough time has passed
            return true;
        }

        return false;
    }

    //hide the info panel (esp if the mouse moves off);
    public void HideInfo() {
        beganTimer = false;
        infoIsDisplayed = false;
        InvItemInfoPanel.gameObject.SetActive(false);
    }

    //just sets info panel active
    public void KeepInfoOpen() {
        InvItemInfoPanel.gameObject.SetActive(true);
    }

}
