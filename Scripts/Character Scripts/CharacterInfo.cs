using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour {

    public string characterName;
    public int characterAge;
    public Direction direction;
    public MovementState state;
    public Sprite[] directionSprites;
    protected SpriteRenderer sr;
    public bool canMove;

    //Direction that we are facing
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

    //State that we are moving in, if at all
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
            case Direction.RIGHT:
                sr.sprite = directionSprites[1];
                break;
            case Direction.BACK:
                sr.sprite = directionSprites[2];
                break;
            case Direction.LEFT:
                sr.sprite = directionSprites[3];
                break;
        }
    }

    //returns a bool depending on if we are facing another character
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

    //returns direction to turn to face in order to face another character
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



}
