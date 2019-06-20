using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;

public class NpcLoader : MonoBehaviour{
    public TextAsset xmlRawFile;
    public Text uiText;

    // Use this for initialization
    void Start () {
        string data = "xmlRawFile.text";
        parseXmlFile(data);
    }

    void parseXmlFile(string xmlData){
        string totVal = "";
        XmlDocument xmlDoc = new XmlDocument ();
        xmlDoc.Load( new StringReader(xmlData)); //this is the pain in the ass that is causing an error. Need to fix

        string xmlPathPattern = "//npc";
        // Everything below should work. Just need to fix the loading of the document.
        //XmlNodeList myNodeList = xmlDoc.SelectNodes (xmlPathPattern);
        XmlNode root = xmlDoc.DocumentElement;
        Debug.Log(root.OuterXml);
        //root = root.NextSibling;

        //if(root.HasChildNodes){
            //for(int  i = 0; i < root.ChildNodes.Count; i++){
            //}
        //}
        
        //foreach(XmlNode node in myNodeList){
            XmlNode archetype = root.FirstChild;
            XmlNode magic = archetype.NextSibling;
            XmlNode weapon = magic.NextSibling;

            totVal += " Archetype : "+archetype.InnerXml+"\n Can they use magic : "+ magic.InnerXml+"\n Favorite Weapon : "+weapon.InnerXml+"\n\n";
            uiText.text = totVal;
        //}
    }

     // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
