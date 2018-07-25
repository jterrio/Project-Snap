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

    //add item to the inventory
    public int AddItem(Item item) {
        if(size >= maxSize) {
            return -1; //throw error to the player
        } else {
            size += 1;
        }
        if (item.isStackable) { //if the item is stackable, see if the item exists in inventory so we can stack it
            foreach (InventorySlot inv in inventory) {
                if (inv.item == item) {
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
        int c = 0;
        for(int i = 1; i <= count; i++) {
            c = AddItem(item);
        }
        return c;
    }

    //remove an item to the inventory
    public int RemoveItem(Item item, int removeCount) {
        size -= 1;
        if(size < 0) {
            size = 0;
        }
        foreach(InventorySlot inv in inventory) {
            if(inv.item == item) { //once we find the item, remove the required amount
                inv.count -= removeCount;
                if(inv.count <= 0) { //check to see if the item should still exist in the inventory
                    inventory.Remove(inv);
                    return 0;
                }
                return inv.count;
            }
        }
        return -1;
    }

    //removes all counts of an item from inventory
    public int RemoveAllItem(Item item) {
        int i = 0;
        foreach(InventorySlot inv in inventory) {
            if(inv.item == item) {
                i = inv.count;
                size -= inv.count;
                inventory.Remove(inv);
                return i;
            }
        }
        return -1;
    }

    //delete every item in inventory
    public void ClearInventory() {
        inventory.Clear();
    }

    //check to see if the passed item is in the inventory
    public bool HasItemInInventory(Item item) {
        foreach(InventorySlot inv in inventory) {
            if(inv.item == item) {
                return true;
            }
        }
        return false;
    }

    //check to see if the passed item is in the inventory with at least the passed ammount
    public bool HasItemCountInInventory(Item item, int count) {
        foreach(InventorySlot inv in inventory) {
            if(inv.item == item && inv.count >= count) {
                return true;
            }
        }
        return false;
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

}
