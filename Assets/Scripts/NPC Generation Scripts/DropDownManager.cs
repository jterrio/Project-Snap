using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownManager : MonoBehaviour
{
    [Header("Character Info")]
    public Dropdown archetype;
    public Dropdown weaponType;
    public Dropdown canMagic;
    public Traits traits;

    void Start(){
        PopulateDropdowns();
    }

    // Update is called once per frame
    void Update(){
        
    }
    /// <summary>
    /// Loads the options for any dropdowns in the custom generation scene
    /// </summary>
    void PopulateDropdowns() {
        //Loading the Archetype Dropdown
        string[] arrayArche = Enum.GetNames(typeof(NPC.Archetype));
        List<string> listArche = new List<string>(arrayArche);
        archetype.AddOptions(listArche);

        //Loading the Weapon Type Dropdown
        string[] arrayWeapon = Enum.GetNames(typeof(NPC.WeaponType));
        List<string> listWeapon = new List<string>(arrayWeapon);
        weaponType.AddOptions(listWeapon);

        //Loading the Weapon Type Dropdown
        string[] arrayMagic = Enum.GetNames(typeof(NPC.Magic));
        List<string> listMagic = new List<string>(arrayMagic);
        canMagic.AddOptions(listMagic);

        //Not set up therefore it is giving errors and needs to be commented out.

        ////Loading the Integrity Dropdown
        //string[] arrayIntegrity = Enum.GetNames(typeof(NPC.Integrity));
        //List<string> listIntegrity = new List<string>(arrayIntegrity);
        //traits.integrity.AddOptions(listIntegrity);

        ////Loading the Creativity Dropdown
        //string[] arrayCreativity = Enum.GetNames(typeof(NPC.Creativity));
        //List<string> listCreativity = new List<string>(arrayCreativity);
        //traits.creativity.AddOptions(listCreativity);

        ////Loading the Humor Dropdown
        //string[] arrayHumor = Enum.GetNames(typeof(NPC.Humor));
        //List<string> listHumor = new List<string>(arrayHumor);
        //traits.humor.AddOptions(listHumor);

        ////Loading the Attitude Dropdown
        //string[] arrayAttitude = Enum.GetNames(typeof(NPC.Attitude));
        //List<string> listAttitude = new List<string>(arrayAttitude);
        //traits.attitude.AddOptions(listAttitude);

        ////Loading the Mind Dropdown
        //string[] arrayMind = Enum.GetNames(typeof(NPC.Mind));
        //List<string> listMind = new List<string>(arrayMind);
        //traits.mind.AddOptions(listMind);

        ////Loading the Reliability Dropdown
        //string[] arrayReliability = Enum.GetNames(typeof(NPC.Reliability));
        //List<string> listReliability = new List<string>(arrayReliability);
        //traits.reliability.AddOptions(listReliability);

        ////Loading the Learning Dropdown
        //string[] arrayLearning = Enum.GetNames(typeof(NPC.Learning));
        //List<string> listLearning = new List<string>(arrayLearning);
        //traits.learning.AddOptions(listLearning);

        ////Loading the Leadership Dropdown
        //string[] arrayLeadership = Enum.GetNames(typeof(NPC.Leadership));
        //List<string> listLeadership = new List<string>(arrayLeadership);
        //traits.leadership.AddOptions(listLeadership);

        ////Loading the Commitment Dropdown
        //string[] arrayCommitment = Enum.GetNames(typeof(NPC.Commitment));
        //List<string> listCommitment = new List<string>(arrayCommitment);
        //traits.commitment.AddOptions(listCommitment);

        ////Loading the Thinking Dropdown
        //string[] arrayThinking = Enum.GetNames(typeof(NPC.Thinking));
        //List<string> listThinking = new List<string>(arrayThinking);
        //traits.thinking.AddOptions(listThinking);

        ////Loading the Loyal Dropdown
        //string[] arrayLoyal = Enum.GetNames(typeof(NPC.Loyal));
        //List<string> listLoyal = new List<string>(arrayLoyal);
        //traits.loyal.AddOptions(listLoyal);

        ////Loading the Persuadability Dropdown
        //string[] arrayPersuadability = Enum.GetNames(typeof(NPC.Persuadability));
        //List<string> listPersuadability = new List<string>(arrayPersuadability);
        //traits.persuadability.AddOptions(listPersuadability);

        ////Loading the Intelligence Dropdown
        //string[] arrayIntelligence = Enum.GetNames(typeof(NPC.Intelligence));
        //List<string> listIntelligence = new List<string>(arrayIntelligence);
        //traits.intelligence.AddOptions(listIntelligence);

        ////Loading the Travel Dropdown
        //string[] arrayTravel = Enum.GetNames(typeof(NPC.Travel));
        //List<string> listTravel = new List<string>(arrayTravel);
        //traits.travel.AddOptions(listTravel);

        ////Loading the Charisma Dropdown
        //string[] arrayCharisma = Enum.GetNames(typeof(NPC.Charisma));
        //List<string> listCharisma = new List<string>(arrayCharisma);
        //traits.charisma.AddOptions(listCharisma);

        ////Loading the Friendship Dropdown
        //string[] arrayFriendship = Enum.GetNames(typeof(NPC.Friendship));
        //List<string> listFriendship = new List<string>(arrayFriendship);
        //traits.friendship.AddOptions(listFriendship);

        ////Loading the Gender Dropdown
        //string[] arrayGender = Enum.GetNames(typeof(NPC.Gender));
        //List<string> listGender = new List<string>(arrayGender);
        //traits.gender.AddOptions(listGender);

        ////Loading the Body Dropdown
        //string[] arrayBody = Enum.GetNames(typeof(NPC.Body));
        //List<string> listBody = new List<string>(arrayBody);
        //traits.body.AddOptions(listBody);

        ////Loading the Height Dropdown
        //string[] arrayHeight = Enum.GetNames(typeof(NPC.Height));
        //List<string> listHeight = new List<string>(arrayHeight);
        //traits.height.AddOptions(listHeight);

        ////Loading the Hair Dropdown
        //string[] arrayHair = Enum.GetNames(typeof(NPC.Hair));
        //List<string> listHair = new List<string>(arrayHair);
        //traits.hair.AddOptions(listHair);

        ////Loading the HairColor Dropdown
        //string[] arrayHairColor = Enum.GetNames(typeof(NPC.HairColor));
        //List<string> listHairColor = new List<string>(arrayHairColor);
        //traits.hairColor.AddOptions(listHairColor);

        ////Loading the SkinTone Dropdown
        //string[] arraySkinTone = Enum.GetNames(typeof(NPC.SkinTone));
        //List<string> listSkinTone = new List<string>(arraySkinTone);
        //traits.skinTone.AddOptions(listSkinTone);
    }

    /*public List<string> EnumToList(enum toBeList) {
        public string[] array = Enum.GetNames(typeof(toBeList));
        List<string> list = new List<string>(array);
        return list;
    }*/ // Condense method for the above code. Needs to be worked on and then reduce the code with it.

    [System.Serializable]
    public class Traits
    {

        [Header("Personality Traits")]
        public Dropdown integrity;
        public Dropdown creativity;
        public Dropdown humor;
        public Dropdown attitude;
        public Dropdown mind;
        public Dropdown reliability;
        public Dropdown learning;
        public Dropdown leadership;
        public Dropdown commitment;
        public Dropdown thinking;
        public Dropdown loyal;
        public Dropdown persuadability;
        public Dropdown intelligence;
        public Dropdown travel;
        public Dropdown charisma;
        public Dropdown friendship;

        [Header("Physical Traits")]
        public Dropdown gender;
        public Dropdown body;
        public Dropdown height;
        public Dropdown hair;
        public Dropdown hairColor;
        public Dropdown skinTone;
    }
}
