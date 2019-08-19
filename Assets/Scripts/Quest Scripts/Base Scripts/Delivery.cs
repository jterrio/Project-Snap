using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : QuestObjective {

    public Item deliveryItem;
    public NPC deliveryTarget;

    //add the key item to the players inventory
    void GiveItem() {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.AddItem(deliveryItem);
    }


    //remove the item from their inventory
    void RemoveItem() {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.RemoveAllItem(deliveryItem);
    }

    //check to see if the key item is in the player's inventory
    bool isKeyItemInInventory() {
        return GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.HasItemInInventory(deliveryItem);
    }

    //check to see if the target npc is the npc for the quest
    bool hasReachedTarget(NPC target) {
        if(target == deliveryTarget) {
            return true;
        }
        return false;
    }
	
}
