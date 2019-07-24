using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerScript : MonoBehaviour {

    public SpriteRenderer sr;
    private int factor = 1000; //higher this number, less likely to have "buggy" rendering issue with overlapping

    // Update is called once per frame
    void Update() {
        sr.sortingOrder = Mathf.RoundToInt((gameObject.transform.position.y - 1.5f) * factor) * -1;
    }
}
