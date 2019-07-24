using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFade : MonoBehaviour {

    public AnimationClip anim;

	// Use this for initialization
	void Start () {
        print(anim.wrapMode);
	}
	
}
