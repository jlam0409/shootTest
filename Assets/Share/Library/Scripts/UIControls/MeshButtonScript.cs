using UnityEngine;
using System;	
using System.Collections;

public enum ButtonStateChangeMode{
	None = 0,
	SetTexture = 1,
	SetValue = 2,
	SetTextureValue = 3,
	SetSaturation = 4,	// not implement yet
}

public enum TouchEventConverter{
	None,
	OnTouchUp,
	OnTouchDown,
	OnTouchClick,
	OnTouchDrag,
	OnTouchHold,
	OnTouchDragUp,
	OnTouchLeaveUp
}

public enum CallbackTouchEvent{
	None,
	OnTouchUp,
	OnTouchDown,
	OnTouchDrag
}

[RequireComponent (typeof (BoxCollider))]
public class MeshButtonScript : UIControl {
	public CallbackTouchEvent callbackAtEvent = CallbackTouchEvent.OnTouchUp;
	public MeshButtonState defaultState = MeshButtonState.Up;	
	public MeshButtonState buttonState = MeshButtonState.None;
	public MeshButtonState dragDisplayState = MeshButtonState.Up;
	
	public bool enableTouchConverter = false;
	public TouchEventConverter onTouchClickConverter = TouchEventConverter.None;
	public TouchEventConverter onTouchHoldConverter = TouchEventConverter.None;
	public TouchEventConverter onTouchDragUpConverter = TouchEventConverter.None;
	public TouchEventConverter onTouchDragConverter = TouchEventConverter.None;
	
	//public bool setTexture = true;
	public ButtonStateChangeMode mode = ButtonStateChangeMode.SetTexture;
	public int upValue = 127;
	public int downValue = 107;
	public int dragValue = 117;
	public int lockValue = 97;
	
	public Texture upTexture = null;
	public Texture downTexture = null;
	public Texture dragTexture = null;
	public Texture lockTexture = null;
	
	public AudioClip sound = null;
	private Renderer _meshRenderer;
	
	private BoxCollider _boxCollider;
	private Vector3 _colliderSize;
	private Vector3 _colliderCenter;
	
	public void Awake(){
		_meshRenderer = gameObject.renderer;
		_boxCollider = gameObject.GetComponent ("BoxCollider") as BoxCollider;
		_colliderCenter = _boxCollider.center;
		_colliderSize = _boxCollider.size;
		
		// colliderSize should not be zero, automatically set a to a minimum value 0.05 if it is 0
		if ( _boxCollider.size.y == 0){
			_colliderSize.y = 0.05f;
			_boxCollider.size = _colliderSize;
		}
		
		SetState (defaultState);
	}
	
	//Start handle touch events----------------------------------------------------------------------------------
	public void OnTouchDown (TouchData touchData_){
//		DebugText.Log ("OnTouchDown received");
		if (touchTransfer){
			SendTouchData("OnTouchDown", touchData_);
		}
		
		if (buttonState != MeshButtonState.Up){
			return;
		}
		
		// set state
		SetState (MeshButtonState.Down);

		// not request sound play 
		if (sound != null && callbackAtEvent == CallbackTouchEvent.OnTouchDown) 
			AudioManager.RequestPlayButtonSfx( sound );			
		
		// send callback
		if (callbackAtEvent == CallbackTouchEvent.OnTouchDown){
			SendCallback();
		}
	}
	
	public void OnTouchUp (TouchData touchData_){
		//DebugText.Log ("OnTouchUp received");
		if (touchTransfer){
			SendTouchData("OnTouchUp", touchData_);
		}
		
		//only respond when the button is down
		if (buttonState != MeshButtonState.Down){
			return;
		}
		
		//set state
		SetState (MeshButtonState.Up);

		if (sound != null){
			AudioManager.RequestPlayButtonSfx( sound );		
		}
		
		// send callback
		if (callbackAtEvent == CallbackTouchEvent.OnTouchUp){
			SendCallback();
		}
	}
	
	public void OnTouchLeaveUp (TouchData touchData_){
		//DebugText.Log ("OnTouchLeaveUp received");
		if (touchTransfer){
			SendTouchData("OnTouchLeaveUp", touchData_);
		}
		
		//not respond when the button is locked
		if (buttonState == MeshButtonState.Lock){
			return;
		}
		
		// set state
		SetState (MeshButtonState.Up);	
	}
	
	public void OnTouchDrag (TouchData touchData_){	
		//DebugText.Log ("OnTouchDrag received");
		if (touchTransfer){
			SendTouchData("OnTouchDrag", touchData_);
		}
		
		//not respond when the button is locked
		if (buttonState == MeshButtonState.Lock){
			return;
		}
		
		// convert mouse event
		if (! CheckConverter ("OnTouchDrag", onTouchDragConverter, touchData_) ){
		
			//set state
			if (dragDisplayState == MeshButtonState.Drag){
				SetState (MeshButtonState.Drag);
			} else {
				SetState (dragDisplayState);
			}
			
			if (callbackAtEvent == CallbackTouchEvent.OnTouchDrag){
				SendOnDragCallback();
			}
		}
	}
	
	public void OnTouchDragUp (TouchData touchData_){
		//DebugText.Log ("OnTouchDragUp received");
		if (touchTransfer){
			SendTouchData("OnTouchDragUp", touchData_);
		}
		
		//not respond when the button is locked
		if (buttonState == MeshButtonState.Lock){
			return;
		}
		
		// convert mouse event
		if (! CheckConverter ("OnTouchDragUp", onTouchDragUpConverter, touchData_) ){
			SetState (MeshButtonState.Up);
		}
	}
	
	public void OnTouchHold(TouchData touchData_){
		CheckConverter ("OnTouchHold", onTouchHoldConverter, touchData_);
	}
	
	public void OnTouchClick(TouchData touchData_){
		CheckConverter ("OnTouchClick", onTouchClickConverter, touchData_);
	}

	public bool CheckConverter (string eventFrom, TouchEventConverter converter, TouchData touchData_){
		//DebugText.Log ("Converter-  From: " + eventFrom + "\tTo: " + converter);
		if (eventFrom == converter.ToString()){
			//DebugText.LogError ("Event converter cannot convert to itself!");
			return false;
		}
		
		bool convertResult = true;
		switch (converter){
			case TouchEventConverter.OnTouchUp:			OnTouchUp(touchData_);	break;
			case TouchEventConverter.OnTouchDown:		OnTouchDown(touchData_);	break;
			case TouchEventConverter.OnTouchClick:		OnTouchClick(touchData_);	break;
			case TouchEventConverter.OnTouchDrag:		OnTouchDrag(touchData_);	break;
			case TouchEventConverter.OnTouchDragUp:		OnTouchDragUp(touchData_);	break;
			case TouchEventConverter.OnTouchHold:		OnTouchHold (touchData_);	break;
			case TouchEventConverter.OnTouchLeaveUp:	OnTouchLeaveUp (touchData_);	break;
			default:
			case TouchEventConverter.None:			convertResult = false;		break;
		}
		return convertResult;
	}
	//End handle touch events----------------------------------------------------------------------------------
	
	public IEnumerator DelaySetState ( float delayTime_, MeshButtonState state_){
		yield return new WaitForSeconds (delayTime_);
		SetState (state_);
	}
	
	public void LockButton(bool lock_){		
		if (lock_)
			SetState (MeshButtonState.Lock);
		else 
			SetState (MeshButtonState.Up);
	}
	
	public bool IsLocked(){
		return (GetState() == MeshButtonState.Lock);
	}
	public MeshButtonState GetState(){
		return buttonState;
	}
			
	public void SetState(MeshButtonState state_) {
		if (buttonState == state_)
			return;

		buttonState = state_;
		
		if (mode == ButtonStateChangeMode.SetTexture){
			SetStateTexture ( state_ );
		} else if (mode == ButtonStateChangeMode.SetValue){
			SetStateValue( state_ );
		} else if (mode == ButtonStateChangeMode.SetTextureValue){
			SetStateTextureValue (state_);
		}
	}
	
	private void SetStateTextureValue (MeshButtonState state_){
		SetStateTexture ( state_ );
		SetStateValue( state_ );
	}
	
	private void SetStateValue( MeshButtonState state_ ){
		int bufferValue;
		switch ( state_ ){
			case MeshButtonState.Down:	bufferValue = downValue;	break;
			case MeshButtonState.Lock:	bufferValue = lockValue;	break;
			case MeshButtonState.Drag:	bufferValue = dragValue;	break;
			default:
			case MeshButtonState.Up:	bufferValue = upValue;		break;
		}
		float colorValue = bufferValue / 255.0f;
		Color color = new Color(colorValue, colorValue, colorValue, 1f);
		_meshRenderer.material.SetColor("_Color", color);
	}
	
	private void SetStateSaturation (MeshButtonState state_){
		int bufferValue;
		switch ( state_ ){
			case MeshButtonState.Down:	bufferValue = downValue;	break;
			case MeshButtonState.Lock:	bufferValue = lockValue;	break;
			case MeshButtonState.Drag:	bufferValue = dragValue;	break;
			default:
			case MeshButtonState.Up:	bufferValue = upValue;		break;
		}
		
		float colorValue = bufferValue / 255.0f;
		Color color = new Color(colorValue, colorValue, colorValue, 1f);
		_meshRenderer.material.SetColor("_Color", color);
	}
	
	private void SetStateTexture( MeshButtonState state_){
		Texture bufferTexture;
		switch ( state_ ){
			case MeshButtonState.Down:	bufferTexture = downTexture;	break;
			case MeshButtonState.Lock:	bufferTexture = lockTexture;	break;
			case MeshButtonState.Drag:	bufferTexture = dragTexture;	break;
			default:
			case MeshButtonState.Up:	bufferTexture = upTexture;		break;
		}
		
		if (_meshRenderer != null){
			if (bufferTexture != null){
				_meshRenderer.enabled = true;
				_meshRenderer.material.mainTexture = bufferTexture;
			} else {
				_meshRenderer.enabled = false;
			}
		}
	}
	
	// Collider Size equal to zero still have a chance can get the touch(Need test)
	public void EnableCollider(){
		_boxCollider.size = _colliderSize;
		_boxCollider.center = _colliderCenter;
	}
	
	// Collider Size equal to zero still have a chance can get the touch(Need test)
	public void DisableCollider(){
		_boxCollider.size = Vector3.zero;
		_boxCollider.center = Vector3.zero;
	}
}