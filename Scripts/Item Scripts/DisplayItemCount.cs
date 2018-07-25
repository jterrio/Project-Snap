using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemCount : MonoBehaviour {

    private int itemNumber = 1;
    public TMPro.TextMeshProUGUI numberField;


    public int ItemNumber {
        get {
            return itemNumber;
        }
        set {
            itemNumber = value;
            if(itemNumber > 1) {
                numberField.text = itemNumber.ToString();
            } else {
                numberField.text = "";
            }
            
        }
    }
}
