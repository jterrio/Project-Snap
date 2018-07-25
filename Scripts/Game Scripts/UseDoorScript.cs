using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UseDoorScript : MonoBehaviour {

    public GameObject destination;
    public CharacterInfo.Direction directionToFace;
    public AudioClip audio;

    public bool goesToNewScene;
    public Scene destinationScene;


    public void PlaySound() {
        AudioManagerScript.ins.audioSource.clip = audio;
        AudioManagerScript.ins.audioSource.Play();
    }

}
