using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : CharacterInfo {

    public Inventory inventory;
    public float money;
    public GameObject detector;


    void Update() {
        if (inCombat) {
            UIManager.ins.HealthImageFilled.fillAmount = currentHealth / maxHealth;
            UIManager.ins.StaminaImageFilled.fillAmount = currentStamina / maxStamina;
        }
    }

    void Start() {
        direction = Direction.FRONT;
        state = MovementState.STANDING;
        sr = GetComponent<SpriteRenderer>();
        canMove = true;
        speed = defaultSpeed;
        runSpeed = defaultRunSpeed;
    }

    public void EnterCombat() {
        canMove = false;
        inCombat = true;
        polyNav.enabled = true;
        UIManager.ins.EnableCombatHUD();
    }

    public void LeaveCombat() {
        canMove = true;
        inCombat = false;
        polyNav.enabled = false;
        UIManager.ins.DisableCombatHUD();
    }

}
