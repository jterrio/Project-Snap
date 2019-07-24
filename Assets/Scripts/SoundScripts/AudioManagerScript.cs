using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour {

    public static AudioManagerScript ins;
    public AudioSource audioSource;

    void Start() {
        //singleton
        if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
    }
}
