using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagerScript : MonoBehaviour {

    public static ItemManagerScript ins;

    public List<Item> allItems;



    public Item GetItemFromID(int ID) {
        foreach(Item i in allItems) {
            if(i.ID == ID) {
                return Instantiate(i);
            }
        }
        return Instantiate(allItems[0]);
    }


    void Start() {
        
        if(ins == null) {
            ins = this;
        }else if(ins != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
        allItems = new List<Item>(Resources.LoadAll<Item>("Items"));

    }
}
