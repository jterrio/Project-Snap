using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuySellScript : MonoBehaviour, IPointerClickHandler, IScrollHandler {

    public bool isSelected;
    public DisplayItemCount dic;
    public int maxItemCount;
    public int currentSelectedCount;

    public void SetValues(int current, int max) {
        currentSelectedCount = current;
        maxItemCount = max;
    }

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
        if (maxItemCount != 1) {
            dic.numberField.text = currentSelectedCount.ToString() + "/" + maxItemCount.ToString();
        }
        transform.GetComponent<Image>().color = new Color(0, 1, 0);
    }

    //changes color back to normal
    void Delight() {
        if (maxItemCount != 1) {
            dic.numberField.text = maxItemCount.ToString();
        }
        currentSelectedCount = 1;
        transform.GetComponent<Image>().color = new Color(1, 1, 1);
    }

    public void OnScroll(PointerEventData eventData) {
        if (isSelected) {
            //update ammount we want
            if(eventData.scrollDelta.y > 0) { //increase
                if(currentSelectedCount < maxItemCount) {
                    currentSelectedCount += 1;
                    MerchantManagerScript.ins.UpdateCost(isSelected, gameObject);
                }
            } else { //decrease
                if(currentSelectedCount > 1) {
                    MerchantManagerScript.ins.RemoveItem(gameObject);
                    currentSelectedCount -= 1;
                }
            }
            Highlight();
        }
    }
}
