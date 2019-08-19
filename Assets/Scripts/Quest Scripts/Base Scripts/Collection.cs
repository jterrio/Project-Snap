using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : QuestObjective {

    public Item targetItem;
    public int targetCount;

    void RemoveItemsFromPlayerInventory() {
        GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.RemoveItem(targetItem, targetCount);
    }

    bool DoesPlayerHaveItems() {
        return GameManagerScript.ins.player.GetComponent<PlayerInfo>().inventory.HasItemCountInInventory(targetItem, targetCount);
    }

}
