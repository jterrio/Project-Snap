using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectAttackScript : MonoBehaviour, IPointerClickHandler {

    private bool isSelected;
    private Spell selectedSpell;

    public void OnPointerClick(PointerEventData eventData) {
        selectedSpell = GetComponentInChildren<DisplaySpellInfo>().spell;
        IsSelected = !IsSelected;
    }

    public bool IsSelected {
        get {
            return isSelected;
        }
        set {
            isSelected = value;
            if (value) { //true
                transform.GetComponent<Image>().color = new Color(0, 1, 0);
                CombatManager.ins.combatHUDAttack.SelectSpell(selectedSpell, this.gameObject);
            } else { //false
                selectedSpell = null;
                CombatManager.ins.combatHUDAttack.UnSelectSpell(selectedSpell, this.gameObject);
                transform.GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
    }
}
