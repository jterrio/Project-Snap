using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class NpcGenerator : MonoBehaviour {
    //Misc
    public Text archeText;
    public Text Magic;
    public Text FavWeapon;
    //Stats
    public Text intuText;
    public Text intText;
    public Text strText;
    public Text charText;
    public Text preText;
    public Text dexText;
    public Text perText;
    public Text spirText;
    //Traits
    public Text physicalText;
    public Text personalityText;

    public string name = "Steve";
    public Text MyName;

    private string archetype = "";
    private int intuition = 0;
    private int intelligence = 0;
    private int strength = 0;
    private int charisma = 0;
    private int precision = 0;
    private int dexterity = 0;
    private int perception = 0;
    private int spirituality = 0; //Not used for Npcs
    private bool canUseMagic = false;
    private readonly string[] weaponType = { "Gun", "Sword", "GunSword", "GunMage", "MageSword", "Magic" };
    private string favWeapon = "Nothing";
    private string[] physicalTraits = null; //made later
    private string[] personalityTraits = null; //made later
    private int rando = 0;

    private void Awake(){
        NpcSaver.Init();
        name = "Steve";
        MyName.text = name;
    }

    // Start is called before the first frame update
    void Start() {
        name = "Steve";
        Run();
        MyName.text = name;
    }

    // Update is called once per frame
    void Update(){
        LoadData();
        updateName();
    }

    /// <summary>
    /// Exports to a Json file
    /// </summary>
    public void Save() {
        NpcSave npcSave = new NpcSave();
        npcSave.arch = archetype;
        npcSave.intu = intuition;
        npcSave.inte = intelligence;
        npcSave.str = strength;
        npcSave.cha = charisma;
        npcSave.pre = precision;
        npcSave.dex = dexterity;
        npcSave.per = perception;
        npcSave.spirit = spirituality;
        npcSave.UseMagic = canUseMagic;
        npcSave.favWeap = favWeapon;
        npcSave.physical = physicalTraits;
        npcSave.personality = personalityTraits;

        string json = JsonUtility.ToJson(npcSave);
        // To convert back to an object myObject = JsonUtility.FromJson<MyClass>(json);
        Debug.Log(json);
        NpcSaver.Save(json,MyName.text);
    }


    /// <summary>
    /// Updates the name for Your character
    /// </summary>
    public void updateName(){
        name = MyName.text;
    }

    /// <summary>
    /// Loads the data into the text objects in Unity inspector.
    /// </summary>
    public void LoadData(){
        //Misc Loading
        MyName.text = name;
        archeText.text = archetype;
        if (canUseMagic){
            Magic.text = name + " can use Magic.";
        }
        else{
            Magic.text = name + " cannot use Magic.";
        }
        FavWeapon.text = favWeapon + " is the favorite weapon of " + name;

        //Stats Loading
        intuText.text = "Intuition: " + intuition.ToString();
        intText.text = "Intelligence: " + intelligence.ToString();
        strText.text = "Strength: " + strength.ToString();
        charText.text = "Charisma: " + charisma.ToString();
        preText.text = "Precision: " + precision.ToString();
        dexText.text = "Dexterity: " + dexterity.ToString();
        perText.text = "Perception: " + perception.ToString();
        spirText.text = "Spirituality: " + spirituality.ToString();

        //Traits Loading
        string physicalTemp = "The Physical Traits of " + name + " are:";
        for(int i = 0; i < physicalTraits.Length; i++){
            if (physicalTraits[i] != "0" && physicalTraits[i] != "Average"){
                physicalTemp += (" " + physicalTraits[i] + ".");
            }
        }
        physicalText.text = physicalTemp;

        string personalityTemp = "The Personality Traits of " + name + " are:";
        for (int i = 0; i < personalityTraits.Length; i++)
        {
            if (personalityTraits[i] != "0" && personalityTraits[i] != "Average")
            {
                personalityTemp += (" " + personalityTraits[i] + ".");
            }
        }
        personalityText.text = personalityTemp;
    }


    private void Character(int startingStat, int startingPer) {
        //und - ability to pickup skills in combat
        intuition = startingStat;
        //int - ability to study skills outside of combat
        intelligence = startingStat;
        //str - ability to swing hard for physical attacks
        strength = startingStat;
        //chr - ability to get people to like you
        charisma = startingStat;
        //prc - ability to aim
        precision = startingStat;
        //spr - ability to survive more attacks
        spirituality = 0;
        //dex - ability to use more attacks or move
        dexterity = startingStat;
        //per - ability to process info in combat (default time per turn is 0.75seconds. every level reduces this by 0.025, with a hard cap at 20 points)
        perception = startingPer;
    }

    /// <summary>
    /// Updates Intuition
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateUnd(int updatedValue) {
        intuition += updatedValue;
    }

    /// <summary>
    /// Updates Intelligence
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateInt(int updatedValue) {
        intelligence += updatedValue;
    }

    /// <summary>
    /// Updates Strength
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateStr(int updatedValue) {
        strength += updatedValue;
    }

    /// <summary>
    /// Updates Charisma
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateChr(int updatedValue) {
        charisma += updatedValue;
    }

    /// <summary>
    /// Updates Precision
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updatePrc(int updatedValue) {
        precision += updatedValue;
    }

    /// <summary>
    /// Updates Spirituality
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateSpr(int updatedValue) {
        spirituality += updatedValue;
    }
    /// <summary>
    /// Updates Dexterity
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updateDex(int updatedValue) {
        dexterity += updatedValue;
    }
    /// <summary>
    /// Updates Perception
    /// </summary>
    /// <param name="updatedValue"></param>
    private void updatePer(int updatedValue) {
        perception += updatedValue;
    }

    /// <summary>
    /// Checks to see if the value is greater than 100 or less than 0 and corrects it
    /// </summary>
    /// <param name="valueToCheck"></param>
    /// <returns></returns>
    private int CheckMaxMin(int valueToCheck) {
        if (valueToCheck > 100) {
            return 100;
        }

        else if (valueToCheck < 0) {
            return 0;
        }

        else {
            return valueToCheck;
        }
    }

    /// <summary>
    /// Finalizing all the values so they are not over max or under min
    /// </summary>
    private void FinalizeValues() {
        intuition = CheckMaxMin(intuition);
        intelligence = CheckMaxMin(intelligence);
        strength = CheckMaxMin(strength);
        charisma = CheckMaxMin(charisma);
        precision = CheckMaxMin(precision);
        dexterity = CheckMaxMin(dexterity);
        if (perception > 20) {
            perception = 20;
        }
        else if (perception < 0) {
            perception = 0;
        }
    }

    private int top = 85;
    private int midBot = 16;

    System.Random rnd = new System.Random();

    /// <summary>
    /// returns a number bewtween 1 and 100
    /// </summary>
    public int RandomNum() {
        return rnd.Next(1, 100);
    }

    /// <summary>
    /// returns what type of npc they are going to be and the starting stats
    /// </summary>
    private void NpcType() {
        string[] student = { "Mage Student", "Study Magic Student", "Jock Sudent", "Intellectual Student", "Student" };
        int rando = RandomNum();
        //Goverment People
        if (rando >= 80) {
            Character(45, 9);
            archetype = "Government Person";
            rando = RandomNum();
            //Can they use magic
            if (rando > 15) {
                canUseMagic = true;
            }
            else {
                canUseMagic = false;
                // print("Goverment Person", end = '\n')
            }
        }

        //Students: Mages, Non Mage but study magic, don't study magic
        else if (rando < 80 && rando > 60) {
            rando = RandomNum();
            Character(40, 8);
            //Mages
            if (rando >= 60) {
                updateUnd(5);
                updateInt(-5);
                updateStr(5);
                updateDex(5);
                updatePrc(5);
                updatePer(2);
                archetype = student[0];
                canUseMagic = true;
            }
            //NonMage but sudy magic
            else if (rando < 60 && rando >= 35) {
                updateUnd(-5);
                updateInt(5);
                updatePrc(3);
                updatePer(2);
                archetype = student[1];
                canUseMagic = false;
            }
            //Don't study magic
            else {
                rando = RandomNum();
                //Can they use magic
                if (rando > 30) {
                    canUseMagic = true;
                }
                else {
                    canUseMagic = false;
                }
                rando = RandomNum();
                // Jock
                if (rando >= 60) {
                    updateStr(5);
                    updateDex(5);
                    updatePrc(5);
                    updatePer(1);
                    archetype = student[2];
                }

                // Intellectual Study
                else if (rando < 60 && rando >= 40) {
                    updateUnd(5);
                    updateInt(5);
                    updatePer(2);
                    archetype = student[3];
                }
                //# General Studies
                else {
                    updateUnd(2);
                    updateInt(2);
                    updateStr(2);
                    updateDex(2);
                    updatePrc(2);
                    archetype = student[4];
                }
            }
        }

        //# People on the streets
        else if (rando <= 60 && rando > 5) {
            rando = RandomNum();
            //Can they use magic
            if (rando > 30) {
                canUseMagic = true;
            }

            else {
                canUseMagic = false;
            }

            rando = RandomNum();
            //#Average Person
            if (rando >= 60) {
                string[] People = { "Laborer", "Soldier", "Thug", "BlackSmith", "Merchant" };
                Character(30, 7);
                rando = RandomNum();

                //Laborer
                if (rando > 80) {
                    archetype = People[0];
                }
                //Soldier
                else if (rando <= 80 && rando > 60) {
                    archetype = People[1];
                }
                //Thug
                else if (rando <= 60 && rando > 40) {
                    archetype = People[2];
                }
                //BlackSmith
                else if (rando <= 40 && rando > 20) {
                    archetype = People[3];
                }
                //Merchant
                else {
                    archetype = People[3];
                }
            }

            else if (rando < 60 && rando >= 40) {
                Character(50, 7);
                updateStr(-5);
                archetype = "Veteran";
            }

            else if (rando < 40 && rando >= 20) {
                Character(10, 4);
                archetype = "Child";
            }

            //Lower Class
            else {
                Character(20, 4);
                archetype = "Lower Class";
            }
        }
        //Hero Type
        else {
            Character(65, 13);
            archetype = "Hero";
            rando = RandomNum();
            if (rando > 50) {
                canUseMagic = true;
            }

            else {
                canUseMagic = false;
            }
        }
        // Determine their fighting style
        rando = RandomNum();
        if (canUseMagic) {
            //Gun Mage
            rando = RandomNum();
            if (rando >= 66) {
                updatePrc(5);
                updatePer(1);
                favWeapon = weaponType[3];
            }
            // Sword Mage
            else if (rando < 66 && rando > 33) {
                updateStr(5);
                updatePer(1);
                favWeapon = weaponType[4];
            }

            //Mage   
            else {
                updateStr(3);
                updateDex(3);
                updatePrc(3);
                updatePer(1);
                favWeapon = weaponType[5];
            }
        }
        else {
            //Gun
            if (rando >= 66) {
                updatePrc(5);
                favWeapon = weaponType[0];
            }
            // Sword
            else if (rando < 66 && rando > 33) {
                updateStr(5);
                favWeapon = weaponType[1];
            }
            //GunSword
            else {
                updatePrc(3);
                updateStr(3);
                favWeapon = weaponType[2];
            }
        }
    }

    /// <summary>
    /// creates an array of Personality Atributes to define a npc
    /// </summary>
    /// <returns></returns>
    private string[] Atributes() {
        int count = 0 ;
        string[] atributes = new string[20];//make it work
        foreach(string x in atributes){
            atributes[count] = "0";
            count++;
        }

        rando = RandomNum();
        //What their words mean
        if (rando >= top) {
            atributes[0] = "Genuine";
            updateChr(2);
        }

        else if (rando < top && rando > midBot) {
            atributes[0] = "Average";
        }
        else {
            atributes[0] = "Ingenuine";
            updatePer(1);
            updateChr(-4);
        }
        // Their Creativity
        rando = RandomNum();
        if (new string[] {"Government Person", "Mage Student", "Intellectual Student", "Study Magic Student"}.Contains(archetype)) {
            rando += 10;
        }

        if (rando >= top) {
            atributes[1] = "Creative";
            updateChr(2);
            updateInt(4);
            updateUnd(4);
        }

        else if (rando < top && rando > midBot) {
            atributes[1] = "Average";
            updateUnd(1);
            updateInt(1);
        }
        else {
            atributes[1] = "Uncreative";
            updateInt(-4);
            updateUnd(-4);
            updateChr(-2);
        }
        // Their Humor meter
        rando = RandomNum();
        if (rando >= top) {
            atributes[2] = "Jokester";
            updateChr(3);
            updatePrc(1);
            updateDex(2);
        }

        else if (rando < top && rando > midBot) {
            atributes[2] = "Average";
        }

        else {
            atributes[2] = "Serious";
            updatePrc(3);
            updateStr(1);
            updateChr(-3);
        }
        // The way they hold themselves
        rando = RandomNum();
        if (physicalTraits[1] == "Buff") {
            rando += 15;
        }


        if (rando >= top) {
            atributes[3] = "Arrogant";
            updateChr(-3);
            updatePrc(2);
        }

        else if (rando < top && rando > midBot) {
            atributes[3] = "Average";
            updateChr(1);
        }
        else {
            atributes[3] = "SelfDepricating";
            updateInt(2);
            updateChr(-1);
        }
        // How they think, body or mind, think how they react to things
        rando = RandomNum();
        if (new string[] { "Intellectual Student", "Hero", "Government Person", "Study Magic Student" }.Contains(archetype)){
            rando -= 10;
        }
        if (new string[] { "Jock Sudent", "Lower Class" }.Contains(archetype)) {
            rando += 10;
        }
        if (physicalTraits[1] == "Buff") {
            rando += 15;
        }

        if (rando >= top) {
            atributes[4] = "Meat-Head";
            updateUnd(10);
            updateInt(-5);
            updatePer(1);
            updateStr(2);
        }

        else if (rando < top && rando > midBot) {
            atributes[4] = "Average";
        }
        else {
            atributes[4] = "Savvy";
            updateInt(10);
            updateUnd(-5);
            updatePer(1);
            updatePrc(2);
        }
        // How they learn, via experience or books
        rando = RandomNum();
        if (new string[] { "Intellectual Student", "Study Magic Student" }.Contains(archetype)){
            rando += 10;
        }
        if (new string[] { "Mage Student", "Jock Sudent", "Lower Class" }.Contains(archetype)){
            rando -= 10;
        }
        if (physicalTraits[1] == "Buff") {
            rando -= 20;
        }

        if (rando >= top) {
            atributes[5] = "Book-Worm";
            updateInt(4);
            updatePrc(2);
            updateStr(-2);
        }

        else if (rando < top && rando > midBot) {
            atributes[5] = "Average";
        }
        else {
            atributes[5] = "Hands-On";
            updateUnd(4);
            updateStr(2);
            updatePrc(-2);
        }

        // Can they follow orders
        rando = RandomNum();
        if (new string[] { "Lower Class" }.Contains(archetype)){
            rando -= 10;
        }
        if (new string[] { "Government Person", "Mage Student" }.Contains(archetype)){
            rando += 10;
        }

        if (rando >= top) {
            atributes[6] = "Reliable";
            updatePrc(3);
        }

        else if (rando < top && rando > midBot) {
            atributes[6] = "Average";
        }
        else {
            atributes[6] = "Unreliable";
            updateDex(3);
        }
        // Can they lead
        rando = RandomNum();
        if (rando >= top) {
            atributes[7] = "Direct";
            updateChr(10);
        }

        else if (rando < top && rando > midBot) {
            atributes[7] = "Average";
        }
        else {
            atributes[7] = "Unclear";
            updateChr(-10);
        }
        // Can they Commit to things
        rando = RandomNum();
        if (rando >= top) {
            atributes[8] = "Committed";
            updateInt(4);
        }

        else if (rando < top && rando > midBot) {
            atributes[8] = "Average";
        }
        else {
            atributes[8] = "Procrastinator";
            updateInt(-4);
        }
        // Thinking speed
        rando = RandomNum();
        if (new string[] { "Intellectual Student", "Government Person", "Study Magic Student" }.Contains(archetype)){
            rando += 20;
        }

        if (rando >= top) {
            atributes[9] = "Quick-Thinker";
            updateInt(5);
            updateUnd(5);
            updatePrc(3);
            updateChr(3);
            updatePer(2);
        }

        else if (rando < top && rando > midBot) {
            atributes[9] = "Average";
        }
        else {
            atributes[9] = "Slow";
            updateInt(-5);
            updateUnd(-5);
            updatePrc(-3);
            updateChr(-3);
            updatePer(-2);
        }
        // How Loyal they can be
        rando = RandomNum();
        if (rando >= top) {
            atributes[10] = "Loyal";
        }

        else if (rando < top && rando > midBot) {
            atributes[10] = "Average";
        }
        else {
            atributes[10] = "Unloyal";
        }
        // How easily they are persuaded to do something
        rando = RandomNum();
        if (rando >= top) {
            atributes[11] = "Push-Over";
            updateDex(4);
        }

        else if (rando < top && rando > midBot) {
            atributes[11] = "Average";
        }
        else {
            atributes[11] = "Stubborn";
            updateChr(-4);
            updatePer(1);
        }
        // Their Intelligence
        rando = RandomNum();
        if (new string[] { "Intellectual Student", "Hero", "Government Person", "Study Magic Student" }.Contains(archetype)){
            rando += 10;
        }
        if (new string[] { "Lower Class", "Jock Sudent" }.Contains(archetype)){
            rando -= 10;
        }
        if (atributes[9] == "Slow") {
            rando -= 20;
        }

        if (rando >= top) {
            atributes[12] = "Intelligent";
            updateInt(5);
            updateUnd(3);
        }

        else if (rando < top && rando > midBot) {
            atributes[12] = "Average";
        }
        else {
            atributes[12] = "Unintelligent";
            updateInt(-5);
            updateUnd(-3);
        }
        // Their reluctance to leave home
        rando = RandomNum();
        if (new string[] { "Lower Class", "Hero", "Mage Student" }.Contains(archetype)){
            rando += 10;
        }

        if (rando >= top) {
            atributes[13] = "Wonderlust";
            updateDex(4);
            updateStr(4);
            updatePer(-1);
        }

        else if (rando < top && rando > midBot) {
            atributes[13] = "Average";
        }
        else {
            atributes[13] = "Shut-in";
            updateStr(-4);
            updateDex(-4);
            updatePer(2);
        }
        // Are they fun to be around aka Charismatic
        rando = RandomNum();
        if (new string[] { "Jock Sudent", "Government Person" }.Contains(archetype)){
            rando += 10;
        }

        if (rando >= top) {
            atributes[14] = "Charismatic";
            updateChr(10);
        }

        else if (rando < top && rando > midBot) {
            atributes[14] = "Average";
        }
        else {
            atributes[14] = "Dull";
            updateChr(-10);
        }
        // How Friendly are they
        rando = RandomNum();
        if (rando >= top) {
            atributes[15] = "Friendly";
            updateChr(4);
            updatePer(-1);
        }

        else if (rando < top && rando > midBot) {
            atributes[15] = "Average";
        }
        else {
            atributes[15] = "Unfriendly";
            updateChr(-8);
        }

        return atributes;
    }

    /// <summary>
    /// creates and array of Physical Atributes to define a npc
    /// </summary>
    /// <returns></returns>
    private string[] Traits() {
        string[] traits = new string[10];
        int count = 0;
        foreach (string x in traits)
        {//to give me empty spots
            traits[count] = "0";
            count++;
        }
        rando = RandomNum();
        if (rando > 50) {
            traits[0] = "Male";
            updateStr(10);
            updateDex(-5);
        }
        else {
            traits[0] = "Female";
            updateStr(-5);
            updateDex(10);
        }
        // Body Type
        rando = RandomNum();
        if (rando > top) {
            traits[1] = "Buff";
            updateStr(7);
            updateDex(4);
            updatePrc(-2);
        }

        else if (rando < top && rando > midBot) {
            traits[1] = "Average";
        }
        else {
            traits[1] = "Scrawny";
            updateStr(-7);
            updateDex(-2);
            updatePrc(5);
        }
        // Height
        rando = RandomNum();
        if (rando > top) {
            traits[2] = "Tall";
            updateDex(-3);
        }

        else if (rando < top && rando > midBot) {
            traits[2] = "Average";
        }
        else {
            traits[2] = "Short";
            updateDex(4);
        }
        // Hair Color
        string[] List = { "Blonde", "Brown", "Black", "Grey", "White", "Red" };
        rando = RandomNum();
        if (rando > 84) {
            traits[3] = List[0];
        }
        else if (rando <= 84 && rando > 67) {
            traits[3] = List[1];
        }
        else if (rando <= 67 && rando > 51) {
            traits[3] = List[2];
        }
        else if (rando <= 51 && rando > 34) {
            traits[3] = List[3];
        }
        else if (rando <= 34 && rando > 17) {
            traits[3] = List[4];
        }
        else {
            traits[3] = List[5];
        }
        // Hair Type
        string[] hariList = { "Long-Curly-Hair", "Short-Straight-Hair", "Short-Curly-Hair", "Long-Straight-Hair", "Bald-Hair", "Thinning-Hair", "Receding-Hair" };
        rando = RandomNum();
        if (rando > 84)
        {
            traits[4] = hariList[0];
        }
        else if (rando <= 84 && rando > 67)
        {
            traits[4] = hariList[1];
        }
        else if (rando <= 67 && rando > 51)
        {
            traits[4] = hariList[2];
        }
        else if (rando <= 51 && rando > 34)
        {
            traits[4] = hariList[3];
        }
        else if (rando <= 32 && rando > 15){
            traits[4] = hariList[4];
            if (traits[0] == "Female"){
                traits[4] = hariList[5];
            }
        }

        else {
            traits[4] = hariList[6];
            if (traits[0] == "Female")
            {
                traits[4] = hariList[5];
            }
        }
        // Skin Tone
        rando = RandomNum();
        if (rando >= 40) {
            traits[5] = "White";
        }
        else if (rando < 40 && rando >= 30){
            traits[5] = "Olive";
        }
        else if (rando < 30 && rando >= 20) {
            traits[5] = "Light-Brown";
        }
        else if (rando < 20 && rando >= 10) {
            traits[5] = "Brown";
        }
        else {
            traits[5] = "Dark-Brown";
        }
        return traits;

    }

    /// <summary>
    /// Runs the whole function and is the basic controller that updates values
    /// </summary>
    public void Run() {
        NpcType();//Creating the Character base stats and Choosing What type they will be peasant Average Joe...
        /*print(archetype);
        if (canUseMagic) {
            print("Can use Magic");
        }
        else {
            print("Cannot use Magic");
        }
        print("Favorite weapon type is");*/

        //Physical Traits being made, possibaly spirt chooser
        physicalTraits = Traits();
        int physicalCount = 0;
        for (int i = 0; i < physicalTraits.Length; i++){
            if (physicalTraits[i] != "0" && physicalTraits[i] != "Average"){
                physicalCount++;
            }
        }

        string[] phTraits = new string[physicalCount];
        physicalCount = 0;
        for (int i = 0; i < physicalTraits.Length; i++){
            if (physicalTraits[i] != "0" && physicalTraits[i] != "Average"){
                phTraits[physicalCount] = physicalTraits[i];
                physicalCount++;
            }
        }
        physicalTraits = phTraits;

        //Personality Traits being made
        personalityTraits = Atributes();
        int personalityCount = 0;
        for (int i = 0; i < personalityTraits.Length; i++){
            if (personalityTraits[i] != "0" && personalityTraits[i] != "Average"){
                personalityCount++;
            }
        }

        string[] perTraits = new string[personalityCount];
        personalityCount = 0;
        for (int i = 0; i < personalityTraits.Length; i++){
            if (personalityTraits[i] != "0" && personalityTraits[i] != "Average"){
                perTraits[personalityCount] = personalityTraits[i];
                personalityCount++;
            }
        }
        personalityTraits = perTraits;
        FinalizeValues();//Fixes any stat issues
        /*print("Intuition:" + intuition);
        print("Intelligence:" + intelligence);
        print("Strength:" + strength);
        print("Charisma:" + charisma);
        print("Precision" + precision);
        print("Dextarity:" + dexterity);
        print("Perception:" + perception);
        int count = 0;
        foreach( string x in personalityTraits){
            if (x != "0" && x != "Average"){
                print(x + "");
            }
        }
        print("");
        //print(personalityTraits);
        foreach (string x in physicalTraits) {
            count++;
            if (x != "0" && x != "Average"){
                if (count == 6) {
                    print("Hair,");
                }

                print(x);
            }
        }
        print("Skin Tone");
        */

}

    /// <summary>
    /// NPC Class that is used to export the npc to Json
    /// </summary>
    [System.Serializable]
    public class NpcSave{
        public string arch;
        public int intu;
        public int inte;
        public int str;
        public int cha;
        public int pre;
        public int dex;
        public int per;
        public int spirit; //Not used for Npcs
        public bool UseMagic;
        public string favWeap;
        public string[] physical;
        public string[] personality;
    }
}
