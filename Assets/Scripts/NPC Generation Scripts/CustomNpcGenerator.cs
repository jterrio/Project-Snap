using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNpcGenerator : MonoBehaviour
{
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        
    }

    System.Random rnd = new System.Random();
    /// <summary>
    /// returns a number bewtween 1 and 100
    /// </summary>
    public int RandomNum() {
        return rnd.Next(1, 100);
    }
}
