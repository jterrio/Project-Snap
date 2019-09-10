using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagerScript : MonoBehaviour {

    public static ItemManagerScript ins;

    public List<Item> allItems;



    public Item GetItemFromID(int ID) {
        for(int i = 0; i < allItems.Count; i++) {
            if(allItems[i].ID == ID) {
                return Instantiate(allItems[i]);
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

        allItems = new List<Item>(Resources.LoadAll<Item>("Items"));

    }
}
