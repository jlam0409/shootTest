using UnityEngine;
using System;	
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class MeshToggleButtonScript : UIControl {
	public MeshRadioButtonGroupScript radioButtonGroup = null;
	public MeshLevelButtonGroupScript levelButtonGroup = null;
	
	public MeshToggleButtonState buttonState = MeshToggleButtonState.On;
	public MeshToggleButtonState lockState = MeshToggleButtonState.Unlock;

	public Callback onCallback;
	public Callback offCallback;

	public Renderer textureObject = null;
	public Texture onTexture = null;
	public Texture offTexture = null;
	public Texture lockTexture = null;
	
	public ButtonStateChangeMode mode = ButtonStateChangeMode.SetTexture;
	
	public AudioClip sound = null;
	
	public void Awake(){
		if (lockState == MeshToggleButtonState.Lock){
			SetState (MeshToggleButtonState.Lock);
		} else {
			SetState (buttonState, false);
		}
		
	}
		
	public MeshToggleButtonState OnTouchDown (TouchData touchData_){
		if (lockState == MeshToggleButtonState.Lock)
			return MeshToggleButtonState.Lock;
		
		if (sound != null){
			AudioManager.RequestPlayButtonSfx( sound );	
		}
		
		MeshToggleButtonState newState = MeshToggleButtonState.None;
		if (radioButtonGroup != null) {
			radioButtonGroup.SelectButton (this);
			newState = MeshToggleButtonState.On;
		} else if (levelButtonGroup != null){
				levelButtonGroup.SelectButton(this);
				newState = MeshToggleButtonState.On;
				SetState (newState);
		} else {
			switch (buttonState){
				case MeshToggleButtonState.On:		newState = MeshToggleButtonState.Off;		break;
				case MeshToggleButtonState.Off:		newState = MeshToggleButtonState.On;		break;
				default:	break;
			}
			SetState (newState);
		}
		
		return newState;
	}
		
	public void LockButton(Boolean lock_){
		if (lock_){
			SetState (MeshToggleButtonState.Lock);
		} else {
			SetState (MeshToggleButtonState.Unlock);
			SetState (buttonState);
		}
	}
	
	public void SetState(MeshToggleButtonState state_, bool callback=true){
		
		Texture bufferTexture = null;
		switch ( state_ ){
			case MeshToggleButtonState.On:
				bufferTexture = onTexture;
				buttonState = state_;
				if (callback){
					onCallback.SendCallback(this);
				}
				break;
			case MeshToggleButtonState.Off:
				bufferTexture = offTexture;
				buttonState = state_;
				if (callback){
					offCallback.SendCallback(this);
				}
				break;
			case MeshToggleButtonState.Lock:
				bufferTexture = lockTexture;
				lockState = state_;
				break;
			case MeshToggleButtonState.Unlock:
				lockState = state_;
				break;
			default:
				bufferTexture = onTexture;
				break;
		}
		
		if (mode == ButtonStateChangeMode.SetTexture) {
			Renderer bufferRenderer;
			if (textureObject == null){
				bufferRenderer = gameObject.renderer;
			} else {
				bufferRenderer = textureObject;	
			}
			
			if (bufferTexture != null){
				bufferRenderer.enabled = true;
				bufferRenderer.material.mainTexture = bufferTexture;
			} else {
				bufferRenderer.enabled = false;
			}
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