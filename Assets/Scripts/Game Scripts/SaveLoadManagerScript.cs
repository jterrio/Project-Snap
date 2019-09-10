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

    }

    // Start is called before the first frame update
    void Start() {
        //singleton
        if(ins == null) {
            ins = this;
        } else if (ins != this) {
            Destroy(this);
        }
    }

    public void LoadData(int i) {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString())) {
            //UIManager.ins.LoadFailed();
            print("Load Failed!");
            return;
        }

        var serializer = new XmlSerializer(typeof(SaveFile));
        var stream = new FileStream(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString(), FileMode.Open);
        sf = serializer.Deserialize(stream) as SaveFile;
        stream.Close();

        SpreadDataToLoad();
        CleanTempData();
        //UIManager.ins.LoadSuccess();
    }

    public void SaveData(int i) {
        var serializer = new XmlSerializer(typeof(SaveFile));
        var stream = new FileStream(Application.dataPath + "/StreamingAssets/Saves/" + "File" + i.ToString(), FileMode.Create);

        CollectDataToSave();
        serializer.Serialize(stream, sf);
        stream.Close();
        CleanTempData();
        //UIManager.ins.SaveSuccess();
    }

    public void CollectDataToSave() {
        sf.npcData = NPCManagerScript.ins.CollectCurrentData();
    }

    public void CleanTempData() {
        sf.npcData = null;
    }

    public void SpreadDataToLoad() {
        NPCManagerScript.ins.allNPCData = sf.npcData;

        NPCManagerScript.ins.LoadNPCSceneData();
    }


}
