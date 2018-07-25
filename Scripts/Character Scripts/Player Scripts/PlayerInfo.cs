using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : CharacterInfo {

    public Inventory inventory;
    public float money;

    void Start() {
        direction = Direction.FRONT;
        state = MovementState.STANDING;
        sr = GetComponent<SpriteRenderer>();
        canMove = true;
    }

}
