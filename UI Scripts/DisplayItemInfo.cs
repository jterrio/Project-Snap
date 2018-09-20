using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DisplayItemInfo : MonoBehaviour {

    public Item item;
    public Image image;

    public TMPro.TextMeshProUGUI itemName;
    public TMPro.TextMeshProUGUI itemDesc;
    public TMPro.TextMeshProUGUI itemType;
    public TMPro.TextMeshProUGUI itemCost, itemExtra2, itemExtra3, itemExtra4, itemExtra5;

    public Vector3 defaultPositionTop;
    public Vector3 defaultPositionBottom;

    public Vector3 defaultInvertedPositionTop;
    public Vector3 defaultInvertedPositionBottom;

    public RectTransform foreground;

    //determines where the info panel should appear
    //newParent is legacy and does not matter
    //position is determined by the mouse position
    public void MakePanelChild(RectTransform newParent) {
        transform.position = newParent.position;
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if(mousePosition.x <= 0.5 && mousePosition.y >= 0.5) {
            //top left
            transform.position += defaultPositionTop;
        }else if(mousePosition.x <= 0.5 && mousePosition.y < 0.5) {
            //bottom left
            transform.position += defaultPositionBottom;
        } else if(mousePosition.x > 0.5 && mousePosition.y >= 0.5) {
            //top right
            transform.position += defaultInvertedPositionTop;
        } else {
            //bottom right
            transform.position += defaultInvertedPositionBottom;
        }
    }

    //reset the fields
    void ResetFields() {
        itemExtra2.text = "";
        itemExtra3.text = "";
        itemExtra4.text = "";
        itemExtra5.text = "";
    }

    //display the item info in the info panel
    public void DisplayInfo(Item newItem) {
        ResetFields(); //reset the field
        item = newItem; //set the current item 
        //set the item information
        image.sprite = item.sprite;
        itemName.text = item.itemName;
        itemDesc.text = item.itemDesc;
        itemType.text = "Type: " + item.itemType.ToString();
        if (item.isSellable) {
            itemCost.text = "Price: " + item.baseItemCost;
        } else {
            itemCost.text = "Not Sellable";
        }
        //add switch/case here
        //Set fields based on the items type
        switch (item.itemType) {
            case Item.BaseType.Clothing:
                Clothing clothing = (Clothing)item;
                itemExtra2.text = "Protection: " + clothing.protection;
                break;
            case Item.BaseType.Consumable:
                Consumable consumable = (Consumable)item;
                if (consumable.healthRestored != 0 && consumable.energyRestored != 0) {
                    itemExtra2.text = "Health Restored: " + consumable.healthRestored;
                    itemExtra3.text = "Energy Restored: " + consumable.energyRestored;
                } else if (consumable.healthRestored == 0 && consumable.energyRestored != 0) {
                    itemExtra2.text = "Energy Restored: " + consumable.energyRestored;
                    itemExtra3.text = "";
                } else if (consumable.healthRestored != 0 && consumable.energyRestored == 0) {
                    itemExtra2.text = "Health Restored: " + consumable.healthRestored;
                    itemExtra3.text = "";
                } else {
                    itemExtra2.text = "";
                    itemExtra3.text = "";
                }
                break;
            case Item.BaseType.Key:
                Key key = (Key)item;
                break;
            case Item.BaseType.Misc:
                Misc misc = (Misc)item;
                break;
            case Item.BaseType.Useable:
                Useable useable = (Useable)item;
                break;
            case Item.BaseType.Weapon:
                Weapon weapon = (Weapon)item;
                itemExtra2.text = "Damage: " + weapon.attackValue;
                itemExtra3.text = "Durability: " + weapon.durability;
                break;

        }
    }
}
