using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : CharacterInfo {

    public List<Item> inventory;
    public float money;

    void Start() {
        direction = Direction.FRONT;
    }

}
