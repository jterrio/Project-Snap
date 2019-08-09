using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour {

    [Header("Character Info")]
    public string characterName;
    public int characterAge;

    [Header("Directional Info")]
    public Direction direction;
    public MovementState state;
    public Sprite[] directionSprites;
    protected SpriteRenderer sr;

    [Header("Scripts")]
    public SpellInventory spellInventory;
    public Stats stats;
    public PolyNav.PolyNavAgent polyNav;

    [Header("Combat")]
    public bool canMove;
    private bool isWalking = true;
    public bool inCombat = false;
    public List<CombatHUDAttack.Attack> spellQueue = new List<CombatHUDAttack.Attack>();
    public Coroutine spellCastCoroutine;
    private int progress = 0;
    private float maxRangeForShootPrediction = 90;

    [Header("Health & Stamina")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    [Header("Running & Walking")]
    public float defaultSpeed;
    public float defaultRunSpeed;
    public float speed;
    public float runSpeed;

    /// <summary>
    /// Direction that we are facing
    /// </summary>
    public enum Direction {
        FRONT,
        FRONTRIGHT,
        RIGHT,
        BACKRIGHT,
        BACK,
        BACKLEFT,
        LEFT,
        FRONTLEFT
    }

    /// <summary>
    /// State that we are moving in, if at all
    /// </summary>
    public enum MovementState {
        STANDING,
        WALKING,
        RUNNING
    }

    /// <summary>
    /// Takes the direction and assigns the corresponding sprite
    /// </summary>
    public void SetSprite() {
        switch (direction) {
            case Direction.FRONT:
                sr.sprite = directionSprites[0];
                break;
            case Direction.FRONTRIGHT:
                sr.sprite = directionSprites[1];
                break;
            case Direction.RIGHT:
                sr.sprite = directionSprites[2];
                break;
            case Direction.BACKRIGHT:
                sr.sprite = directionSprites[3];
                break;
            case Direction.BACK:
                sr.sprite = directionSprites[4];
                break;
            case Direction.BACKLEFT:
                sr.sprite = directionSprites[5];
                break;
            case Direction.LEFT:
                sr.sprite = directionSprites[6];
                break;
            case Direction.FRONTLEFT:
                sr.sprite = directionSprites[7];
                break;
            default:
                sr.sprite = directionSprites[0];
                break;
        }
    }

    /// <summary>
    /// returns a bool depending on if we are facing another character
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool isFacing(Direction self, Direction other) {
        //switch statement to check to see if self is indeed opposite of other
        switch (self) {
            case Direction.FRONT:
                if(other == Direction.BACK) {
                    return true;
                }
                return false;
            case Direction.BACK:
                if (other == Direction.FRONT) {
                    return true;
                }
                return false;
            case Direction.LEFT:
                if (other == Direction.RIGHT) {
                    return true;
                }
                return false;
            case Direction.RIGHT:
                if (other == Direction.LEFT) {
                    return true;
                }
                return false;
            case Direction.FRONTLEFT:
                if (other == Direction.BACKRIGHT) {
                    return true;
                }
                return false;
            case Direction.BACKRIGHT:
                if (other == Direction.FRONTLEFT) {
                    return true;
                }
                return false;
            case Direction.FRONTRIGHT:
                if (other == Direction.BACKLEFT) {
                    return true;
                }
                return false;
            case Direction.BACKLEFT:
                if (other == Direction.FRONTRIGHT) {
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    /// <summary>
    /// returns direction to turn to face in order to face another character
    /// </summary>
    /// <param name="other"></param>
    /// <returns>return the inverse of the current direction</returns>
    public Direction TurnToFace(Direction other) {
        switch (other) {
            case Direction.FRONT:
                return Direction.BACK;
            case Direction.BACK:
                return Direction.FRONT;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
            case Direction.FRONTLEFT:
                return Direction.BACKRIGHT;
            case Direction.BACKRIGHT:
                return Direction.FRONTLEFT;
            case Direction.BACKLEFT:
                return Direction.FRONTRIGHT;
            case Direction.FRONTRIGHT:
                return Direction.BACKLEFT;
            default:
                return Direction.FRONT;
        }
    }

    /// <summary>
    /// returns direction to turn to face in order to face a given location
    /// </summary>
    /// <param name="targetLocation"></param>
    /// <returns>return direction that points to targetLocation</returns>
    public Direction FaceDirection(Vector3 targetLocation) {
        float currentAngle = Vector2.Angle(Vector2.down, targetLocation - gameObject.transform.position); //get current Angle we are running
        if (targetLocation.x < gameObject.transform.position.x) {
            currentAngle = 360 - currentAngle;
        }
        switch (Mathf.FloorToInt((currentAngle + 22.5f)  / 45)) {

            case 0:
                return Direction.FRONT;
            case 1:
                return Direction.FRONTRIGHT;
            case 2:
                return Direction.RIGHT;
            case 3:
                return Direction.BACKRIGHT;
            case 4:
                return Direction.BACK;
            case 5:
                return Direction.BACKLEFT;
            case 6:
                return Direction.LEFT;
            case 7:
                return Direction.FRONTLEFT;
            default:
                return Direction.FRONT;
        }
    }


    /// <summary>
    /// Starts the spell casting coroutine
    /// </summary>
    public void CastSpell() {
        spellCastCoroutine = StartCoroutine(SpellCoroutine());
    }

    /// <summary>
    /// Starts casting from the spellQueue if there are spells
    /// </summary>
    /// <returns></returns>
    IEnumerator SpellCoroutine() {
        //check to see if there are spells and this is the player
        if (gameObject == GameManagerScript.ins.player && spellQueue.Count > 0) {
            //grab info from the spell
            Image child = spellQueue[0].loggedInfo.GetComponentInChildren<Image>();
            Toggle toggle = spellQueue[0].loggedInfo.GetComponentInChildren<Toggle>();
            Slider slider = spellQueue[0].loggedInfo.GetComponentInChildren<Slider>();
            //init
            float angle = 0;
            for (progress = 0; progress < (spellQueue[0].selectedSpell.castTime * 100); progress++) {
                //if toggle (move while casting) is on, then move at the speed assigned. If not, then don't move.
                if (toggle.isOn) {
                    polyNav.maxSpeed = defaultSpeed;
                } else {
                    polyNav.maxSpeed = 0;
                }
                
                yield return new WaitForSeconds(0.01f);

                //show on ui
                currentStamina -= (spellQueue[0].selectedSpell.energyToCast / (spellQueue[0].selectedSpell.castTime * 100));
                child.fillAmount = progress / (spellQueue[0].selectedSpell.castTime * 100);
            }
            //Cast and Reset
            child.fillAmount = 1;
            polyNav.maxSpeed = defaultSpeed;

            //Check firemode and cast
            if (spellQueue[0].fireMode == CombatHUDAttack.FireMode.TARGET) {
                angle = ((slider.value - 0.5f) * maxRangeForShootPrediction * -1);
                SpellManagerScript.ins.CastSpell(spellQueue[0], gameObject, angle);
            } else {
                SpellManagerScript.ins.CastSpell(spellQueue[0], gameObject, 0);
            }

            //Remove the spell from UI
            CombatManager.ins.combatHUDAttack.RemoveAttackFromLayout(spellQueue[0]);
            //Remove spell from queue and reset the coroutine to know that it is finished
            spellQueue.RemoveAt(0);
            spellCastCoroutine = null;
        }
    }

    /// <summary>
    /// Cancels spell
    /// </summary>
    /// <param name="a"></param>
    public void CancelSpell(CombatHUDAttack.Attack a) {

        //Check if spell trying to cancel is being casted
        if(spellCastCoroutine != null && a.isCasting) {
            StopCoroutine(spellCastCoroutine);
            spellCastCoroutine = null;
        }
        //remove from UI
        if(gameObject == GameManagerScript.ins.player) {
            CombatManager.ins.combatHUDAttack.RemoveAttackFromLayout(a);
        }
        //remove from spellQueue
        if (spellQueue.Count > 0) {
            spellQueue.Remove(a);
        }
    }

    public void RecoverStamina() {
        currentStamina += ( maxStamina * 0.02f);
        if(currentStamina > maxStamina) {
            currentStamina = maxStamina;
        }
    }

    public void DrainStamina() {
        currentStamina -= (maxStamina * 0.02f);
        if(currentStamina < 0) {
            currentStamina = 0;
        }
    }

    public void DrainStamina(float a) {
        currentStamina -= a;
        if(currentStamina < 0) {
            currentStamina = 0;
        }
    }

    public void RecoverStamina(float a) {
        currentStamina += a;
        if (currentStamina > maxStamina) {
            currentStamina = maxStamina;
        }

    }

    public void LoseHealth(float a) {
        currentHealth -= a;
        if(currentHealth < 0) {
            currentHealth = 0;
        }

    }

    public void DrainStaminaTimes(int a) {
        for(int i = 0; i < a; i++) {
            DrainStamina();
        }
    }

    public void RecoverStaminaTimes(int a) {
        for (int i = 0; i < a; i++) {
            RecoverStamina();
        }
    }

    public void TryInsta(float chance) {
        if(currentHealth < maxHealth * 0.5f) {
            float a = chance - GetComponent<Stats>().spirituality * 3.5f;
            if(a > 0) {
                float b = Random.Range(1, 100);
                if(b <= a) { //true, insta killed
                    print("You got got!");
                    currentHealth = 0;
                }
            }
        }
    }


    public void GainHealth(float a) {
        currentHealth += a;
        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public void SetHealth(float a) {
        currentHealth = a;
        if(currentHealth < 0) {
            currentHealth = 0;
        }
        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
    }
  
    public void ResetStamina() {
        currentStamina = maxStamina;
    }

    public float GetStaminaPercentage() {
        return currentStamina / maxStamina;
    }

    public float GetHealthPercentage() {
        return currentHealth / maxHealth;
    }

    public bool IsWalking {
        get {
            return isWalking;
        }
        set {
            isWalking = value;
            if (isWalking) {
                polyNav.maxSpeed = speed;
            } else {
                polyNav.maxSpeed = runSpeed;
            }
        }

    }

    public bool DoHate(GameObject other) {
        if(gameObject == GameManagerScript.ins.player) {
            if(other.GetComponent<Stats>().attitude <= -30) {
                return true;
            }
        } else if (other == GameManagerScript.ins.player){
            if (GetComponent<Stats>().attitude <= -30) {
                return true;
            }
        } else {
            if(GetComponent<NPCInfo>().faction != other.GetComponent<NPCInfo>().faction) {
                return true;
            }
        }
        return false;
    }


    public bool DoLike(GameObject other) {
        return !DoHate(other);
    }

    /// <summary>
    /// Sets direction for when the character and used by player when in combat
    /// </summary>
    public void SetDirection() {
        //player
        if (gameObject == GameManagerScript.ins.player) {
            if (spellQueue.Count != 0) {
                if (spellQueue[0].fireMode == CombatHUDAttack.FireMode.TARGET) {
                    direction = FaceDirection(CombatManager.ins.combatHUDAttack.memory[spellQueue[0].attackTarget]);
                } else if (spellQueue[0].fireMode == CombatHUDAttack.FireMode.POINT) {
                    direction = FaceDirection(spellQueue[0].attackPointModePoint);
                } else {
                    direction = FaceDirection(gameObject.transform.position + spellQueue[0].attackDirection);
                }
                return;
            }
            if (CombatManager.ins.combatHUDLog.IsEmpty) {
                polyNav.Stop();
            }
        }

        //player movement and npc movement
        if (polyNav.movingDirection == Vector2.zero) {
            return;
        }
        int x = 0;
        int y = 0;
        if (polyNav.movingDirection.x > 0.225f) {
            x = 1;
        } else if (polyNav.movingDirection.x < -0.225f) {
            x = -1;
        }
        if (polyNav.movingDirection.y > 0.225f) {
            y = 1;
        } else if (polyNav.movingDirection.y < -0.225f) {
            y = -1;
        }
        switch (x) {
            case -1:
                switch (y) {
                    case -1:
                        direction = Direction.FRONTLEFT;
                        break;
                    case 1:
                        direction = Direction.BACKLEFT;
                        break;
                    default:
                        direction = Direction.LEFT;
                        break;
                }
                break;
            case 1:
                switch (y) {
                    case -1:
                        direction = Direction.FRONTRIGHT;
                        break;
                    case 1:
                        direction = Direction.BACKRIGHT;
                        break;
                    default:
                        direction = Direction.RIGHT;
                        break;
                }
                break;
            case 0:
                switch (y) {
                    case -1:
                        direction = Direction.FRONT;
                        break;
                    case 1:
                        direction = Direction.BACK;
                        break;
                    default:
                        break;
                }
                break;
        }


    }



    /// <summary>
    /// Get the position of the feet
    /// </summary>
    /// <returns>Feet, the location of the PolyNav AI</returns>
    public Vector3 Feet() {
        return new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z);
    }

    /// <summary>
    /// Checks whether we can see the targetObject
    /// </summary>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    public bool IsVisible(GameObject targetObject) {
        bool isVisible = false;
        if (targetObject != null) {
            int selfOldLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //change to ignore raycast
            int targetOldLayer = targetObject.layer;
            targetObject.layer = LayerMask.NameToLayer("SightTest"); //change to what we test

            Vector3 selfTopLeft = new Vector3(transform.position.x - 0.135f, transform.position.y + 0.2f, transform.position.z);
            Vector3 selfTopRight = new Vector3(transform.position.x + 0.135f, transform.position.y + 0.2f, transform.position.z);
            Vector3 selfBottomLeft = new Vector3(transform.position.x - 0.135f, transform.position.y - 0.45f, transform.position.z);
            Vector3 selfBottomRight = new Vector3(transform.position.x + 0.135f, transform.position.y - 0.45f, transform.position.z);

            Vector3 targetTopLeft = new Vector3(targetObject.transform.position.x - 0.135f, targetObject.transform.position.y + 0.2f, targetObject.transform.position.z);
            Vector3 targetTopRight = new Vector3(targetObject.transform.position.x + 0.135f, targetObject.transform.position.y + 0.2f, targetObject.transform.position.z);
            Vector3 targetBottomLeft = new Vector3(targetObject.transform.position.x - 0.135f, targetObject.transform.position.y - 0.45f, targetObject.transform.position.z);
            Vector3 targetBottomRight = new Vector3(targetObject.transform.position.x + 0.135f, targetObject.transform.position.y - 0.45f, targetObject.transform.position.z);
            List<Vector3> targetPositions = new List<Vector3>();
            targetPositions.Add(targetTopLeft);
            targetPositions.Add(targetTopRight);
            targetPositions.Add(targetBottomLeft);
            targetPositions.Add(targetBottomRight);

            bool topLeftCanSee = false;
            bool topRightCanSee = false;
            bool bottomLeftCanSee = false;
            bool bottomRightCanSee = false;

            //SELF TOPLEFT
            foreach(Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfTopLeft, v - selfTopLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if(hit.collider != null) {
                    if((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        topLeftCanSee = true;
                    }
                }
            }
            //SELF TOPRIGHT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfTopRight, v - selfTopRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        topRightCanSee = true;
                    }
                }
            }
            //SELF BOTTOMLEFT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfBottomLeft, v - selfBottomLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        bottomLeftCanSee = true;
                    }
                }
            }
            //SELF BOTTOMRIGHT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfBottomRight, v - selfBottomRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        bottomRightCanSee = true;
                    }
                }
            }

            if((topLeftCanSee && topRightCanSee) || (topLeftCanSee && bottomLeftCanSee) || (topRightCanSee && bottomRightCanSee) || (bottomLeftCanSee && bottomRightCanSee)) {
                isVisible = true;
            }

            gameObject.layer = selfOldLayer; //Set layer back to normal
            targetObject.layer = targetOldLayer; //Set layer back to normal

        }
        return isVisible;
    }

    /// <summary>
    /// checks whether the location of the attack can see them
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackPosition"></param>
    /// <returns></returns>
    public bool IsVisible(GameObject targetObject, Vector3 attackPosition) {
        bool isVisible = false;
        if (targetObject != null) {
            int selfOldLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //change to ignore raycast
            int targetOldLayer = targetObject.layer;
            targetObject.layer = LayerMask.NameToLayer("SightTest"); //change to what we test

            Vector3 selfTopLeft = new Vector3(attackPosition.x - 0.135f, attackPosition.y + 0.65f, attackPosition.z);
            Vector3 selfTopRight = new Vector3(attackPosition.x + 0.135f, attackPosition.y + 0.65f, attackPosition.z);
            Vector3 selfBottomLeft = new Vector3(attackPosition.x - 0.135f, attackPosition.y, attackPosition.z);
            Vector3 selfBottomRight = new Vector3(attackPosition.x + 0.135f, attackPosition.y, attackPosition.z);

            Vector3 targetTopLeft = new Vector3(targetObject.transform.position.x - 0.135f, targetObject.transform.position.y + 0.2f, targetObject.transform.position.z);
            Vector3 targetTopRight = new Vector3(targetObject.transform.position.x + 0.135f, targetObject.transform.position.y + 0.2f, targetObject.transform.position.z);
            Vector3 targetBottomLeft = new Vector3(targetObject.transform.position.x - 0.135f, targetObject.transform.position.y - 0.45f, targetObject.transform.position.z);
            Vector3 targetBottomRight = new Vector3(targetObject.transform.position.x + 0.135f, targetObject.transform.position.y - 0.45f, targetObject.transform.position.z);
            List<Vector3> targetPositions = new List<Vector3>();
            targetPositions.Add(targetTopLeft);
            targetPositions.Add(targetTopRight);
            targetPositions.Add(targetBottomLeft);
            targetPositions.Add(targetBottomRight);

            bool topLeftCanSee = false;
            bool topRightCanSee = false;
            bool bottomLeftCanSee = false;
            bool bottomRightCanSee = false;

            //SELF TOPLEFT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfTopLeft, v - selfTopLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        topLeftCanSee = true;
                    }
                }
            }
            //SELF TOPRIGHT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfTopRight, v - selfTopRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        topRightCanSee = true;
                    }
                }
            }
            //SELF BOTTOMLEFT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfBottomLeft, v - selfBottomLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        bottomLeftCanSee = true;
                    }
                }
            }
            //SELF BOTTOMRIGHT
            foreach (Vector3 v in targetPositions) {
                RaycastHit2D hit = Physics2D.Raycast(selfBottomRight, v - selfBottomRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
                if (hit.collider != null) {
                    if ((hit.collider.gameObject == targetObject) && (hit.collider.isTrigger)) {
                        bottomRightCanSee = true;
                    }
                }
            }

            if ((topLeftCanSee && topRightCanSee) || (topLeftCanSee && bottomLeftCanSee) || (topRightCanSee && bottomRightCanSee) || (bottomLeftCanSee && bottomRightCanSee)) {
                isVisible = true;
            }

            gameObject.layer = selfOldLayer; //Set layer back to normal
            targetObject.layer = targetOldLayer; //Set layer back to normal

        }
        return isVisible;
    }

    public bool IsVisible(Vector3 targetPosition, Vector3 attackPosition) {
        bool isVisible = false;

        Vector3 selfTopLeft = new Vector3(attackPosition.x - 0.135f, attackPosition.y + 0.65f, attackPosition.z);
        Vector3 selfTopRight = new Vector3(attackPosition.x + 0.135f, attackPosition.y + 0.65f, attackPosition.z);
        Vector3 selfBottomLeft = new Vector3(attackPosition.x - 0.135f, attackPosition.y, attackPosition.z);
        Vector3 selfBottomRight = new Vector3(attackPosition.x + 0.135f, attackPosition.y, attackPosition.z);

        Vector3 targetTopLeft = new Vector3(targetPosition.x - 0.135f, targetPosition.y + 0.2f, targetPosition.z);
        Vector3 targetTopRight = new Vector3(targetPosition.x + 0.135f, targetPosition.y + 0.2f, targetPosition.z);
        Vector3 targetBottomLeft = new Vector3(targetPosition.x - 0.135f, targetPosition.y - 0.2f, targetPosition.z);
        Vector3 targetBottomRight = new Vector3(targetPosition.x + 0.135f, targetPosition.y - 0.2f, targetPosition.z);
        List<Vector3> targetPositions = new List<Vector3>();
        targetPositions.Add(targetTopLeft);
        targetPositions.Add(targetTopRight);
        targetPositions.Add(targetBottomLeft);
        targetPositions.Add(targetBottomRight);

        bool topLeftCanSee = false;
        bool topRightCanSee = false;
        bool bottomLeftCanSee = false;
        bool bottomRightCanSee = false;

        //SELF TOPLEFT
        foreach (Vector3 v in targetPositions) {
            RaycastHit2D hit = Physics2D.Raycast(selfTopLeft, v - selfTopLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
            if (hit.collider != null) {
                topLeftCanSee = true;
            }
        }
        //SELF TOPRIGHT
        foreach (Vector3 v in targetPositions) {
            RaycastHit2D hit = Physics2D.Raycast(selfTopRight, v - selfTopRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
            if (hit.collider != null) {
                topRightCanSee = true;
            }
        }
        //SELF BOTTOMLEFT
        foreach (Vector3 v in targetPositions) {
            RaycastHit2D hit = Physics2D.Raycast(selfBottomLeft, v - selfBottomLeft, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
            if (hit.collider != null) {
                bottomLeftCanSee = true;
            }
        }
        //SELF BOTTOMRIGHT
        foreach (Vector3 v in targetPositions) {
            RaycastHit2D hit = Physics2D.Raycast(selfBottomRight, v - selfBottomRight, Mathf.Infinity, CombatManager.ins.characterVisibleTest); //raycast
            if (hit.collider != null) {
                bottomRightCanSee = true;
            }
        }

        if ((topLeftCanSee && topRightCanSee) || (topLeftCanSee && bottomLeftCanSee) || (topRightCanSee && bottomRightCanSee) || (bottomLeftCanSee && bottomRightCanSee)) {
            isVisible = true;
        }

        return isVisible;
    }


    public bool CanMoveTo(Vector3 p) {

        RaycastHit2D hit = Physics2D.Raycast(p, Vector2.zero, Mathf.Infinity, CombatManager.ins.mapTest);
        if (hit.collider != null) {
            return true;
        }
        return false;
    }

}
