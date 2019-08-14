using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBetween : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("This starts inactive")]
    public GameObject inactive;

    [Tooltip("This starts active")]
    public GameObject active;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switchingBewteen(inactive,active);
    }

    /// <summary>
    /// Switches the states of two oject to be the opposite of the other
    /// </summary>
    /// <param name="inactive">Starts off</param>
    /// <param name="active">Starts On</param>
    void switchingBewteen(GameObject inactive,GameObject active) {
        if ((inactive.activeSelf == false) && (active.activeSelf == false)) {
            inactive.SetActive(true);
        }
        else if ((inactive.activeSelf == true) && (active.activeSelf == false)) {
            //Stay active
        }
        else if ((inactive.activeSelf == false) && (active.activeSelf == true)) {
            //Stay inactive
        }
        else {
            inactive.SetActive(false);
        }
    }
}
