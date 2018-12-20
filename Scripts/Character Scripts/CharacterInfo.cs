using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour {

    public string characterName;
    public int characterAge;
    public Direction direction;
    public MovementState state;
    public SpellInventory spellInventory;
    public Sprite[] directionSprites;
    public Stats stats;
    protected SpriteRenderer sr;
    public bool canMove;
    public bool isWalking = true;
    public bool inCombat = false;
    public List<CombatHUDAttack.Attack> spellQueue = new List<CombatHUDAttack.Attack>();
    public Coroutine spellCastCoroutine;
    public PolyNav.PolyNavAgent polyNav;

    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    public float defaultSpeed;
    public float defaultRunSpeed;
    public float speed;
    public float runSpeed;

    private int progress = 0;


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

        }
        return false; //something has gone wrong if you reach this
    }

    /// <summary>
    /// returns direction to turn to face in order to face another character
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Direction TurnToFace(Direction other) {
        //return the inverse of the current direction
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
        }

        return Direction.FRONT; //something has gone wrong to reach this
    }


    public void CastSpell() {
        spellCastCoroutine = StartCoroutine(SpellCoroutine());
    }

    IEnumerator SpellCoroutine() {
        print("Beginning to cast spell...");
        if (gameObject == GameManagerScript.ins.player && spellQueue.Count > 0) {
            Image child = spellQueue[0].loggedInfo.GetComponentInChildren<Image>();
            Toggle toggle = spellQueue[0].loggedInfo.GetComponentInChildren<Toggle>();
            for (progress = 0; progress < (spellQueue[0].selectedSpell.castTime * 100); progress++) {
                if (toggle.isOn) {
                    polyNav.maxSpeed = defaultSpeed;
                } else {
                    polyNav.maxSpeed = 0;
                }
                yield return new WaitForSeconds(0.01f);
                currentStamina -= (spellQueue[0].selectedSpell.energyToCast / (spellQueue[0].selectedSpell.castTime * 100));
                child.fillAmount = progress / (spellQueue[0].selectedSpell.castTime * 100);
            }
            //exit the for loop at 99%, so set it to 1
            child.fillAmount = 1;
            polyNav.maxSpeed = defaultSpeed;
            print("Spell casted!");
            SpellManagerScript.ins.CastSpell(spellQueue[0], gameObject);

            //set check if the player here
            CombatManager.ins.combatHUDAttack.RemoveAttackFromLayout(spellQueue[0]);

            spellQueue.RemoveAt(0);
            spellCastCoroutine = null;
        } else {

        }

    }

    public void CancelSpell(CombatHUDAttack.Attack a) {
        print("Spell canceled!");
        if(spellCastCoroutine != null) {
            StopCoroutine(spellCastCoroutine);
            spellCastCoroutine = null;
        }
        if(gameObject == GameManagerScript.ins.player) {
            CombatManager.ins.combatHUDAttack.RemoveAttackFromLayout(a);
        }
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
        currentStamina -= (maxStamina * 0.01f);
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
        if (polyNav.movingDirection == Vector2.zero) {
            return;
        }
        int x = 0;
        int y = 0;
        if (polyNav.movingDirection.x > 0.25) {
            x = 1;
        } else if (polyNav.movingDirection.x < -0.25) {
            x = -1;
        }
        if (polyNav.movingDirection.y > 0.25) {
            y = 1;
        } else if (polyNav.movingDirection.y < -0.25) {
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
                    case 0:
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
                    case 0:
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
                    case 0:
                        break;
                }
                break;
        }


    }

}
