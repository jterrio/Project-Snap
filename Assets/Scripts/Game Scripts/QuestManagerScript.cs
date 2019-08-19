using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerScript : MonoBehaviour {

    public static QuestManagerScript ins;
    public List<Quest> allQuests;

	// Use this for initialization
	void Start () {
		//singelton
        if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
