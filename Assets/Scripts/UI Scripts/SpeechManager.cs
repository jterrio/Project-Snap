﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechManager : MonoBehaviour {

    public static SpeechManager ins;
    public float typingSpeed;

    public RectTransform speechOutline;
    public RectTransform speechPanel;
    public RectTransform talkerNamePanel;
    public RectTransform talkerTextPanel;
    public RectTransform merchantPanel;

    public RectTransform choicePanel;
    public RectTransform choiceGridLayoutPanel;
    public Button choicePrefab;

    private GameObject player;
    private GameObject npc;
    private TMPro.TextMeshProUGUI nameText;
    private TMPro.TextMeshProUGUI speechText;
    private string textToDisplay;
    private bool hasStartedTalking = false;
    private bool hasFinishedTalking = false;
    private bool inChoice = false;
    private bool inShop = false;
    private Coroutine typingCoroutine;

    private NPCSpeechHolder npcSpeech;
    private PlayerInfo pInfo;
    private NPCInfo npcInfo;

    private bool changedQuestDialoguePath = false;

    public bool InShop {
        get {
            return inShop;
        }
    }

    // Use this for initialization
    void Start () {
        //make singleton
        if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        //init
        nameText = talkerNamePanel.GetComponent<TMPro.TextMeshProUGUI>();
        speechText = talkerTextPanel.GetComponent<TMPro.TextMeshProUGUI>();
	}

    /// <summary>
    /// called when the player hits space (either to enter dialogue, skip text, or see next line)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
    public void Speak(GameObject player, GameObject npc) {
        this.player = player; //set player
        
        this.npc = npc; //set actor/npc
        if(npcSpeech == null || pInfo == null || npcInfo == null) { //get components if they are null (meaning this is the first line)
            npcSpeech = npc.GetComponent<NPCSpeechHolder>();
            pInfo = player.GetComponent<PlayerInfo>();
            npcInfo = npc.GetComponent<NPCInfo>();
        }

        //return if we are in the shop
        //useful for if we want to have the merchant talk after buying and selling
        if (inShop) {
            return;
        }
        //if not in shop, then we are either in a choice or we are in normal dialogue
        //either way, we need the speechOutline active
        speechOutline.gameObject.SetActive(true);

        //if we are in a choice, return so we don't mess anything up
        if (inChoice) {
            return;
        }
        //if in normal dialogue, that means we need the current line to push out
        NPCSpeechHolder.Dialogue dialogue = npcSpeech.Speak();

        //checks to see if the first line of dialogue has been spoken
        if (!hasStartedTalking) {
            hasStartedTalking = true;
            player.GetComponent<PlayerMovementScript>().CanPlayerMove = false; //disable player movement
            DoDialogue(dialogue); //prints dialogue
        } else if (!hasFinishedTalking) { //we havent finished dialogue, so finish it
            hasFinishedTalking = true;
            FinishSpeech(); //print full text
        } else{ //this means that the current dialogue is done and we can move onto the next line
            hasFinishedTalking = false;
            //checks to see if the current dialogue does not open a shop
            //if it does, this wont run and the shop will open from this line
            
            if (dialogue.checkQuestCondition) {
                changedQuestDialoguePath = true;
                if (CheckQuest(dialogue)) {
                    npcSpeech.SetDialogueSetChoice(dialogue.setToGoToForComplete);
                } else {
                    npcSpeech.SetDialogueSetChoice(dialogue.setToGoToForInProgress);
                }
            } else {
                changedQuestDialoguePath = false;
            }
            
            GiveQuest(dialogue);
            if (!CheckShop(dialogue)) {

                //check to see if we can continue talking (reached the last line) and move current dialogue to next one
                bool IsFinished = npcSpeech.UpdateCurrentLine();
                dialogue = npcSpeech.Speak(); //get next line
                if (!IsFinished || changedQuestDialoguePath) { //if not finished, print next line
                    DoDialogue(dialogue);
                } else { //if we are finished, end the dialogue
                    EndSpeech();
                }
            }
        }
        
    }

    /// <summary>
    /// if we can open the shop from the current dialogue, open it
    /// </summary>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    bool CheckShop(NPCSpeechHolder.Dialogue dialogue) {
        if (dialogue.openShop) {
            OpenShop(); //opens shop and sets values
            return true;
        }
        return false;
    }

    bool GiveQuest(NPCSpeechHolder.Dialogue dialogue) {
        if (dialogue.givesQuest && !QuestManagerScript.ins.DoesQuestExist(dialogue.questID)) {
            GameManagerScript.ins.playerQuests.AddQuest(QuestManagerScript.ins.CreateQuestFromID(dialogue.questID));
            return true;
        }
        return false;
    }

    bool CheckQuest(NPCSpeechHolder.Dialogue dialogue) {
        if (QuestManagerScript.ins.GetExistingQuestFromID(dialogue.questID).HasCompletedCurrentObjective()) {
            QuestManagerScript.ins.GetExistingQuestFromID(dialogue.questID).CompleteObjective();
            return true;
        }
        return false;
    }

    /// <summary>
    /// opens shop and sets values
    /// </summary>
    void OpenShop() {
        inShop = true; //set that we are in the sgop
        speechOutline.gameObject.SetActive(false); //disable speech panels
        choicePanel.gameObject.SetActive(false);
        speechPanel.gameObject.SetActive(false);
        MerchantManagerScript.ins.Shop(player, npc); //tell merchant script to open shop
    }

    /// <summary>
    /// close the shop panel 
    /// </summary>
    public void CloseShop() {
        inShop = false; //set that we have left the shop
        merchantPanel.gameObject.SetActive(false); //set the panel inactive if it wasnt already
        MerchantManagerScript.ins.CloseShop(); //tell the merhcant script to close shop
        EndSpeech(); //end speech
    }

    void DoDialogue(NPCSpeechHolder.Dialogue dialogue) {
        //check to see if the current dialogue will be normal dialogue or a choice
        if (dialogue.isChoice) { //if choice, then display the choices and set values
            inChoice = true;
            DisplayChoices(dialogue);
        } else { //if normal speech, set values and being displaying the text
            SetNameText(dialogue.speakerName);
            SetSpeechText(dialogue.text);
            EnterSpeech(nameText.text, speechText.text);
        }
    }

    /// <summary>
    /// displays the choices of the current dialogue
    /// </summary>
    /// <param name="dialogue"></param>
    void DisplayChoices(NPCSpeechHolder.Dialogue dialogue) {
        speechPanel.gameObject.SetActive(false); //set this inactive since we don't need it
        //destory old choices if there were any in the grid layout
        foreach(Transform child in choiceGridLayoutPanel.transform) {
            Destroy(child.gameObject);
        }

        //instantiate the new choices in the grid layout
        foreach (NPCSpeechHolder.Choice choice in dialogue.myChoices.choices) {
            Button choiceButton = (Button)Instantiate(choicePrefab, choiceGridLayoutPanel);
            choiceButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choice.choiceText;
            choiceButton.GetComponent<ChoiceScript>().choiceSet = choice.setToGoTo;
        }
        choicePanel.gameObject.SetActive(true); //set the choice panel and grid layout to be active
    }

    /// <summary>
    /// pick a choice; number represents set number to jump to for the dialogue
    /// </summary>
    /// <param name="number"></param>
    public void PickChoice(int number) {
        //check to see if the choice opens the shop, if does, open it
        if (npcSpeech.Speak().openShop) {
            OpenShop();
        }

        //set the dialgoeu set to number
        npcSpeech.SetDialogueSetChoice(number);
        inChoice = false; //set values
        hasFinishedTalking = false;
        hasStartedTalking = false;
        Speak(player, npc); //re-enter speech manually since the player clicked instead of hitting space
    }

    /// <summary>
    /// set the name text; dynamically sets player name and npc names
    /// </summary>
    /// <param name="text"></param>
    void SetNameText(string text) {
        if(text == "[PlayerName]") {
            nameText.text = pInfo.characterName;
        } else {
            nameText.text = npcInfo.characterName;
        }
    }

    /// <summary>
    /// set speech text to the full text
    /// </summary>
    /// <param name="text"></param>
    void SetSpeechText(string text) {
        speechText.text = text;
    }

    /// <summary>
    /// enter speech and begin printing the speech
    /// </summary>
    /// <param name="talkerName"></param>
    /// <param name="talkerText"></param>
    public void EnterSpeech(string talkerName, string talkerText) {
        //check to see if we skipped some dialogue and we need to stop the previous coroutine
        if(typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
        }
        speechText.text = ""; //reset the speech text
        nameText.text = talkerName; //display name
        textToDisplay = talkerText; //set text to display
        choicePanel.gameObject.SetActive(false); //disable the choice panel
        speechPanel.gameObject.SetActive(true); //enable the speech panel
        typingCoroutine = StartCoroutine(Type()); //begin the couroutine for displaying the letters
    }

    /// <summary>
    /// Reset values and exit the dialogue
    /// </summary>
    public void EndSpeech() {
        if(npcSpeech == null) {
            return;
        }

        npcSpeech.ResetSetNumber();
        npcSpeech.currentLine = 0;
        player.GetComponent<PlayerMovementScript>().CanPlayerMove = true;
        npcInfo.InitPosition();
        npcInfo.StopTalk();
        npcSpeech = null;
        pInfo = null;
        npcInfo = null;
        speechOutline.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);
        speechPanel.gameObject.SetActive(false);
        StopCoroutine(typingCoroutine);
        speechText.text = "";
        nameText.text = "";
        textToDisplay = "";
        inShop = false;
        inChoice = false;
        hasStartedTalking = false;
        hasFinishedTalking = false;
        
    }

    /// <summary>
    /// for ending an item pick-up dialogue/sequence
    /// </summary>
    public void EndSpeechItem() {
        speechOutline.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);
        speechPanel.gameObject.SetActive(false);
        StopCoroutine(typingCoroutine);
        speechText.text = "";
        nameText.text = "";
        textToDisplay = "";
        hasStartedTalking = false;
        hasFinishedTalking = false;

    }

    /// <summary>
    /// end the coroutine and display the full text (used for skipping dialogue)
    /// </summary>
    public void FinishSpeech() {
        StopCoroutine(typingCoroutine);
        speechText.text = textToDisplay;
    }

    public bool PickUpItem(Item item) {
        speechOutline.gameObject.SetActive(true);
        choicePanel.gameObject.SetActive(false);
        speechPanel.gameObject.SetActive(true);
        if (!hasStartedTalking) {
            hasStartedTalking = true;
            DoTextDialogue(item.itemName);
            return false;
        }else if (!hasFinishedTalking) {
            hasFinishedTalking = true;
            FinishSpeech();
            return false;
        } else {
            EndSpeechItem();
            return true;
        }
    }

    void DoTextDialogue(string itemName) {
        nameText.text = "";
        speechText.text = itemName + " has been picked up.";
        EnterSpeech(nameText.text, speechText.text);
    }

    /// <summary>
    /// is what the couroutine calls
    /// </summary>
    /// <returns></returns>
    IEnumerator Type() {
        //splits sentence/speech into charaters to be displayed at the given speed: typing speed
        foreach(char letter in textToDisplay.ToCharArray()) {
            speechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        hasFinishedTalking = true; //once the couroutine finished, set that we have finished
    }

}
