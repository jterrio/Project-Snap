using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCSpeechHolder : MonoBehaviour {

    private NPCInfo info;
    private GameObject player;
    private bool isPlayerInRange;
    public SpriteRenderer spriteRenderer;
    private Color defaultColor;
    public DialogueSet[] speechSets;
    public int currentLine = 0;
    public int currentSet = 0;
    
    /*GENERAL CURRENT SET READER
     * 0 = default dialogue
     * 1 = Aceept dialogue (merchants); 
     * 2 = Decline dialogue (merchants);
     * 
     * 
     * */

    [System.Serializable]
    public class DialogueSet {
        public Dialogue[] dialogue;
    }

    [System.Serializable]
    public class Dialogue {
        public string speakerName;
        public string text;

        public bool isChoice;
        public Choices myChoices;
        public bool openShop; //open shop after choice/space

   
        public int resetSet; //set to reset to beginning
        public bool reset; //if it should reset
    }

    [System.Serializable]
    public class Choices {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice {
        public string choiceText;
        public int setToGoTo;
    }

    void Start() {
        info = GetComponent<NPCInfo>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    void Update() {
       
    }

    /// <summary>
    /// give (speech manager) the current dialogue
    /// </summary>
    /// <returns></returns>
    public Dialogue Speak() {
        return speechSets[currentSet].dialogue[currentLine];
    }

    /// <summary>
    /// move to the next dialogue set/line
    /// </summary>
    /// <returns></returns>
    public bool UpdateCurrentLine() {
        //if we are at the end of the current set's line, do something
        if(currentLine == speechSets[currentSet].dialogue.Length - 1) {
            //if reset is true, set current set to 0
            //set current line to 0 to get to beginning
            if (speechSets[currentSet].dialogue[currentLine].reset) {
                ResetSetNumber();
            }
            //currentLine = 0;
            return true;
        } else {
            //move to next line
            currentLine += 1;
            return false;
        }
    }

    /// <summary>
    /// if we reset, the set should have a set number to reset to, which we set the current set to
    /// </summary>
    public void ResetSetNumber() {
        SetDialogueSet(speechSets[currentSet].dialogue[currentLine].resetSet);

    }



    public bool IsPlayerInRange {
        get {
            return isPlayerInRange;
        }
        set {
            isPlayerInRange = value;
        }
    }

    public GameObject Player {
        get {
            return player;
        }
        set {
            player = value;
        }
    }

    /// <summary>
    /// set dialogue set number to the given number and move the current line to beginning (0)
    /// </summary>
    /// <param name="setNumber"></param>
    public void SetDialogueSet(int setNumber) {
        currentSet = setNumber;
        //currentLine = 0;
    }

}
