using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : Quest {

    public Item targetItem;
    public int targetCount;

    void RemoveItemsFromPlayerInventory() {
        player.GetComponent<PlayerInfo>().inventory.RemoveItem(targetItem, targetCount);
    }

    bool DoesPlayerHaveItems() {
        return player.GetComponent<PlayerInfo>().inventory.HasItemCountInInventory(targetItem, targetCount);
    }

}
