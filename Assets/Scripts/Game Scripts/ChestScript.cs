using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour {

    public SpriteRenderer sr;
    public Sprite[] stateSprite;
    public Inventory chestInventory;
    public bool isOpen = false;
    public uint id;
    public bool isLocked;
    public Item key;


	// Use this for initialization
	void Start () {
        sr.sprite = stateSprite[0];
	}


    public void OpenChest() {
        if (isLocked) {
            if (!UIManager.ins.player.GetComponent<PlayerInfo>().inventory.HasItemInInventory(key)) {
                return;
            } else {
                isLocked = false;
            }
        }
        ReloadItems();
        UIManager.ins.OpenChest(gameObject);
        UIManager.ins.UpdateChestPlayerItemCount(GameManagerScript.ins.playerInventory.Size, GameManagerScript.ins.playerInventory.maxSize);
        UIManager.ins.UpdateChestItemCount(chestInventory.Size, chestInventory.maxSize);
        sr.sprite = stateSprite[1]; //open chest sprite
        isOpen = true;
    }

    public void ReloadItems() {
        RemoveItems();
        LoadItems();
    }

    public void CloseChest() {
        RemoveItems();
        sr.sprite = stateSprite[0];
        isOpen = false;
    }

    public void RemoveItems() {
        RemovePlayerItems();
        RemoveChestItems();
    }

    public void LoadItems() {
        LoadPlayerItems();
        LoadChestItems();
    }

    void RemovePlayerItems() {
        foreach(Transform transform in UIManager.ins.ChestPlayerGridLayout.transform) {
            Destroy(transform.gameObject);
        }
    }

    void RemoveChestItems() {
        foreach (Transform transform in UIManager.ins.ChestChestGridLayout.transform) {
            Destroy(transform.gameObject);
        }
    }
    void LoadPlayerItems() {
        foreach (Inventory.InventorySlot inv in UIManager.ins.player.GetComponent<PlayerInfo>().inventory.inventory) {
            RectTransform newItem = (RectTransform)Instantiate(UIManager.ins.ChestItemPrefab, UIManager.ins.ChestPlayerGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
        }
    }

    void LoadChestItems() {
        foreach (Inventory.InventorySlot inv in chestInventory.inventory) {
            RectTransform newItem = (RectTransform)Instantiate(UIManager.ins.ChestItemPrefab, UIManager.ins.ChestChestGridLayout);
            newItem.transform.GetChild(0).GetComponent<DisplayItemInInventory>().InventoryDisplay(inv.item, newItem);
            newItem.transform.GetComponentInChildren<DisplayItemCount>().ItemNumber = inv.count;
        }
    } 
}
