using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class ItemManagerScript : MonoBehaviour {

    public static ItemManagerScript ins;

    public ItemManagerData allItemData;
    public List<Item> allItems;
    public List<GameObject> allItemsInScene;
    public List<GameObject> allChestsInScene;



    [System.Serializable]
    public class ItemManagerData {
        public List<ChestData> chestData = new List<ChestData>();

        [XmlArray("Inventory")]
        [XmlArrayItem("Slot")]
        public List<ItemData> itemData = new List<ItemData>();
    }

    [System.Serializable]
    public class ItemData {
        public int id;
        public bool isActive;
    }

    [System.Serializable]
    public class ChestData {

        [XmlArray("Inventory")]
        [XmlArrayItem("Slot")]
        public List<InventorySlotData> chestInventory;
        public bool isLocked;
        public uint id;
    }

    [System.Serializable]
    public class InventorySlotData {
        public int id;
        public int count;
    }

    public ItemManagerData ExportItemManagerData() {
        CollectItemManagerData();
        return allItemData;
    }

    public void CollectItemManagerData() {
        foreach(GameObject item in allItemsInScene) {
            ItemData itemD = new ItemData();
            itemD.id = item.GetComponent<WorldItemScript>().item.ID;
            itemD.isActive = item.activeInHierarchy;
            foreach(ItemData iData in allItemData.itemData) {
                if(iData.id == itemD.id) {
                    allItemData.itemData.Remove(iData);
                    break;
                }
            }
            allItemData.itemData.Add(itemD);
        }
        foreach(GameObject chest in allChestsInScene) {
            ChestData cd = new ChestData();
            cd.chestInventory = new List<InventorySlotData>();
            foreach (Inventory.InventorySlot invSlot in chest.GetComponent<Inventory>().inventory) {
                InventorySlotData invSlotData = new InventorySlotData();
                invSlotData.id = invSlot.item.ID;
                invSlotData.count = invSlot.count;
                foreach (ChestData chestD in allItemData.chestData) {
                    if (chestD.id == chest.GetComponent<ChestScript>().id) {
                        allItemData.chestData.Remove(chestD);
                        break;
                    }
                }
                cd.chestInventory.Add(invSlotData);
            }
            cd.isLocked = chest.GetComponent<ChestScript>().isLocked;
            allItemData.chestData.Add(cd);
        }
    }

    public void SpreadItemManagerData(ItemManagerData imd) {
        allItemData = imd;
        foreach(GameObject i in allItemsInScene) {   
            foreach(ItemData id in allItemData.itemData) {
                if(i.GetComponent<WorldItemScript>().item.ID == id.id) {
                    i.SetActive(id.isActive);
                    break;
                }
            }
        }
        foreach(GameObject c in allChestsInScene) {
            foreach(ChestData cd in allItemData.chestData) {
                if(c.GetComponent<ChestScript>().id == cd.id) {
                    ChestScript cis = c.GetComponent<ChestScript>();
                    cis.chestInventory.ClearInventory();
                    foreach (InventorySlotData invSlotData in cd.chestInventory) {
                        cis.chestInventory.AddItemCount(ins.GetItemFromID(invSlotData.id), invSlotData.count);
                    }
                    cis.isLocked = cd.isLocked;
                    break;
                }
            }
        }
        
    }



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
            foreach(Transform child in transform) {
                child.SetParent(ins.transform);
            }
            ins.GetAllItemsInScene();
            ins.GetAllChestsInScene();
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
        GetAllItemsInScene();
        GetAllChestsInScene();
        allItems = new List<Item>(Resources.LoadAll<Item>("Items"));

    }

    public void GetAllItemsInScene() {
        allItemsInScene.Clear();
        foreach (Transform child in transform) {
            if (child.gameObject.tag == "Item") {
                allItemsInScene.Add(child.gameObject);
            }
        }
    }

    public void GetAllChestsInScene() {
        allChestsInScene.Clear();
        foreach(Transform child in transform) {
            if (child.gameObject.tag == "Chest") {
                allChestsInScene.Add(child.gameObject);
            }
        }
    }

}
