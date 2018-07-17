using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuySellScript : MonoBehaviour, IPointerClickHandler {

    public bool isSelected;


    public void OnPointerClick(PointerEventData eventData) {
        //set the selected value bool
        isSelected = !isSelected;
        //do something with the new selected value
        if (isSelected) {
            Highlight();
        } else {
            Delight();
        }
        //tell merchant manager to update cost based on if we selected or un-selected
        MerchantManagerScript.ins.UpdateCost(isSelected, gameObject);
    }

    //changes color to reflect it being highligh
    void Highlight() {
        transform.GetComponent<Image>().color = new Color(0, 1, 0);
    }

    //changes color back to normal
    void Delight() {
        transform.GetComponent<Image>().color = new Color(1, 1, 1);
    }
}
