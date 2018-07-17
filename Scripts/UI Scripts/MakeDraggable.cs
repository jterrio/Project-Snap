using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MakeDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static GameObject itemBeingDragged;
    private Vector3 startPosition;
    private Vector3 offset;

    //get offset and set the object that is being dragged
    public void OnBeginDrag(PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        offset = startPosition - Input.mousePosition;
    }

    //move the object to the mouse with the added offset
    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition + offset;
    }

    //end the drag
    //needs to add visibility check that moves to startPosition if invalid position
    public void OnEndDrag(PointerEventData eventData) {

        itemBeingDragged = null;
        
    }


    bool isVisible(PointerEventData eventData) {
        return false;
    }
}
