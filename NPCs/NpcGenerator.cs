using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGenerator : MonoBehaviour {
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

    // Start is called before the first frame update
    void Start() {
        int count;
        NpcType(); //Creating the Character base stats and Choosing What type they will be peasant Average Joe...
        print(archetype, end = "\n");
        if (canUseMagic) {
            print("Can use Magic");
        }
        else {
            print("Cannot use Magic");
        }
        print("Favorite weapon type is", favWeapon, end = "\n");
        physicalTraits = Traits();//Physical Traits being made, possibaly spirt chooser
        personalityTraits = Atributes();//Personality Traits being made
        FinalizeValues();//#Fixes any stat issues
        print("Intuition:", intuition, end = "\n");
        print("Intelligence:", intelligence, end = "\n");
        print("Strength:", strength, end = "\n");
        print("Charisma:", charisma, end = "\n");
        print("Precision", precision, end = "\n");
        print("Dextarity:", dexterity, end = "\n");
        print("Perception:", perception, end = "\n");
        count = 0;
        for x in personalityTraits{
            if (x != 0 and x != "Average"){
            print(x, "", end = "");
        }
        print(end = "\n");
        //print(personalityTraits)
        for x in physicalTraits{
            count += 1;
            if (x != 0 and x != "Average"){
            if (count == 6) {
                print("Hair,", end = " ");
            }
        }
        print(x, "", end = "");

        print("Skin Tone", end = "");
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

    // Updates Intuition
    private void updateUnd(int updatedValue) {
        intuition += updatedValue;
    }

    // Updates Intelligence
    private void updateInt(int updatedValue) {
        intelligence += updatedValue;
    }

    // Updates Strength
    private void updateStr(int updatedValue) {
        strength += updatedValue;
    }

    // Updates Charisma
    private void updateChr(int updatedValue) {
        charisma += updatedValue;
    }

    // Updates Precision
    private void updatePrc(int updatedValue) {
        precision += updatedValue;
    }

    // Updates Spirituality
    private void updateSpr(int updatedValue) {
        spirituality += updatedValue;
    }
    // Updates Dexterity
    private void updateDex(int updatedValue) {
        dexterity += updatedValue;
    }
    // Updates Perception
    private void updatePer(int updatedValue) {
        perception += updatedValue;
    }

    // Checks to see if the value is greater than 100 or less than 0 and corrects it
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

    //Finalizing all the values so they are not over max or under min
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
    //returns a number bewtween 1 and 100
    public int RandomNum() {
        return System.Math.Floor(rnd.Next(1, 100));
    }

    // returns what type of npc they are going to be and the starting stats
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
            else if (rando < 55 && rando >= 35) {
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
                string[] People = { "Laborer", "Soldier", "Thug", "BlackSmith", "Banker" };
                Character(30, 7);
                rando = RandomNum();

                //Laborer
                if (rando > 80) {
                    archetype = People[0];
                }
                // Soldier
                else if (rando >= 80 && rando < 60) {
                    archetype = People[1];
                }
                // Thug
                else if (rando >= 60 && rando < 40) {
                    archetype = People[2];
                }
                // BlackSmith
                else if (rando >= 40 && rando < 20) {
                    archetype = People[3];
                }
                //Banker
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

    // creates an array of atributes to define an npc
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
        if archetype in("Government Person", "Mage Student", "Intellectual Student", "Study Magic Student"):
            rando += 10;


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
        rando = RandomNum()
        if archetype in("Intellectual Student", "Hero", "Government Person", "Study Magic Student"){
            rando -= 10;
        }
        if (archetype == "Jock Sudent" || archetype == "Lower Class") {
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
        if archetype in("Intellectual Student", "Study Magic Student"):
            rando += 10;

        if archetype in("Mage Student", "Jock Sudent", "Lower Class"):
            rando -= 10;

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
        rando = RandomNum()
        if archetype in("Lower Class"){
            rando -= 10;
        }
        if archetype in("Government Person", "Mage Student"):
            rando += 10;


        if (rando >= top) {
            atributes[6] = "Obedient";
            updatePrc(3);
        }

        else if (rando < top && rando > midBot) {
            atributes[6] = "Average";
        }
        else {
            atributes[6] = "Disobedient";
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
        if archetype in("Intellectual Student", "Government Person", "Study Magic Student"):
            rando += 20;


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
        if archetype in("Intellectual Student", "Hero", "Government Person", "Study Magic Student"):
            rando += 10;

        if archetype in("Lower Class", "Jock Sudent"){
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
        if archetype in("Lower Class", "Hero", "Mage Student"):
            rando += 10;


        if (rando >= top) {
            atributes[13] = "Wonderlust";
            updateDex(4);
            updateStr(4);
            updatePer(-1);
        }

        else if (rando < top && rando > midBot) {
            atributes[13] = "Average";
        }
        else:
            atributes[13] = "Shut-in";
        updateStr(-4);
        updateDex(-4);
        updatePer(2);

        // Are they fun to be around aka Charismatic
        rando = RandomNum();
        if archetype in("Jock Sudent", "Government Person"):
            rando += 10;


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

    private string[] Traits() {
        string[] traits = [0] * 10;
        for x in traits{//to give me empty spots
            traits[x] = "0";
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
            if (traits[0] == "Female") {
                updateInt(-20);
            }
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
        if (rando > 84) {
            traits[4] = hariList[0];
        }
        else if (rando <= 84 && rando > 67) {
            traits[4] = hariList[1];
        }
        else if (rando <= 67 && rando > 51) {
            traits[4] = hariList[2];
        }
        else if (rando <= 51 && rando > 34) {
            traits[4] = hariList[3];
        }
        else if (rando <= 32 && rando > 15) {
            traits[4] = hariList[4];
            if (traits[0] == "Female"):
                traits[4] = hariList[5];
        }
        else {
            traits[4] = hariList[6];
        }
        // Skin Tone
        rando = RandomNum();
        if (rando >= 30) {
            traits[5] = "White";
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

    def Run(){
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    global archetype
    global canUseMagic
    global favWeapon
    global physicalTraits
    global personalityTraits
    NpcType()#Creating the Character base stats and Choosing What type they will be peasant Average Joe...
    print(archetype, end = "\n")
    if(canUseMagic):
        print("Can use Magic")

    else:
        print("Cannot use Magic")

    print("Favorite weapon type is", favWeapon, end = "\n")
    physicalTraits = Traits()#Physical Traits being made, possibaly spirt chooser
    personalityTraits = Atributes()#Personality Traits being made
    FinalizeValues()#Fixes any stat issues
    print("Intuition:", intuition, end = "\n")
    print("Intelligence:", intelligence, end = "\n")
    print("Strength:", strength, end = "\n")
    print("Charisma:", charisma, end = "\n")
    print("Precision", precision, end = "\n")
    print("Dextarity:", dexterity, end = "\n")
    print("Perception:", perception, end = "\n")
    count = 0
    for x in personalityTraits:
        if(x != 0 and x != "Average"):
            print(x, "" , end = "")
    print(end = "\n")
    #print(personalityTraits)
    for x in physicalTraits:
        count += 1
        if(x != 0 and x != "Average"):
            if(count == 6):
                print("Hair,", end = " ")


            print(x, "" , end = "")

    print("Skin Tone", end = "")


}
    /*// Update is called once per frame
    void Update()
    {
        
    }*/
}
