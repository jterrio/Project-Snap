﻿using System.Collections;
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
        GANG, //expand on this (for multiple gangs)
        ISABEL //player faction 
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
        DontDestroyOnLoad(gameObject);
        CreateFactionRelations();
        //print(IsNeutral(Faction.GANG, Faction.CIVILIAN));
        //SetFactionRelation(Faction.BLACKROSE, Faction.EMPIRE, -999);
    }
    /// <summary>
    /// Creates all the faction relations
    /// </summary>
    public void CreateFactionRelations() {
        if(factionRelations.Count >= System.Enum.GetNames(typeof(Faction)).Length) {
            return;
        }

        foreach (Faction faction in (Faction[])System.Enum.GetValues(typeof(Faction))) {
            foreach (Faction faction2 in (Faction[])System.Enum.GetValues(typeof(Faction))){
                CreateFactionRelation(faction, faction2);
            }
        }
    }
    /// <summary>
    /// Checks to see if a faction relation exists
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DoesFactionRelationExist(Faction a,Faction b) {
        foreach (FactionRelation fr in factionRelations) {
            if((fr.faction1 == a || fr.faction1 == b) && (fr.faction2 == a || fr.faction2 == b) && (a != b)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Creates a singular relation
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public void CreateFactionRelation(Faction a, Faction b) {
        
        if ((int)a != (int)b) {
            if (!DoesFactionRelationExist(a, b)) {
                FactionRelation fr = new FactionRelation();
                fr.faction1 = a;
                fr.faction2 = b;
                factionRelations.Add(fr);
            }
        }
    }
    /// <summary>
    /// Updates the relation faction A has to faction B.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="newRelation"></param>
    public void SetFactionRelation(Faction a,Faction b, float newRelation) {
        if ((int)a == (int)b) {
            return;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                fr.opinion = newRelation;
                Validate(fr);
                return;
            }
        }
    }

    /// <summary>
    /// Checks to see if relation is within certain value. If not sets to highest or lowest possiable.
    /// </summary>
    /// <param name="relation"></param>
    void Validate(FactionRelation relation) { 
        if(relation.opinion < -100) {
            relation.opinion = -100;
        }
        else if(relation.opinion > 100) {
            relation.opinion = 100;
        }
    }

    /// <summary>
    /// //Returns the relation faction A has of faction B. ex: A thinks -50 of B.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public float GetFactionRelation(Faction a,Faction b) {
        if ((int)a == (int)b) {
            return 0;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return fr.opinion;
            }
        }
        return 0;
    }

    /// <summary>
    /// Updates the opinion that Faction A has of Faction B, through addition. You pass Negatives in addedValue to subtract.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="addedValue"></param>
    /// <returns></returns>
    public float AddToFactionRelation(Faction a, Faction b, float addedValue) { 
        if ((int)a == (int)b) {
            return 0;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return (fr.opinion += addedValue);
            }
        }
        return 0;//This is not right!!
    }


    // Begining of Relationship Methods 

    /// <summary>
    /// if >= -100 or < -60 "Faction A hates Faction B." If not "they do not hate them."
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DoesHate(Faction a, Faction b) {
        if ((int)a == (int)b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return (fr.opinion < doesHateUpperBound);
            }
        }
        return false;
    }

    /// <summary>
    /// if >= -60 or < -10 "Faction A dislikes Faction B." If not "they do not dislike them."
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DoesDislike(Faction a, Faction b) {
        if ((int)a == (int)b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) { 
                return (fr.opinion < doesDislikeUpperBound);
            }
        }
        return false;
    }

    /// <summary>
    /// if >= -10 or <= 10 "Faction A is neutral towards Faction B." Or they are not.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsNeutral(Faction a, Faction b) {
        if ((int)a == (int)b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return ((fr.opinion >= isNeutralLowerBound) && (fr.opinion <= isNeutralUpperBound));
            }
        }
        return false;
    }

    /// <summary>
    /// if > 10 or <= 60 "Faction A likes Faction B." Or they do not.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DoesLike(Faction a, Faction b) {
        if ((int)a == (int)b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return (fr.opinion > doesLikeLowerBound);
            }
        }
        return false;
    }

    /// <summary>
    /// if > 60 or <= 100 "Faction A loves Faction B." Or they do not.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DoesLove(Faction a, Faction b) {
        if (a == b) {
            return false;
        }
        foreach (FactionRelation fr in factionRelations) {
            if (((int)a == (int)fr.faction1 || (int)a == (int)fr.faction2) && ((int)b == (int)fr.faction1 || (int)b == (int)fr.faction2)) {
                return (fr.opinion > doesLoveLowerBound);
            }
        }
        return false;
    }   

    //End of Relationship Methods

    public List<FactionRelation> ExportFactionRelations() {
        return factionRelations;
    }

    public void ImportFactionRelations(List<FactionRelation> frdList) {
        factionRelations = frdList;
    }

}
