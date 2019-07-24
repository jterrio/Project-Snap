using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NpcSaver{

    public static readonly string NPC_FOLDER = Application.dataPath + "/NPCs/CreatedNPCs/";

   public static void Init(){
        // Makes sure the Folder for the NPCs is there
        if (!Directory.Exists(NPC_FOLDER)){
            //Create NPC Folder
            Directory.CreateDirectory(NPC_FOLDER);
        }
    }

    public static void Save(string npcSave,string npcName){
        File.WriteAllText(NPC_FOLDER + "/" + npcName + "Npc.txt", npcSave);
    }
}
