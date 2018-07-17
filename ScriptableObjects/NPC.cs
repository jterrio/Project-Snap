using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject {

    public string characterName;
    public int characterAge;
    public bool isTalkable;
    public bool isKeyNPC;
    public bool isMerchant;
    public bool hasQuests;

    public Item[] itemPool;

}
