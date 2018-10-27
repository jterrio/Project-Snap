using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManagerScript : MonoBehaviour {

    public static FactionManagerScript ins;
    [SerializeField]
    private List<FactionRelation> factionRelations;// This is a list of all the relations that factions have of each other.
    //These Floats are for checking the Relationship Methods
    public float doesHateUpperBound = -60, doesDislikeUpperBound = -10, isNeutralLowerBound = -10, isNeutralUpperBound = 10, doesLikeLowerBound = 10, doesLoveLowerBound = 60;

    public enum Faction {
        EMPIRE, //those allied to sasha but not in black rose and are fighting against daud
        REBELS, //daud and his crew
        BLACKROSE, //sasha and her crew
        METRODORA, //isabel and her crew
        CIVILIAN, //civilians, like random people or merchants
        GANG //expand on this (for multiple gangs)
    }


    [System.Serializable]
    public class FactionRelation {
        public Faction faction1;
        public Faction faction2;
        public float opinion;
    }

    void Start () {
        //singleton
        if (ins == null) {
            ins = this;
        } else {
            if (ins != this) {
                Destroy(gameObject);
            }
        }
        //print(IsNeutral(Faction.GANG, Faction.CIVILIAN));
        //SetFactionRelation(Faction.BLACKROSE, Faction.EMPIRE, -999);
    }

    public void SetFactionRelation(Faction a,Faction b, float newRelation) { //Updates the relation faction A has to faction B.
        if (a == b) {
            return;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                fr.opinion = newRelation;
                Validate(fr);
                return;
            }
        }
    }

    void Validate(FactionRelation relation) { // Checks to see if relation is within certain value. If not sets to highest or lowest possiable.
        if(relation.opinion < -100) {
            relation.opinion = -100;
        }
        else if(relation.opinion > 100) {
            relation.opinion = 100;
        }
    }

    public float GetFactionRelation(Faction a,Faction b) { //Returns the relation faction A has of faction B. ex: A thinks -50 of B.
        if (a == b) {
            return 0;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return fr.opinion;
            }
        }
        return 0;
    }

    public float AddToFactionRelation(Faction a, Faction b, float addedValue) { // Updates the opinion that Faction A has of Faction B, through addition. You pass Negatives in addedValue to subtract.
        if (a == b) {
            return 0;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return (fr.opinion += addedValue);
            }
        }
        return 0;//This is not right!!
    }


    // Begining of Relationship Methods 
    
    public bool DoesHate(Faction a, Faction b) { // if >= -100 or < -60 "Faction A hates Faction B." If not "they do not hate them."
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return (fr.opinion < doesHateUpperBound);
            }
        }
        return false;
    }
    public bool DoesDislike(Faction a, Faction b) { // if >= -60 or < -10 "Faction A dislikes Faction B." If not "they do not dislike them."
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) { 
                return (fr.opinion < doesDislikeUpperBound);
            }
        }
        return false;
    }
    public bool IsNeutral(Faction a, Faction b) { // if >= -10 or <= 10 "Faction A is neutral towards Faction B." Or they are not.
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return ((fr.opinion >= isNeutralLowerBound) && (fr.opinion <= isNeutralUpperBound));
            }
        }
        return false;
    }
    public bool DoesLike(Faction a, Faction b) { // if > 10 or <= 60 "Faction A likes Faction B." Or they do not.
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return (fr.opinion > doesLikeLowerBound);
            }
        }
        return false;
    }
    public bool DoesLove(Faction a, Faction b) { // if > 60 or <= 100 "Faction A loves Faction B." Or they do not.
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if ((a == fr.faction1 || a == fr.faction2) && (b == fr.faction1 || b == fr.faction2)) {
                return (fr.opinion > doesLoveLowerBound);
            }
        }
        return false;
    }   

    //End of Relationship Methods
}
