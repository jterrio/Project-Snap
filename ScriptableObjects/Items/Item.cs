using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject {

    public string itemName;
    public string itemDesc;
    public int ID;

    public bool isSellable;
    public bool isStackable;
    public BaseType itemType;
    public float baseItemCost;

    public Sprite sprite;

    public enum BaseType {
        Weapon,
        Consumable,
        Useable,
        Clothing,
        Misc,
        Key
    }

}
