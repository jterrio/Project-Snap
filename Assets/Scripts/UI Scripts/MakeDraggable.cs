using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MakeDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static GameObject itemBeingDragged;
    private Vector3 startPosition;
    private Vector3 offset;

    /// <summary>
    /// Get offset and set the object that is being dragged
    /// </summary>
    /// <param name="eventData">Mouse data</param>
    public void OnBeginDrag(PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        offset = startPosition - Input.mousePosition;
    }

    /// <summary>
    /// Move the object to the mouse with the added offset
    /// </summary>
    /// <param name="eventData">Mouse data</param>
    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition + offset;
    }

    //end the drag
    //needs to add visibility check that moves to startPosition if invalid position
    public void OnEndDrag(PointerEventData eventData) {

        itemBeingDragged = null;
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    bool isVisible(PointerEventData eventData) {
        return false;
    }
}
