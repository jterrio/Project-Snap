using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInfo : CharacterInfo {

    public NPC npc;

    private NPCSpeechHolder speech;
    public bool isTalkable;
    public bool isMerchant;
    public bool isKeyNPC;
    public bool hasQuests;

    public List<Item> merchantInventory;
    public float merchantMoney;


    void Start() {
        isTalkable = npc.isTalkable;
        characterName = npc.characterName;
        characterAge = npc.characterAge;
        isKeyNPC = npc.isKeyNPC;
        isTalkable = npc.isTalkable;
        isMerchant = npc.isMerchant;
        hasQuests = npc.hasQuests;
        direction = CharacterInfo.Direction.LEFT;
        speech = GetComponent<NPCSpeechHolder>();
    }



}
