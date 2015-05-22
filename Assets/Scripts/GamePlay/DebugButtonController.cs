using UnityEngine;
using System.Collections;
using System;

public class DebugButtonController : MonoBehaviour {
	public TextMesh buttonLabel;
	public MeshButtonScript button;

	public void SetLabel(string label){
		buttonLabel.text = label;
	}

	public void SetCallback (Action callback){
		button.callback.callbackAction = callback;
	}
}
