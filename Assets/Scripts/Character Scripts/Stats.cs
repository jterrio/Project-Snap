using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {

    //stats - MAX 100
    public int intuition = 0; //und - ability to pickup skills in combat
    public int intelligence = 0; //int - ability to study skills outside of combat
    public int strength = 0; //str - ability to swing hard for physical attacks
    public int charisma = 0; //chr - ability to get people to like you
    public int precision = 0; //prc - ability to aim
    public int spirituality = 0; //spr - ability to survive more attacks
    public int dexterity = 0; //dex - ability to use more attacks or move
    public int perception = 0; //per - ability to process info in combat (default time per turn is 0.75seconds. every level reduces this by 0.025, with a hard cap at 20 points)

    //used for combat calculations
    public int attitude; //atitude towards player and 
    public int fear; //from -100 to 100. -100 is fearless and 100 means they run from everything
    public int danger; //from -100 to 100. danger average of enemies detrmines if fear is ued to make npc run






 /// <summary>
 /// COMBAT
 /// </summary>


    public void SetAttitude(int number) {
        attitude = number;
    }

    public void ChangeAttitude(int number) {
        attitude += number;
    }

    public void SetFear(int number) {
        fear = number;
    }

    public void ChangeFear(int number) {
        fear += number;
    }

    public void SetDanger(int number) {
        danger = number;
    }

    public void ChangeDanger(int number) {
        danger += number;
    }

    /// <summary>
    /// STATS
    /// </summary>

    public void AddOneIntuition() {
        intuition += 1;
    }

    public void AddOneIntelligence() {
        intelligence += 1;
    }

    public void AddOneStrength() {
        strength += 1;
    }

    public void AddOneCharisma() {
        charisma += 1;
    }

    public void AddOnePrecision() {
        precision += 1;
    }

    public void AddOneSpirituality() {
        spirituality += 1;
    }

    public void AddOnePerception() {
        perception += 1;
    }

    public void AddCountIntuition(int count) {
        intuition += count;
    }

    public void AddCountIntelligence(int count) {
        intelligence += count;
    }

    public void AddCountStrength(int count) {
        strength += count;
    }

    public void AddCountCharisma(int count) {
        charisma += count;
    }

    public void AddCountPrecision(int count) {
        precision += count;
    }

    public void AddCountSpirituality(int count) {
        spirituality += count;
    }

    public void AddCountPerception(int count) {
        perception += count;
    }

}
