using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldItemScript : MonoBehaviour {

    public Item item;
    public SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer.sprite = item.sprite;
	}
}
