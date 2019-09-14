using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<InventorySlot> inventory;
    private int size = 0;
    public int maxSize;

    [System.Serializable]
    public class InventorySlot {
        public Item item;
        public int count;

        public void CreateItemSlot(Item item, int count) {
            this.item = item;
            this.count = count;
        }

    }


    void Start() {
        CalculateSize();
    }

    void CalculateSize() {
        size = 0;
        foreach(InventorySlot inv in inventory) {
            size += inv.count;
        }
    }

    /// <summary>
    /// add item to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int AddItem(Item item) {
        if(size >= maxSize) {
            return -1; //throw error to the player
        } else {
            size += 1;
        }
        if (item.isStackable) { //if the item is stackable, see if the item exists in inventory so we can stack it
            foreach (InventorySlot inv in inventory) {
                if (inv.item.ID == item.ID) {
                    inv.count += 1; //once we find the item to stack, return
                    return inv.count;
                }
            }
        }
        //if the item is not stackable or is not in the inventory, create a new slot for it
        InventorySlot temp = new InventorySlot();
        temp.CreateItemSlot(item, 1);
        inventory.Add(temp);
        return 1;
    }

    public int AddItemCount(Item item, int count) {
        if (size + count > maxSize) {
            return -1; //throw error to the player
        } else {
            size += count;
        }
        if (item.isStackable) { //if the item is stackable, see if the item exists in inventory so we can stack it
            foreach (InventorySlot inv in inventory) {
                if (inv.item.ID == item.ID) {
                    inv.count += count; //once we find the item to stack, return
                    return inv.count;
                }
            }
        }
        //if the item is not stackable or is not in the inventory, create a new slot for it
        InventorySlot temp = new InventorySlot();
        temp.CreateItemSlot(item, count);
        inventory.Add(temp);
        return count;
    }

    /// <summary>
    /// remove an item to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removeCount"></param>
    /// <returns></returns>
    public int RemoveItem(Item item, int removeCount) {
        size -= removeCount;
        if(size < 0) {
            size = 0;
        }
        if (item.isStackable) {
            foreach (InventorySlot inv in inventory) {
                if (inv.item.ID == item.ID) { //once we find the item, remove the required amount
                    inv.count -= removeCount;
                    if (inv.count <= 0) { //check to see if the item should still exist in the inventory
                        inventory.Remove(inv);
                        return 0;
                    }
                    return inv.count;
                }
            }
        } else {
            int c = 0;
            foreach (InventorySlot inv in new List<InventorySlot>(inventory)) {
                if (inv.item.ID == item.ID) { //once we find the item, remove the required amount
                    inventory.Remove(inv);
                    c += 1;
                    if(c >= removeCount) {
                        return removeCount;
                    }
                }
            }
            return c;
        }
        return -1;
    }

    /// <summary>
    /// removes all counts of an item from inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAllItem(Item item) {
        int i = 0;
        foreach(InventorySlot inv in inventory) {
            if(inv.item.ID == item.ID) {
                i = inv.count;
                size -= inv.count;
                inventory.Remove(inv);
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// delete every item in inventory
    /// </summary>
    public void ClearInventory() {
        inventory.Clear();
    }

    /// <summary>
    /// check to see if the passed item is in the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasItemInInventory(Item item) {
        foreach(InventorySlot inv in inventory) {
            if(inv.item.ID == item.ID) {
                return true;
            }
        }
        return false;
    }

    public void Transaction(PlayerInfo playerInfo, NPCInfo npcInfo, Inventory itemsToBuy, Inventory itemsToSell) {
        //remove each item from player and give to merchant and calculate money
        foreach (Inventory.InventorySlot inv in itemsToSell.inventory) {
            playerInfo.inventory.RemoveItem(inv.item, inv.count);
            npcInfo.merchantInventory.AddItemCount(inv.item, inv.count);
            playerInfo.money += inv.count * inv.item.baseItemCost;
            npcInfo.merchantMoney -= inv.count * inv.item.baseItemCost;
        }
        //this is the tricky part. need to void the transaction if the players inventory becomes full
        foreach(Inventory.InventorySlot inv in itemsToBuy.inventory) {
            if (!CanAddItemCount(inv.count)) {
                continue;
            }
            playerInfo.inventory.AddItemCount(inv.item, inv.count);
            npcInfo.merchantInventory.RemoveItem(inv.item, inv.count);
            playerInfo.money -= inv.count * inv.item.baseItemCost;
            npcInfo.merchantMoney += inv.count * inv.item.baseItemCost;

        }
    }

    /// <summary>
    /// check to see if the passed item is in the inventory with at least the passed ammount
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool HasItemCountInInventory(Item item, int count) {
        if (item.isStackable) {
            foreach (InventorySlot inv in inventory) {
                if (inv.item.ID == item.ID && inv.count >= count) {
                    return true;
                }
            }
            return false;
        } else {
            int c = 0;
            foreach (InventorySlot inv in inventory) {
                if (inv.item.ID == item.ID) {
                    c += 1;
                }
            }
            if(c >= count) {
                return true;
            }return false;
        }
    }

    public int Size {
        get {
            return size;
        }
    }

    public bool CanAddItem() {
        if(size >= maxSize) {
            return false;
        }
        return true;
    }

    public bool CanAddItemCount(int count) {
        if(size + count >= maxSize) {
            return false;
        }
        return true;
    }

}
