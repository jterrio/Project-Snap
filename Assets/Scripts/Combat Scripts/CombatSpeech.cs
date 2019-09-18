using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatSpeech : MonoBehaviour {

    public bool isSelected = false; //selected by button
    public bool hasClicked = false; //has clicked on selectedNPC
    public GameObject selectedNPC; //selected by mouse hovering over
    public GameObject savedNPC; //selected NPC that choices will affect
    public GameObject buttonPrefab;
    public RectTransform gridlayout; //layout for buttons for choices
    public RectTransform gridlayoutGivenOrders; //show the queue of selected choices
    public GameObject gridlayoutItem;
    public List<GivenOrder> givenOrders;
    private Coroutine speechCoroutine;
    public int progress;
    public bool wasLoaded;
    public GameObject watchAreaPrefab, guardAreaPrefab;
    public RectTransform bribePrefab;
    public RectTransform bribePanel, bribeGridLayout, bribeText;
    public Button bribeButton;
    public string rejectBribe = "They will NOT accept this offer.";
    public string acceptBribe = "They will accept this offer.";
    public string close = "Close";
    public string gift = "Bribe";
    public Inventory itemsForBribe;

    public class GivenOrder {
        public GameObject npc;
        public Order o;
        public GameObject child;
        public GameObject standArea;
        public GameObject watchArea;
    }

    public enum Order {
        GUARD, //0
        WATCH, //1
        SENTRY, //2
        TAUNT, //3
        BUY, //4
        INTIMIDATE, //5
        NOTFOLLOWING //6
    }

    private void Start() {
        givenOrders = new List<GivenOrder>();
    }

    // Update is called once per frame
    void Update() {
        ContinueSpeechQueue();
        if (!isSelected) {
            return;
        }

        NPCSelectCheck();

        VerifySelectedNPC();
        ClickCheck();
    }

    void ContinueSpeechQueue() {
        if (givenOrders.Count < 1) {
            return;
        }
        if (speechCoroutine == null) {
            speechCoroutine = StartCoroutine(SpeechQueue());
        }

    }

    IEnumerator SpeechQueue() {
        //grab info from the speech
        Image child = givenOrders[0].child.GetComponentInChildren<Image>();

        //1 second
        if (!wasLoaded) {
            for (progress = 0; progress < 100; progress++) {

                yield return new WaitForSeconds(0.01f);
                child.fillAmount = (float)progress / 100f;
            }
        } else {
            wasLoaded = false;
            for (progress = progress; progress < 100; progress++) {

                yield return new WaitForSeconds(0.01f);
                child.fillAmount = (float)progress / 100f;
            }
        }
        //Cast and Reset
        child.fillAmount = 1;

        //cast - SEND ORDERS
        if (givenOrders[0].o != Order.BUY) {
            givenOrders[0].npc.GetComponent<CombatScript>().SetOrders(givenOrders[0]);
            RestartOrderQueue();
        } else {
            OpenBribePanel();
        }
    }

    public void RemoveFromQueue(RectTransform r) {
        foreach(GivenOrder go in givenOrders) {
            if(go.child == r.gameObject) {
                if(givenOrders.IndexOf(go) == 0 && speechCoroutine != null) {
                    StopCoroutine(speechCoroutine);
                    speechCoroutine = null;
                }
                givenOrders.Remove(go);
                Destroy(go.standArea);
                Destroy(go.watchArea);
                Destroy(r.gameObject);
                return;
            }
        }
    }

    void RestartOrderQueue() {
        //Remove from UI
        //Remove from queue and reset the coroutine to know that it is finished
        RemoveFromOrders(givenOrders[0]);
        if(speechCoroutine != null) {
            StopCoroutine(speechCoroutine);
            speechCoroutine = null;
        }
        
    }


    public void RemoveFromOrders(GivenOrder o) {

        foreach (Transform child in gridlayoutGivenOrders) {
            if (child.gameObject == o.child) {
                if (o.standArea != null) {
                    Destroy(o.standArea);
                }
                if (o.watchArea != null) {
                    Destroy(o.watchArea);
                }
                Destroy(child.gameObject);
                givenOrders.Remove(o);
                return;
            }
        }

    }

    public void RemoveAllOrders() {
        foreach(GivenOrder o in givenOrders) {
            Destroy(o.child.gameObject);
            if(o.standArea != null) {
                Destroy(o.standArea);
            }
            if(o.watchArea != null) {
                Destroy(o.watchArea);
            }
        }
        givenOrders.Clear();
    }

    void ClickCheck() {
        //check if we click
        if (selectedNPC != null && Input.GetMouseButtonDown(0)) {
            if ((Vector3.Distance(selectedNPC.transform.position, GameManagerScript.ins.player.transform.position) > 10)) {
                return;
            }
            hasClicked = true;
            UIManager.ins.EnableLogPanelSpeech();
            UIManager.ins.ShowSpeechLogMain();
            UIManager.ins.DisableLogPanelHoverSpeech();
            UIManager.ins.DisableLogPanelHover();
            UIManager.ins.DisableLogPanelBase();
            savedNPC = selectedNPC;
            DestroyChoices();
            PopulateChoices();
        }
        if (hasClicked && Input.GetMouseButtonDown(1)) {
            DestroyChoices();
            UIManager.ins.DisableLogPanelSpeech();
            UIManager.ins.DisableLogPanelBase();
            UIManager.ins.EnableLogPanelHover();
            UIManager.ins.EnableLogPanelHoverSpeech();
            hasClicked = false;
            savedNPC = null;
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
            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero, Mathf.Infinity, CombatManager.ins.characterTest);
            if (hit.collider != null) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")) {
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
        if (selectedNPC == t) {
            VerifySelectedNPC();
        }
        if (selectedNPC != null) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
        }
        selectedNPC = t;
        if (selectedNPC != null) {
            if (Vector3.Distance(t.transform.position, GameManagerScript.ins.player.transform.position) > 10) {
                selectedNPC.GetComponent<Renderer>().material.color = Color.red;
            } else {
                selectedNPC.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    public void VerifySelectedNPC() {
        if (selectedNPC == null) {
            return;
        }
        if (Vector3.Distance(selectedNPC.transform.position, GameManagerScript.ins.player.transform.position) > 10) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void DestroyChoices() {
        foreach (Transform child in gridlayout) {
            Destroy(child.gameObject);
        }
    }

    public void PopulateChoices() {
        NPCInfo info = savedNPC.GetComponent<NPCInfo>();
        Stats s = savedNPC.GetComponent<Stats>();
        if (s.attitude >= 0) {
            GameObject g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(0);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Guard";

            g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(1);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Watch";

            g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(2);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Sentry";
        } else {
            GameObject g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(3);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Taunt";

            g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(4);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Bribe";

            g = (GameObject)Instantiate(buttonPrefab, gridlayout);
            g.GetComponent<CombatSpeechChoiceScript>().SetChoice(5);
            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Intimidate";
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
    }

    public void RightClick() {
        if (selectedNPC != null) {
            selectedNPC.GetComponent<Renderer>().material.color = Color.white;
            selectedNPC = null;
        }
        savedNPC = null;
        isSelected = false;
        hasClicked = false;
        DestroyChoices();
        UIManager.ins.DisableLogPanelSpeech();
        UIManager.ins.DisableLogPanelBase();
        UIManager.ins.EnableLogPanelHover();
        UIManager.ins.EnableLogPanelHoverSpeech();
    }

    public void ChoiceSelected(int i) {
        DestroyChoices();
        GivenOrder go = new GivenOrder();
        go.npc = savedNPC;
        go.o = (Order)i;
        GameObject c = Instantiate(gridlayoutItem, gridlayoutGivenOrders);
        go.child = c;
        CreateObjects(go);
        //update collider when timescale is 0
        UpdateColliders(go);

        givenOrders.Add(go);
        go.child.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = GetChoiceText(i);
        CombatManager.ins.combatDrawMovePosition.ChangeSpeechValue();
    }

    public void ImportOrderData(uint ID, CombatSpeech.Order o, Vector3 standAreaPos, Vector3 watchAreaPos) {
        GivenOrder go = new GivenOrder();
        go.npc = NPCManagerScript.ins.GetNPCInSceneFromID(ID);
        go.o = o;
        GameObject c = Instantiate(gridlayoutItem, gridlayoutGivenOrders);
        go.child = c;
        CreateObjects(go);
        if (go.standArea != null) {
            go.standArea.transform.position = standAreaPos;
        }
        if (go.watchArea != null) {
            go.watchArea.transform.position = watchAreaPos;
        }
        UpdateColliders(go);
        givenOrders.Add(go);
        go.child.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = GetChoiceText((int)o);
    }

    public void UpdateColliders(GivenOrder go) {
        if (go.standArea != null) {
            go.standArea.GetComponent<BoxCollider2D>().isTrigger = false;
            go.standArea.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        if (go.watchArea != null) {
            go.watchArea.GetComponent<BoxCollider2D>().isTrigger = false;
            go.watchArea.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void CreateObjects(GivenOrder go) {
        switch (go.o) {
            case Order.BUY:
                return;
            case Order.GUARD:
                go.standArea = Instantiate(guardAreaPrefab);
                go.standArea.transform.position = go.npc.transform.position;
                return;
            case Order.INTIMIDATE:
                return;
            case Order.SENTRY:
                go.standArea = Instantiate(guardAreaPrefab);
                go.standArea.transform.position = go.npc.transform.position;
                go.watchArea = Instantiate(watchAreaPrefab);
                go.watchArea.transform.position = GameManagerScript.ins.player.transform.position;
                return;
            case Order.TAUNT:
                return;
            case Order.WATCH:
                go.watchArea = Instantiate(watchAreaPrefab);
                go.watchArea.transform.position = GameManagerScript.ins.player.transform.position;
                return;
        }
    }


    string GetChoiceText(int i) {
        switch (i) {
            case 0:
                return "Guard";
            case 1:
                return "Watch";
            case 2:
                return "Sentry";
            case 3:
                return "Taunt";
            case 4:
                return "Bribe";
            case 5:
                return "Intimidate";
            case 6:
                return "Cancel";
            default:
                return "BROKEN";
        }
    }

    public void OpenBribePanel() {
        bribePanel.gameObject.SetActive(true);
        DisplayPlayerItems();
        CalculateBribeText();
    }

    public void CloseBribePanel() {
        bribePanel.gameObject.SetActive(false);
        if (CalculateBribeText()) {
            foreach(Inventory.InventorySlot invSlot in itemsForBribe.inventory) {
                GameManagerScript.ins.playerInventory.RemoveItem(invSlot.item, invSlot.count);
                givenOrders[0].npc.GetComponent<Inventory>().AddItemCount(invSlot.item, invSlot.count);
            }
            float i = 0;
            foreach (Inventory.InventorySlot invSlot in itemsForBribe.inventory) {
                i += (invSlot.count * invSlot.item.baseItemCost);
            }
            i -= (-1 * givenOrders[0].npc.GetComponent<Stats>().attitude * 10);
            givenOrders[0].npc.GetComponent<CombatScript>().OrderLeaveCombat();
            if (i > 0) {
                givenOrders[0].npc.GetComponent<Stats>().ChangeAttitude(Mathf.FloorToInt(i));
            }
        }
        foreach(Transform c in bribeGridLayout) {
            Destroy(c.gameObject);
        }
        itemsForBribe.ClearInventory();
        RestartOrderQueue();
    }

    /// <summary>
    /// Display player items in the bribe window
    /// </summary>
    void DisplayPlayerItems() {
        //goes through every item in the player's inventory
        foreach (Inventory.InventorySlot inv in GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.inventory) {
            //checks to see if it is sellable and/or if it has cost
            //needs to change cost check to account for merchant bias (how it changes price)
            if (!inv.item.isSellable || inv.item.baseItemCost == 0) {
                continue;
            }
            //instantiate item and set it as a grid layout child
            RectTransform newItem = (RectTransform)Instantiate(bribePrefab, bribeGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
            newItem.transform.GetComponent<CombatBribeScript>().SetValues(1, inv.count);
        }
    }

    public void AddItem(GameObject c) {
        Item i = c.GetComponentInChildren<DisplayItemInInventory>().item;
        itemsForBribe.AddItem(i);
        CalculateBribeText();
    }

    public void RemoveItem(GameObject c) {
        Item i = c.GetComponentInChildren<DisplayItemInInventory>().item;
        itemsForBribe.RemoveItem(i, 1);
        CalculateBribeText();
    }

    public void RemoveAllItems(GameObject c) {
        Item i = c.GetComponentInChildren<DisplayItemInInventory>().item;
        itemsForBribe.RemoveAllItem(i);
        CalculateBribeText();
    }

    public bool CalculateBribeText() {
        float i = 0;
        foreach (Inventory.InventorySlot invSlot in itemsForBribe.inventory) {
            i += (invSlot.count * invSlot.item.baseItemCost);
        }
        if (i >= (-1 * givenOrders[0].npc.GetComponent<Stats>().attitude * 5)) {
            bribeText.GetComponent<TMPro.TextMeshProUGUI>().text = acceptBribe;
            bribeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = gift;
            return true;
        } else {
            bribeText.GetComponent<TMPro.TextMeshProUGUI>().text = rejectBribe;
            bribeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = close;
            return false;
        }
    }
}
