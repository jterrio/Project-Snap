using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestItemScript : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData) {
        Item targetItem = GetComponentInChildren<DisplayItemInInventory>().item;
        ChestScript cs = UIManager.ins.targetChest.GetComponent<ChestScript>();
        PlayerInfo pi = UIManager.ins.player.GetComponent<PlayerInfo>();
        if (transform.parent.name.ToString() == UIManager.ins.ChestPlayerGridLayout.name.ToString()) {
            //is in player's inv and needs to go to chest
            if (!cs.chestInventory.CanAddItem()) {
                return;
            }
            pi.inventory.RemoveItem(targetItem, 1);
            cs.chestInventory.AddItem(targetItem);
        } else {
            //is in chest inv and needs to go to player's inv
            if (!pi.inventory.CanAddItem()) {
                return;
            }
            pi.inventory.AddItem(targetItem);
            cs.chestInventory.RemoveItem(targetItem, 1);
        }
        cs.ReloadItems();
        UIManager.ins.UpdateChestItemCount(cs.chestInventory.Size, cs.chestInventory.maxSize);
        UIManager.ins.UpdateChestPlayerItemCount(GameManagerScript.ins.playerInventory.Size, GameManagerScript.ins.playerInventory.maxSize);
    }
}
