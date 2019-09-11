using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

public class SaveLoadManagerScript : MonoBehaviour {

    public static SaveLoadManagerScript ins;
    public SaveFile sf;

    [System.Serializable]
    [XmlRoot("SaveFile")]
    public class SaveFile {

        [XmlArray("NPCData")]
        [XmlArrayItem("NPC")]
        public List<NPCManagerScript.NPCData> npcData;

        [XmlArray("ActiveSpellsData")]
        [XmlArrayItem("Spell")]
        public List<SpellManagerScript.ActiveSpellData> activeSpells;

        [XmlArray("FactionRelationData")]
        [XmlArrayItem("FactionRelation")]
        public List<FactionManagerScript.FactionRelation> factions;

        public NPCManagerScript.PlayerData playerData;

        public QuestManagerScript.PlayerQuestData playerQuestData;
    }

    // Start is called before the first frame update
    void Start() {
        //singleton
        if(ins == null) {
            ins = this;
        } else if (ins != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(int i) {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString() + ".isa")) {
            //UIManager.ins.LoadFailed();
            print("Load Failed!");
            return;
        }

        var serializer = new XmlSerializer(typeof(SaveFile));
        var stream = new FileStream(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString() + ".isa", FileMode.Open);
        sf = serializer.Deserialize(stream) as SaveFile;
        stream.Close();

        SpreadDataToLoad();
        CleanTempData();
        //UIManager.ins.LoadSuccess();
    }

    public void SaveData(int i) {
        var serializer = new XmlSerializer(typeof(SaveFile));
        var stream = new FileStream(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString() + ".isa", FileMode.Create);

        CollectDataToSave();
        serializer.Serialize(stream, sf);
        stream.Close();
        CleanTempData();
        //UIManager.ins.SaveSuccess();
    }

    public void CollectDataToSave() {
        sf.npcData = NPCManagerScript.ins.CollectCurrentData();
        sf.playerData = NPCManagerScript.ins.GetPlayerData();
        sf.activeSpells = SpellManagerScript.ins.GetActiveSpellData();
        sf.factions = FactionManagerScript.ins.ExportFactionRelations();
        sf.playerQuestData = QuestManagerScript.ins.SavePlayerQuestData();
    }

    public void CleanTempData() {
        sf.npcData = null;
        sf.activeSpells = null;
        sf.factions = null;
        sf.playerData = null;
        sf.playerQuestData = null;
    }

    public void SpreadDataToLoad() {
        NPCManagerScript.ins.LoadNPCSceneData(sf.npcData);
        NPCManagerScript.ins.LoadPlayerData(sf.playerData);
        SpellManagerScript.ins.LoadActiveSpellData(sf.activeSpells);
        FactionManagerScript.ins.ImportFactionRelations(sf.factions);
        QuestManagerScript.ins.LoadPlayerQuestData(sf.playerQuestData);
    }


}
