using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class XMLLoader : MonoBehaviour {
	public TextAsset configFile;

	public void LoadXML() {
		XmlDocument doc = new XmlDocument();
		doc.Load(new StringReader(configFile.text));
		XmlNodeList nodeList = doc.GetElementsByTagName("gameConfig");
		for (int i = 0; i < nodeList.Count; i++) {
			XmlNodeList childNodeList = nodeList[i].ChildNodes;
			for (int j = 0; j < childNodeList.Count; j++) {
				XmlNode cellNode = childNodeList.Item(j);
				string cellNodeName = cellNode.Name;
				string cellValue = (cellNode.InnerText);

				int value = int.Parse (cellValue); 
				LoadAttribute (cellNodeName, value);
			}
		}
	}

	public void LoadAttribute(string key, int value){
		switch (key){
			case "timer":			GameConfigData.instance.timer = value;			break;
			case "debrisCount":		GameConfigData.instance.debrisCount = value;	break;
			case "debrisInterval":	GameConfigData.instance.debrisInterval = value;	break;
			case "debrisReward": 	GameConfigData.instance.debrisReward = value;	break;
		}
	}
}
