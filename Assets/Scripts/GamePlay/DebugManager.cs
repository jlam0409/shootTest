using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DebugManager : MonoBehaviour {
	public GameObject debugPrefab;
	public GameObject debugButtonParent;

	public List<DebugButtonController> buttonList;

	public void InitButtonList(){
		buttonList.Add ( CreateButton (new Vector3 (-4f,8f, (2f-0.8f*buttonList.Count)), "Win", GameManager.instance.WinGame));
		buttonList.Add ( CreateButton (new Vector3 (-4f,8f, (2f-0.8f*buttonList.Count)), "Lose", GameManager.instance.LoseGame));
		buttonList.Add ( CreateButton (new Vector3 (-4f,8f, (2f-0.8f*buttonList.Count)), "Add bullet", AddBullet));
	}

	public DebugButtonController CreateButton(Vector3 localPosition, string buttonLabel, Action buttonCallback){
		GameObject buttonGO = Instantiate (debugPrefab, localPosition,Quaternion.identity) as GameObject;
		DebugButtonController controller = buttonGO.GetComponent<DebugButtonController>();
		controller.SetLabel(buttonLabel);
		controller.SetCallback(buttonCallback);
		buttonGO.transform.parent = debugButtonParent.transform;
		buttonGO.transform.localPosition = localPosition;

		return controller;
	}

	public void AddBullet(){
		PlayerData.bullet += 20;
	}

	public void ExpandDebugGroup(){
		if (buttonList.Count == 0){
			InitButtonList();
		}
		debugButtonParent.SetActive(true);

	}

	public void CollapseDebugGroup(){
		debugButtonParent.SetActive(false);
	}


}
