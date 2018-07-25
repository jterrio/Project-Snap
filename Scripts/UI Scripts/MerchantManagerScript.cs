using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantManagerScript : MonoBehaviour {

    public static MerchantManagerScript ins;

    public RectTransform merchantPanel;
    public RectTransform playerItemPanel, playerGridLayout;
    public RectTransform merchantItemPanel, merchantGridLayout;

    public TMPro.TextMeshProUGUI sellCost, buyCost;
    public TMPro.TextMeshProUGUI playerMoney, npcMoney;
    public float totalSellCost, totalBuyCost;

    public Inventory itemsToSell;
    public Inventory itemsToBuy;

    public RectTransform slotPrefab;

    private GameObject player, npc;

	// Use this for initialization
	void Start () {
        //make singleton
	    if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
	}

    //open the shop window
    public void Shop(GameObject player, GameObject npc) {
        ResetText(); //reset text fields for money and such
        ClearPlayerItems(); //clear player item panel for no duplicates
        ClearNPCItems(); //clear npc item panel for no duplicates
        this.player = player; //set target player
        this.npc = npc; //set target npc
        itemsToSell.ClearInventory();//clear items to sell list
        itemsToBuy.ClearInventory(); //clear items to buy list
        playerMoney.text = player.GetComponent<PlayerInfo>().money.ToString(); //show player's money
        npcMoney.text = npc.GetComponent<NPCInfo>().merchantMoney.ToString(); //show merchant's money
        DisplayPlayerItems(); //display player's inventory (sellable) in player item panel
        DisplayNPCItems(); //display npc's inventory (sellable) in npc item panel
        merchantPanel.gameObject.SetActive(true); //set the merchant panel active
    }

    //displays players item in the player item panel
    void DisplayPlayerItems() {
        //goes through every item in the player's inventory
        foreach(Inventory.InventorySlot inv in player.GetComponent<PlayerInfo>().inventory.inventory) {
            //checks to see if it is sellable and/or if it has cost
            //needs to change cost check to account for merchant bias (how it changes price)
            if(!inv.item.isSellable || inv.item.baseItemCost == 0) {
                continue;
            }
            //instantiate item and set it as a grid layout child
            RectTransform newItem = (RectTransform)Instantiate(slotPrefab, playerGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
            newItem.transform.GetComponent<BuySellScript>().SetValues(1, inv.count);
        }
    }

    //displays npc items in the merchant item panel
    void DisplayNPCItems() {
        //goes through every item in the npc's inventory
        foreach (Inventory.InventorySlot inv in npc.GetComponent<NPCInfo>().merchantInventory.inventory) {
            //checks to see if it is sellable and/or if it has cost
            //needs to change cost check to account for merchant bias (how it changes price)
            if (!inv.item.isSellable || inv.item.baseItemCost == 0) {
                continue;
            }
            //instantiate item and set it as a grid layout child
            RectTransform newItem = (RectTransform)Instantiate(slotPrefab, merchantGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
            newItem.transform.GetComponent<BuySellScript>().SetValues(1, inv.count);
        }
    }

    //close the shop panel
    public void CloseShop() {
        itemsToBuy.ClearInventory(); //clear the items to buy list
        itemsToSell.ClearInventory(); //clear the items to sell list
        merchantPanel.gameObject.SetActive(false); //set the merchant panel inactive
        ResetText(); //reset text fields, such as money
        totalBuyCost = 0; //reset total buy cost to 0 for next time
        totalSellCost = 0; //reset total sell cost to 0 for next time
    }

    //exchange items between merchant and player
    public void ExchangeItems() {

        /*Checks multiple things:
         * -checks to see if the player can afford to buy the items he selected based on his money + how much he is selling to the npc
         * -checks to see if the npc can afford to buy the items the player is selling based on npc money + how much he is selling to the player
         * -if either is impossible, throw an error to the player
         * */
        if(totalBuyCost > (player.GetComponent<PlayerInfo>().money + totalSellCost) || totalSellCost > (npc.GetComponent<NPCInfo>().merchantMoney + totalBuyCost)) {
            return; //throw error to player
        }
        //if we are here, then the transaction is possible
       
        player.GetComponent<PlayerInfo>().money -= totalBuyCost; //subtract buy cost
        npc.GetComponent<NPCInfo>().merchantMoney += totalBuyCost; //add money to merchant

        player.GetComponent<PlayerInfo>().money += totalSellCost; //add sell cost
        npc.GetComponent<NPCInfo>().merchantMoney -= totalSellCost; //take money from merchant

        //remove items that the player bought from the npc's inventory and add them to the player's inventory
        /*
         * 
         * 
         * 
         * 
         * 
         * WORK ON - DO PARTIAL TRANSACTION
         * 
         * 
         * 
         * */
        foreach(Inventory.InventorySlot inv in itemsToBuy.inventory) {
            npc.GetComponent<NPCInfo>().merchantInventory.RemoveItem(inv.item, inv.count);
            player.GetComponent<PlayerInfo>().inventory.AddItemCount(inv.item, inv.count);
        }
        //remove items that the player sold from his inventory and add them to the npc's inventory
        foreach(Inventory.InventorySlot inv in itemsToSell.inventory) {
            player.GetComponent<PlayerInfo>().inventory.RemoveItem(inv.item, inv.count);
            npc.GetComponent<NPCInfo>().merchantInventory.AddItemCount(inv.item, inv.count);
        }

        itemsToBuy.ClearInventory(); //clear the items to buy
        itemsToSell.ClearInventory(); //clear the items to sell
        ResetText(); //reset text field
        ClearPlayerItems(); //clear the player items panel
        ClearNPCItems(); //clear the npc items panel
        DisplayNPCItems(); //re-display npc items
        DisplayPlayerItems(); //re-display player items
        playerMoney.text = player.GetComponent<PlayerInfo>().money.ToString(); //update player money
        npcMoney.text = npc.GetComponent<NPCInfo>().merchantMoney.ToString(); //update npc money
        totalBuyCost = 0; //set buy cost to 0
        totalSellCost = 0; //set sell cost to 0

    }

    //destory items in gridlayout for player
    void ClearPlayerItems() {
        foreach(Transform child in playerGridLayout.transform) {
            Destroy(child.gameObject);
        }
    }

    //destory items in gridlayout for npc
    void ClearNPCItems() {
        foreach (Transform child in merchantGridLayout.transform) {
            Destroy(child.gameObject);
        }
    }
    
    //reset the text
    void ResetText() {
        sellCost.text = "Click to Sell";
        buyCost.text = "Click to Buy";
    }

    public void AddItemToBuy(Item item) {
        itemsToBuy.AddItem(item);
    }

    public void AddItemToSell(Item item) {
        itemsToSell.AddItem(item);
    }

    public void DeleteItemFromBuy(Item item) {
        itemsToBuy.RemoveAllItem(item);
    }

    public void DeleteItemFromSell(Item item) {
        itemsToSell.RemoveAllItem(item);
    }

    public void CalculateSellCost() {
        totalSellCost = 0;
        foreach (Inventory.InventorySlot inv in itemsToSell.inventory) {
            totalSellCost += inv.item.baseItemCost * inv.count;
        }
        if (totalSellCost <= 0) {
            totalSellCost = 0;
            sellCost.text = "Click to Sell";
        } else {
            sellCost.text = "Total Sell Value: " + totalSellCost;
        }
    }

    public void CalculateBuyCost() {
        totalBuyCost = 0;
        foreach(Inventory.InventorySlot inv in itemsToBuy.inventory) {
            totalBuyCost += inv.item.baseItemCost * inv.count;
        }
        if (totalBuyCost <= 0) {
            totalBuyCost = 0;
            buyCost.text = "Click to Buy";
        } else {
            buyCost.text = "Total Buy Value: " + totalBuyCost;
        }
    }

    //update text fields for buying and selling
    public int UpdateCost(bool isSelected, GameObject targetObject) {
        //checks to see if the item selected belonged to the player (which means we are selling)
        int i = 0;
        if (targetObject.transform.parent.parent.gameObject == playerItemPanel.gameObject) {
            //if we selected it, add its cost to the total sell cost and display that
            if (isSelected) {

                i = itemsToSell.AddItem(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
            } else {
                //if we unselected the item, remove its cost and update accordingly
                i = itemsToSell.RemoveAllItem(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
            }
            CalculateSellCost();
        } else {
            //this would mean that we are buying
            //if we selected it, add its cost to the total buy cost and display that
            if (isSelected) {
                i = itemsToBuy.AddItem(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
            } else {
                //if we unselected the item, remove its cost and update accordingly
                i = itemsToBuy.RemoveAllItem(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
               
            }
            CalculateBuyCost();
        }
        return i;
    }



}
