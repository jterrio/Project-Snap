using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatBribeScript : MonoBehaviour, IPointerClickHandler, IScrollHandler {

    public bool isSelected;
    public DisplayItemCount dic;
    public int maxItemCount;
    public int currentSelectedCount;

    /// <summary>
    /// Set the value for currently selected and max values
    /// </summary>
    /// <param name="current">Current selected amount</param>
    /// <param name="max">Total amount</param>
    public void SetValues(int current, int max) {
        currentSelectedCount = current;
        maxItemCount = max;
    }

    /// <summary>
    /// Triggers when clicked on the item
    /// </summary>
    /// <param name="eventData">Data for the mouseclick</param>
    public void OnPointerClick(PointerEventData eventData) {
        //set the selected value bool
        isSelected = !isSelected;
        //do something with the new selected value
        if (isSelected) {
            Highlight();
            CombatManager.ins.combatSpeech.AddItem(gameObject);
        } else {
            Delight();
            CombatManager.ins.combatSpeech.RemoveAllItems(gameObject);
        }
    }

    /// <summary>
    /// Highlights the clicked object
    /// </summary>
    void Highlight() {
        if (maxItemCount != 1) {
            dic.numberField.text = currentSelectedCount.ToString() + "/" + maxItemCount.ToString();
        }
        transform.GetComponent<Image>().color = new Color(0, 1, 0);
    }

    /// <summary>
    /// Un-highlights the clicked object and returns it to normal
    /// </summary>
    void Delight() {
        if (maxItemCount != 1) {
            dic.numberField.text = maxItemCount.ToString();
        }
        currentSelectedCount = 1;
        transform.GetComponent<Image>().color = new Color(1, 1, 1);
    }

    /// <summary>
    /// Triggers when the mousewheel is scrolled
    /// </summary>
    /// <param name="eventData">Contains info about the scroll; direction of scroll</param>
    public void OnScroll(PointerEventData eventData) {
        if (isSelected) {
            //update ammount we want
            if (eventData.scrollDelta.y > 0) { //increase
                if (currentSelectedCount < maxItemCount) {
                    currentSelectedCount += 1;
                    CombatManager.ins.combatSpeech.AddItem(gameObject);
                }
            } else { //decrease
                if (currentSelectedCount > 1) {
                    CombatManager.ins.combatSpeech.RemoveItem(gameObject);
                    currentSelectedCount -= 1;
                }
            }
            Highlight();
        }
    }

}
