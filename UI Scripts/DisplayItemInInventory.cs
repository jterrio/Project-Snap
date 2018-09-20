using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItemInInventory : MonoBehaviour {

    public Item item;
    public Image image;
    public RectTransform parent;

    public Text itemName;
    public Text itemDesc;
    public Text itemType;

    public Vector3 defaultPosition;

    //displays info from item and adds it into the inventory
    public void InventoryDisplay(Item newItem, RectTransform parent) {
        item = newItem;
        image.sprite = item.sprite;
        this.parent = parent;
    }


}
