using UnityEngine;
using System;	
using System.Collections;

public class MeshLevelButtonGroupScript : UIControl {
	public MeshToggleButtonScript[] radioButtons = null; 
	public int buttonSelect;
	
	public MeshToggleButtonState lockState = MeshToggleButtonState.Unlock;
		
//	public GameObject controlListener = null;
//	public string buttonCommand = "";
//	public string commandArgument = "";

	public void Awake(){
		if (lockState == MeshToggleButtonState.Lock){
			foreach (MeshToggleButtonScript eachButton in radioButtons){
				eachButton.SetState (MeshToggleButtonState.Lock);
			}
		} else if (buttonSelect == -1) {
			SelectButton (null, false);	
		} else {
			SelectButton (radioButtons[buttonSelect], false);	
		}
	}
	
	public bool SetCurrentLevel (int level){
		if (level >= radioButtons.Length || level < 0)
			return false;
		
		SelectButton(radioButtons[level], false);
		return true;
	}
	
	public int GetCurrentLevel(){
		return buttonSelect;
	}
	
	public int GetMaxLevel(){
		return radioButtons.Length-1;
	}
	
	public void SelectButton (MeshToggleButtonScript selected, bool callback=true){
		// get the id of the selected
		int selectedId = -1;
		for (int i=0; i<radioButtons.Length; i++){
			if (radioButtons[i] == selected){
				selectedId = i;
				break;
			}
		}
		
		if (selectedId >=0)
			buttonSelect = selectedId;
		
		for (int i=0; i<radioButtons.Length; i++){
			if (i<=buttonSelect){
				radioButtons[i].SetState (MeshToggleButtonState.On);
			} else {
				radioButtons[i].SetState (MeshToggleButtonState.Off);
			}
		}
		
		if(callback)
			SendCallback();
	}
	
	public void LockButton(Boolean lock_){
		foreach (MeshToggleButtonScript eachButton in radioButtons){
			eachButton.LockButton(lock_);
		}
	}

//	public void ExecSendCallback(GameObject listener_, string command_, string comandArgument_){
//		if (listener_ == null)
//			return;
//		if (comandArgument_ != ""){
//			switch (comandArgument_){
//				case "this":	
//					listener_.SendMessage (command_, this);	
//				break;
//				case "gameObject":
//				case "GameObject":	
//					listener_.SendMessage (command_, gameObject);	
//				break;
//				case "true":
//					listener_.SendMessage (command_, true);
//				break;
//				case "false":
//					listener_.SendMessage (command_, false);
//				break;
//				default: 
//					listener_.SendMessage (command_, comandArgument_);		
//				break;
//			}			
//		}else {
//			listener_.SendMessage (command_);
//		}
//	}
}