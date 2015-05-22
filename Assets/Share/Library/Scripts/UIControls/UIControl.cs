using UnityEngine;
using System;	
using System.Collections;

public enum MeshButtonState{
	None,
	Up,
	Down,
	Click,
	Drag,
	Lock
}

public enum MeshToggleButtonState{
	None,
	On,
	Off,
	Lock,
	Unlock
}

public enum MeshSliderState{
	Static,
	Move,
	Lock,
	Unlock
}

public enum MeshScrollState{
	Static,
	StartMove,
	Move,
	FinishMove,
	Lock,
	Unlock,
	Slide,
	FastSlide
}

public class UIControl : MonoBehaviour {	
	public Callback callback = null;
	public Callback onDragCallback = null;
	
	// design of touch Transfer: every UI control should implement the touch Transfer
	public bool touchTransfer = false;
	public GameObject touchTransferListener = null;
	
	public void SendCallback(){
		callback.SendCallback(this);
	}

	public void SendOnDragCallback(){
		onDragCallback.SendCallback(this);
	}
	
	public void SendTouchData(string touchEvent, TouchData touchData_){
		if (touchTransferListener != null){
			touchTransferListener.SendMessage (touchEvent, touchData_, SendMessageOptions.DontRequireReceiver);	
		}
	}
}