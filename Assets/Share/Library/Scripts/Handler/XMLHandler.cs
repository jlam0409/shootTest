using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLHandler<T> {
	private static int ignoreRow = 0;

	public delegate void SetDataFunction(T target, string key, string value);
	public delegate T GetDataObjFunction();
	
	public static List<T> LoadXML(TextAsset xmlAsset, GetDataObjFunction dGetData, SetDataFunction dSetData) {
		XmlDocument doc = new XmlDocument();
		doc.Load(new StringReader(xmlAsset.text));
		return LoadXML(doc, dGetData, dSetData);
	}
	
//	List<Question> questions = XMLHandler<Question>.LoadXML(testXML, new XMLHandler<Question>.GetDataObjFunction(GetDataObj), new XMLHandler<Question>.SetDataFunction(SetData));
	public static List<T> LoadXML(XmlDocument doc, GetDataObjFunction dGetData, SetDataFunction dSetData) {
		// please load the doc before pass into here
		
		List<T> returnList = new List<T>();

		// get all the rows
		XmlNodeList nodeList = doc.GetElementsByTagName("Row");
		
		// Data Count = nodeList.Count-ignoreRow-1	// 1 is header row
		
		List<string> headers = new List<string>();
		
//		DebugText.Log ("Load file : " +doc.Name + " count: " + nodeList.Count);
		for (int i = ignoreRow; i < nodeList.Count; i++) {
			T rowData = dGetData();
//			if (i != ignoreRow) {
//				rowData = dGetData();
//			}
			XmlNodeList childNodeList = nodeList[i].ChildNodes;
			int cellIndex = 0;
			// iterate all the cells
			for (int j = 0; j < childNodeList.Count; j++) {
				XmlNode cellNode = childNodeList.Item(j);
				
				// data nodes
				XmlNodeList cellChildNodeList = cellNode.ChildNodes;
				if (cellChildNodeList.Count >= 1) {
					XmlNode dataNode = cellChildNodeList.Item(0);
					if (dataNode.InnerText == null || dataNode.InnerText.Equals("") || dataNode.InnerText.Equals("NULL")) {
						break;
					}
					if (cellNode.Attributes.Count > 0) {
						//DebugText.Log ("CellNode.Attributes.Count: " + cellNode.Attributes.Count);
						foreach (XmlNode eachAttribute in cellNode.Attributes){
							//DebugText.Log ("cellAttribute:" + eachAttribute.Name );
							if (eachAttribute.Name == "ss:Index"){
								XmlNode cellAttribute = cellNode.Attributes["ss:Index"];
								string cellValue = cellAttribute.Value;
								int curCellIndex = cellIndex;
								cellIndex = int.Parse(cellValue)-1;
								for (int c = curCellIndex; c < cellIndex; c++) {
									dSetData(rowData, headers[c], null);
								}
							}
						}
					}

					//DebugText.Log ("Add to cellIndex: " + cellIndex + " text: " + dataNode.InnerText);
					if (i == ignoreRow) {
						headers.Add(dataNode.InnerText);
					} else {
						dSetData(rowData, headers[cellIndex], dataNode.InnerText);
                    }
					cellIndex++;
				}
			}
			if (cellIndex != 0) {
				for (int c = cellIndex; c < headers.Count; c++) {
					dSetData(rowData, headers[c], null);
				}
				
				if (i != ignoreRow) {
					returnList.Add(rowData);
	            }
			}
		}
		return returnList;
	}
}