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

    public List<Item> itemsToSell = new List<Item>();
    public List<Item> itemsToBuy = new List<Item>();

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
        itemsToSell.Clear(); //clear items to sell list
        itemsToBuy.Clear(); //clear items to buy list
        playerMoney.text = player.GetComponent<PlayerInfo>().money.ToString(); //show player's money
        npcMoney.text = npc.GetComponent<NPCInfo>().merchantMoney.ToString(); //show merchant's money
        DisplayPlayerItems(); //display player's inventory (sellable) in player item panel
        DisplayNPCItems(); //display npc's inventory (sellable) in npc item panel
        merchantPanel.gameObject.SetActive(true); //set the merchant panel active
    }

    //displays players item in the player item panel
    void DisplayPlayerItems() {
        //goes through every item in the player's inventory
        foreach(Item item in player.GetComponent<PlayerInfo>().inventory) {
            //checks to see if it is sellable and/or if it has cost
            //needs to change cost check to account for merchant bias (how it changes price)
            if(!item.isSellable || item.baseItemCost == 0) {
                continue;
            }
            //instantiate item and set it as a grid layout child
            RectTransform newItem = (RectTransform)Instantiate(slotPrefab, playerGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(item, newItem);
        }
    }

    //displays npc items in the merchant item panel
    void DisplayNPCItems() {
        //goes through every item in the npc's inventory
        foreach (Item item in npc.GetComponent<NPCInfo>().merchantInventory) {
            //checks to see if it is sellable and/or if it has cost
            //needs to change cost check to account for merchant bias (how it changes price)
            if (!item.isSellable || item.baseItemCost == 0) {
                continue;
            }
            //instantiate item and set it as a grid layout child
            RectTransform newItem = (RectTransform)Instantiate(slotPrefab, merchantGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(item, newItem);
        }
    }

    //close the shop panel
    public void CloseShop() {
        itemsToBuy.Clear(); //clear the items to buy list
        itemsToSell.Clear(); //clear the items to sell list
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
        foreach(Item item in itemsToBuy) {
            npc.GetComponent<NPCInfo>().merchantInventory.Remove(item);
            player.GetComponent<PlayerInfo>().inventory.Add(item);
        }
        //remove items that the player sold from his inventory and add them to the npc's inventory
        foreach(Item item in itemsToSell) {
            npc.GetComponent<NPCInfo>().merchantInventory.Add(item);
            player.GetComponent<PlayerInfo>().inventory.Remove(item);
        }

        itemsToBuy.Clear(); //clear the items to buy
        itemsToSell.Clear(); //clear the items to sell
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

    //update text fields for buying and selling
    public void UpdateCost(bool isSelected, GameObject targetObject) {
        //checks to see if the item selected belonged to the player (which means we are selling)
        if(targetObject.transform.parent.parent.gameObject == playerItemPanel.gameObject) {
            //if we selected it, add its cost to the total sell cost and display that
            if (isSelected) {
                itemsToSell.Add(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
                totalSellCost += targetObject.GetComponentInChildren<DisplayItemInInventory>().item.baseItemCost;
                sellCost.text = "Total Sell Value: " + totalSellCost;
            } else {
                //if we unselected the item, remove its cost and update accordingly
                itemsToSell.Remove(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
                totalSellCost -= targetObject.GetComponentInChildren<DisplayItemInInventory>().item.baseItemCost;
                //display unique text if the sell cost is 0
                if (totalSellCost <= 0) {
                    totalSellCost = 0;
                    sellCost.text = "Click to Sell";
                } else {
                    sellCost.text = "Total Sell Value: " + totalSellCost;
                }
            }
        } else {
            //this would mean that we are buying
            //if we selected it, add its cost to the total buy cost and display that
            if (isSelected) {
                itemsToBuy.Add(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
                totalBuyCost += targetObject.GetComponentInChildren<DisplayItemInInventory>().item.baseItemCost;
                buyCost.text = "Total Buy Value: " + totalBuyCost;
            } else {
                //if we unselected the item, remove its cost and update accordingly
                itemsToBuy.Remove(targetObject.GetComponentInChildren<DisplayItemInInventory>().item);
                totalBuyCost -= targetObject.GetComponentInChildren<DisplayItemInInventory>().item.baseItemCost;
                //display unique text if the buy cost is 0
                if (totalBuyCost <= 0) {
                    totalBuyCost = 0;
                    buyCost.text = "Click to Buy";
                } else {
                    buyCost.text = "Total Buy Value: " + totalBuyCost;
                }
            }
        }
    }



}
