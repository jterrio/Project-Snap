using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollowScript : MonoBehaviour {

    public GameObject player;
    public bool cameraLocked;
    public float cameraSpeed;
    private float defaultOrthoSize;
    private float maxOrthoSize;
    private float minOrthoSize;
    private float maxCameraLookDistance = 15f;

    void Start() {
        cameraLocked = true;
        defaultOrthoSize = Camera.main.orthographicSize;
        maxOrthoSize = defaultOrthoSize * 3;
        minOrthoSize = defaultOrthoSize / 3;
    }


    // Update is called once per frame
    void Update () {
        //follow the player
        if (Input.GetKeyDown(GameManagerScript.ins.UnlockCamera)) {
            cameraLocked = !cameraLocked;
            if (cameraLocked) {
                player.GetComponent<PlayerInfo>().canMove = true;
            } else {
                player.GetComponent<PlayerInfo>().canMove = false;
            }
        }
        CameraZoom();
        
        if (cameraLocked) {
            FollowPlayer();
        } else {
            CameraKeyMovement();
        }
    }

    void FollowPlayer() {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
    }


    void CameraZoom() {
        if (Input.GetKeyDown(GameManagerScript.ins.CenterCamera)) {
            Camera.main.orthographicSize = defaultOrthoSize;
            cameraLocked = true;
        }
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (RaycastResult r in results) {
            if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                return;
            }
        }
        if (SpeechManager.ins.InShop) {
            return;
        }

        var i = Input.GetAxis("Mouse ScrollWheel");
        if (i > 0) { //scroll up - zoom in
            if (Camera.main.orthographicSize > minOrthoSize) {
                Camera.main.orthographicSize /= 1.25f;
            }
        } else if (i < 0) { //scroll down - zoom out
            if (Camera.main.orthographicSize < maxOrthoSize) {
                Camera.main.orthographicSize *= 1.25f;
            }
        }
    }


    void CameraKeyMovement() {

        Vector2 vel = Vector2.zero;
        if (Input.GetKey(GameManagerScript.ins.MoveRight)) {
            vel = new Vector2(1, vel.y);
        }
        if (Input.GetKey(GameManagerScript.ins.MoveLeft)) {
            vel = new Vector2(-1, vel.y);
        }
        if (Input.GetKey(GameManagerScript.ins.MoveUp)) {
            vel = new Vector2(vel.x, 1);
        }
        if (Input.GetKey(GameManagerScript.ins.MoveDown)) {
            vel = new Vector2(vel.x, -1);
        }
        if (Input.GetKeyUp(GameManagerScript.ins.MoveRight)) {
            vel = new Vector2(0, vel.y);
        }
        if (Input.GetKeyUp(GameManagerScript.ins.MoveLeft)) {
            vel = new Vector2(0, vel.y);
        }
        if (Input.GetKeyUp(GameManagerScript.ins.MoveUp)) {
            vel = new Vector2(vel.x, 0);
        }
        if (Input.GetKeyUp(GameManagerScript.ins.MoveDown)) {
            vel = new Vector2(vel.x, 0);
        }
        Vector3 pos = new Vector3(vel.x, vel.y, 0);
        if(Vector3.Distance(transform.position + (pos.normalized * cameraSpeed), player.transform.position) >= maxCameraLookDistance) {
            return;
        }
        transform.position += (pos.normalized * cameraSpeed);
    }


}
