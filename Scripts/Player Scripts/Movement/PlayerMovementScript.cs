using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {


    private bool canMove = true;
    public float speed;
    public float runSpeed;
    private Vector2 vel;
    private Rigidbody2D rb;
    private PlayerInfo pi;
    private bool[] direction = new bool[4];

    void Start() {
        //w, a, s, d; back, left, front, right
        direction[0] = false;
        direction[1] = false;
        direction[2] = false;
        direction[3] = false;
        rb = GetComponent<Rigidbody2D>();
        pi = GetComponent<PlayerInfo>();
    }

    void Update() {

        if (!canMove) {
            return; //disable during cutscenes or something else
        }

        //w, a, s, d; back, left, front, right
        //check if key gets pressed or if key gets unpressed
        if (Input.GetKey(KeyCode.D)) {
            direction[3] = true;
            direction[1] = false;
            vel = new Vector2(1 * speed, vel.y);
        }
        if (Input.GetKey(KeyCode.A)) {
            direction[1] = true;
            direction[3] = false;
            vel = new Vector2(-1 * speed, vel.y);
        }
        if (Input.GetKey(KeyCode.W)) {
            direction[0] = true;
            direction[2] = false;
            vel = new Vector2(vel.x, 1 * speed);
        }
        if (Input.GetKey(KeyCode.S)) {
            direction[2] = true;
            direction[0] = false;
            vel = new Vector2(vel.x, -1 * speed);
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            direction[3] = false;
            vel = new Vector2(0, vel.y);
        }
        if (Input.GetKeyUp(KeyCode.A)) {
            direction[1] = false;
            vel = new Vector2(0, vel.y);
        }
        if (Input.GetKeyUp(KeyCode.W)) {
            direction[0] = false;
            vel = new Vector2(vel.x, 0);
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            direction[2] = false;
            vel = new Vector2(vel.x, 0);
        }
        //set direction bools
        SetDirection();
        //move the character based on keys pressed
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);
        //set velocity to be zero
        vel = Vector2.zero;

        
    }


    void SetDirection() {
        //w, a, s, d; back, left, front, right
        if (direction[0] == true) {
            if(direction[1] == true) {
                pi.direction = CharacterInfo.Direction.BACKLEFT;
            }else if(direction[3] == true) {
                pi.direction = CharacterInfo.Direction.BACKRIGHT;
            } else {
                pi.direction = CharacterInfo.Direction.BACK;
            }
        }else if(direction[2] == true) {
            if (direction[1] == true) {
                pi.direction = CharacterInfo.Direction.FRONTLEFT;
            } else if (direction[3] == true) {
                pi.direction = CharacterInfo.Direction.FRONTRIGHT;
            } else {
                pi.direction = CharacterInfo.Direction.FRONT;
            }
        }else if(direction[1] == true) {
            pi.direction = CharacterInfo.Direction.LEFT;
        }else if(direction[3] == true) {
            pi.direction = CharacterInfo.Direction.RIGHT;
        }
    }


    public bool CanPlayerMove {
        get {
            return canMove;
        }
        set {
            canMove = value;
        }
    }

}
