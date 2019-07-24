using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UseDoorScript : MonoBehaviour {

    public GameObject destination;
    public CharacterInfo.Direction directionToFace;
    public AudioClip audioToPlay;

    public bool goesToNewScene;
    public Scene destinationScene;


    public void PlaySound() {
        AudioManagerScript.ins.audioSource.clip = audioToPlay;
        AudioManagerScript.ins.audioSource.Play();
    }

}
