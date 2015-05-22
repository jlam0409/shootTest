using UnityEngine;
using System;	
using System.Collections;

public class MeshRadioButtonGroupScript : UIControl {
	public MeshToggleButtonScript[] radioButtons = null; 
	public Boolean[] buttonValue;
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
		
	public void SelectButton (MeshToggleButtonScript select_, bool callback=true){
		for (int i=0; i<radioButtons.Length; i++){
			if (select_ == radioButtons[i]){
				buttonSelect = i;
				buttonValue[i] = true;
				radioButtons[i].SetState (MeshToggleButtonState.On);
				if(callback){
					SendCallback();
//					ExecSendCallback(controlListener, buttonCommand, commandArgument);
				}
			} else {
				buttonValue[i] = false;
				radioButtons[i].SetState (MeshToggleButtonState.Off);
			}
		}
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