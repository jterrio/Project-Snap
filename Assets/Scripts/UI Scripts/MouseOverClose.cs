using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOverClose : MonoBehaviour {

    private bool isActive = false;
    public List<GameObject> parents;


    // Update is called once per frame
    void Update(){
        if (isActive == true) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult r in results) {
                if (r.gameObject.layer == LayerMask.NameToLayer("UI") && r.gameObject.activeSelf) {
                    SetActive(false);
                }
            }
        }
    }

    public void SetActive(bool a) {
        isActive = a;
    }
}
