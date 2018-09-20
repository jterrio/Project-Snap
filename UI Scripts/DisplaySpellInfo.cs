using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpellInfo : MonoBehaviour {

    public Spell spell;
    public Image image;
    public RectTransform parent;

    public Vector3 defaultPosition;

    //displays info from item and adds it into the inventory
    public void Display(Spell newSpell, RectTransform parent) {
        spell = newSpell;
        image.sprite = spell.icon;
        this.parent = parent;
    }

}
