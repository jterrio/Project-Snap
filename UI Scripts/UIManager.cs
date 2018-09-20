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
    public RectTransform CombatHUDPanel, LogPanel_Base, LogPanel_Main, LogPanel_gridLayout, LogPanel_Attack, LogPanel_Attack_gridLayout, LogPanel_Attack_Icon_Prefab;
    public Button ControllerPanel_MovementButton, ControllerPanel_AttackButton;
    public RectTransform HealthandStaminaPanel;
    public Image HealthImageFilled, StaminaImageFilled;
    public RectTransform AITurnProgressPanel;
    public Image AITurnProgressImage;

    public RectTransform speechOutline;
    public RectTransform speechPanel;
    public RectTransform talkerNamePanel;
    public RectTransform talkerTextPanel;
    public RectTransform choicePanel;
    public string textToDisplay; //for the coroutine

    public RectTransform ChestPanel, ChestPlayerPanel, ChestChestPanel, ChestPlayerGridLayout, ChestChestGridLayout, ChestItemPrefab;
    public TMPro.TextMeshProUGUI chestItemCountText, chestPlayerItemCountText;

    private float displayTimeOffset = 0.25f;
    private float displayTime;
    private bool infoIsDisplayed;
    private bool beganTimer;

    public GameObject player;
    public GameObject targetChest;
    private PlayerMovementScript playerMovement;


    void Start() {

        //make singleton
        if (ins == null) {
            ins = this;
        } else {
            if (ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);

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
        foreach (Transform child in InvGridLayout.transform) {
            Destroy(child.gameObject);
        }
    }

    //add every item in the players inventory to the screen
    public void PopulateInventoryScreen() {
        foreach (Inventory.InventorySlot inv in player.GetComponent<PlayerInfo>().inventory.inventory) {
            RectTransform newItem = (RectTransform)Instantiate(InventorySlotPrefab, InvGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
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

    //display item picked up
    public bool DisplayItemPickup(Item item) {
        if (!SpeechManager.ins.PickUpItem(item)) {
            playerMovement.CanPlayerMove = false;
            return false;
        }
        playerMovement.CanPlayerMove = true;
        return true;
    }

    //close item pick up
    public void CloseItemPickup() {

    }

    public void OpenChest(GameObject chest) {
        targetChest = chest;
        TabPanel.gameObject.SetActive(false);
        ChestPanel.gameObject.SetActive(true);
        playerMovement.CanPlayerMove = false;
    }

    public void CloseChest() {
        targetChest.GetComponent<ChestScript>().CloseChest();
        targetChest = null;
        ChestPanel.gameObject.SetActive(false);
        playerMovement.CanPlayerMove = true;
        player.GetComponentInChildren<PlayerChestScript>().inChest = false;
    }

    public void UpdateChestItemCount(int current, int max) {
        chestItemCountText.text = current.ToString() + "/" + max.ToString();
    }

    public void UpdateChestPlayerItemCount(int current, int max) {
        chestPlayerItemCountText.text = current.ToString() + "/" + max.ToString();
    }

    public void EnableCombatHUD() {
        CombatHUDPanel.gameObject.SetActive(true);
        HealthandStaminaPanel.gameObject.SetActive(true);
    }

    public void DisableCombatHUD() {
        CombatHUDPanel.gameObject.SetActive(false);
        HealthandStaminaPanel.gameObject.SetActive(false);
    }

    public void HideLogPanel() {
        LogPanel_Main.gameObject.SetActive(false);
    }

    public void ShowLogPanel() {
        HideAttackPanel();
        LogPanel_Main.gameObject.SetActive(true);
    }

    public void ShowAttackPanel() {
        HideLogPanel();
        LogPanel_Attack.gameObject.SetActive(true);
        foreach(Transform child in LogPanel_Attack_gridLayout.transform) {
            Destroy(child.gameObject);
        }
        foreach(Spell spell in GameManagerScript.ins.playerSpells.Spells()) {
            RectTransform newSpell = Instantiate(LogPanel_Attack_Icon_Prefab, LogPanel_Attack_gridLayout);
            newSpell.GetChild(0).GetComponent<DisplaySpellInfo>().Display(spell, newSpell);
        }
    }

    public void HideAttackPanel() {
        LogPanel_Attack.gameObject.SetActive(false);
    }

    public void EnableAITurnProgress() {
        //AITurnProgressPanel.gameObject.SetActive(true);
    }

    public void DisableAITurnProgress() {
        //AITurnProgressPanel.gameObject.SetActive(false);
    }

    public void UpdateAITurnProgress(int amount, int max) {
        //float progress = (float)amount / (float)max;
        //AITurnProgressImage.fillAmount = progress;
    }



}
