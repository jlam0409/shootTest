using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TouchManager {
	/**
		\brief	Dispatch touch state
	*/
	public enum TouchState {
		None,	/**< None */
		Lock,
		Unlock
		//LockAllTouch,	/**< Not dispatch touch event */
		//EnableUiTouch,	/**< Only enable ui touch and drag effect */
		//EnableAllTouch	/**< Enable all touch */
	}
	
	/**
		\brief	Dispatch touch event type
	*/
	public enum TouchEvent {
		None,	/**< None */
		OnTouchDown,	/**< OnTouchDown */
		OnTouchUp,	/**< OnTouchUp */
		OnTouchHold,	/**< OnTouchHold */
		OnTouchClick,	/**< OnTouchClick */
		OnTouchDrag,	/**< OnTouchDrag */
		OnTouchDragUp,	/**< OnTouchDragUp */
		OnTouchEnter,
		OnTouchLeave,
		OnTouchLeaveUp,
	}

	private struct TouchDispatchData {
		public int priority;
		public string message;
		public GameObject gameObject;
		public TouchData touchData;
		public TouchEvent touchEvent;
		
		public TouchDispatchData(int priority_, String message_, GameObject gameObject_, TouchData touchData_, TouchEvent touchEvent_) {
			priority = priority_;
			message = message_;
			gameObject = gameObject_;
			touchData = touchData_;
			touchEvent = touchEvent_;
		}
	}
	
	private Camera _camera;
	private TouchState _touchState;// = TouchState.Unlock;
	private TouchData _touchData;

	private int _priority;
	private float _rayDistance;
	private float _dragTolerance;	
	private float _initialDragTolerance;	
	private float _clickTolerance;
	private float _holdTolerance;
	
	private float[] touchBeganTime = new float[5];	/**< Value of all touches began time */
	private Vector2[] touchBeganPosition = new Vector2[5];	/**< Value of all touches began position */
	private Collider[] touchBeganCollider = new Collider[5];	/**< Collider of all touches which the ray hit at touch began */
	private float[] touchBeganRayDistance = new float[5];
	private bool[] touchDragging = new bool[5];	/**< If true, the corresponding touch is dragging */
	private float[] touchHoldBeganTime = new float[5];	/**< Value of all touches began hold time */
	private Vector2[] touchLastPosition = new Vector2[5];	/**< Value of all touches last position */
	
	private bool[] enterSent = new bool[5];
	private bool[] leaveSent = new bool[5];
	
	private static int managerCounter = 0;
	public static bool disableAllTouch = false;
	private static int callCounter = 0;
	
	private static bool checkPriority = false;

	private GameObject[] _screenBaseEventListener = null;
	private bool dispatchUpMessage = false;

	/**
		\brief	Class constructor
	*/
	public TouchManager (GameObject[] screenBaseEventListener_, Camera camera_, int priority_, float rayDistance_, float dragTolerance_, float initialDragTolerance_, float clickTolerance_, float holdTolerance_) {
		_screenBaseEventListener = screenBaseEventListener_;
		_touchData = new TouchData( );
		_priority = priority_;
		if (_priority != 0) {
			checkPriority = true;
		}
		_camera = camera_;
		_rayDistance = rayDistance_;
		_dragTolerance = dragTolerance_;
		
		// _initialDragTolerance should be larger or equal to _dragTolerance
		if (initialDragTolerance_ < dragTolerance_){
			DebugText.LogWarning ("initialDragTolerance must be larger or equal to dragTolerance! initialDragTolerance value changed to dragTolerance");
			initialDragTolerance_ = dragTolerance_;
		}
		_initialDragTolerance = initialDragTolerance_;
		
		_clickTolerance = clickTolerance_;
		if (holdTolerance_ < clickTolerance_){
			DebugText.LogWarning ("holdTolerance must be longer or equal to clickTolerance! holdTolerance value changed to clickTolerance");
			holdTolerance_ = clickTolerance_;
		}
		_holdTolerance = holdTolerance_;
		managerCounter ++;
	}
	
	public void SetTouchPropery(string property_, float value_){
		switch (property_){
			case "priority":		_priority = (int)value_;					break;
			case "rayDistance":		_rayDistance = value_;						break;
			case "dragTolerance":	_dragTolerance = value_;					break;
			case "initialDragTolerance":	_initialDragTolerance = value_;		break;
			case "clickTolerance":	_clickTolerance = value_;					break;
			case "holdTolerance":	_holdTolerance = value_;					break;
			default:	DebugText.Log ("No property named: " + property_ +  " found");	break;
		}
	}
	
	public void AddManager ( ){
		managerCounter ++;	
	}
	
	public void RemoveManager(){
		managerCounter --;	
	}
	
	public void SetState (TouchState touchState_){
		_touchState = touchState_;
	}

	public TouchState GetState (){
		return _touchState;	
	}
	
	private void InitializeTouch ( int touchFingerId_, Vector2 touchPosition_, Collider collider_, float rayDistance_){
		touchBeganTime[touchFingerId_] = Time.time;
		touchBeganPosition[touchFingerId_] = touchPosition_;
		touchBeganCollider[touchFingerId_] = collider_;
		touchBeganRayDistance[touchFingerId_] = rayDistance_;
		touchDragging[touchFingerId_] = false;		
		touchHoldBeganTime[touchFingerId_] = Time.time;
		
		enterSent[touchFingerId_] = false;
		leaveSent[touchFingerId_] = false;
	}
	
	public void CancelTouch(int touchFingerId_) {
		ParseEvent(touchFingerId_, touchLastPosition[touchFingerId_], TouchPhase.Canceled);
	}
	
	/**
		\fn	void	ParseEvent(touchFingerId : int, touchPosition : Vector2, touchPhase : TouchPhase)
		\brief	Parse the event and pass data to dispatch event 
		\param[in]	touchFingerId	The unique index for touch.
		\param[in]	touchPosition	The position of the touch.
		\param[in]	touchPhase	Describes the phase of the touch.
	*/
	public void ParseEvent(int touchFingerId_, Vector2 touchPosition_, TouchPhase touchPhase_) {
		//DebugText.Log ("Parse Event= " + touchFingerId_ + " " + touchPhase_);
		
		// parse OnTouchDown/OnTouchUp/OnTouchDragUp/OnTouchClick Event
		Ray ray = _camera.ScreenPointToRay (touchPosition_);		
		RaycastHit hit;
		bool isClickedObject = Physics.Raycast (ray, out hit, _rayDistance, _camera.cullingMask);
		_touchData.SetData(touchFingerId_, touchPosition_, hit, (touchPhase_ == TouchPhase.Began)? hit.distance : touchBeganRayDistance[touchFingerId_]);
		//DebugText.Log ("Pos : " + _rayDistance);
		
		// type1: Collider static, touch dynamic
		// cases:		criteria [Start Position, End Position, Timing]
		// 1: Click inside collider, hold inside 0.5 sec, leave inside collider 			-> Dispatch OnTouchDown, OnTouchClick, OnTouchUp
		// 2: Click inside collider, hold inside 1 sec, leave inside collider				-> Dispatch OnTouchDown, OnTouchHold, OnTouchUp
		// 3: Click inside collider, hold inside 0.5 sec, leave outside collider 			-> Dispatch OnTouchDown, OnTouchLeave
		// 4: Click inside collider, hold inside 1 sec, leave outside collider				-> Dispatch OnTouchDown, OnTouchHold, OnTouchLeave
		// 5: Click outside collider, hold inside 0.5 sec, leave inside collider			-> Dispatch OnTouchEnter, OnTouchUp
		// 6: Click outside collider, hold inside 1 sec, leave inside collider				-> Dispatch OnTouchEnter, OnTouchHold, OnTouchUp
		// 7: Click outside collider, hold inside 0.5 sec, leave outside collider			-> Nothing dispatch
		// 8: Click outside collider, hold inside 1 sec, leave outside collider				-> Nothing dispatch
		
		// type2: Collider dynamic, touch dynamic
		// cases:		criteria [Start Position, End Position, Timing]
		// 1: Click inside collider, hold inside 0.5 sec, leave inside collider 			-> Dispatch OnTouchDown, OnTouchClick, OnTouchUp
		// 2: Click inside collider, hold inside 1 sec, leave inside collider				-> Dispatch OnTouchDown, OnTouchHold, OnTouchUp
		// 3: Click inside collider, hold inside 0.5 sec, leave outside collider 			-> Dispatch OnTouchDown, OnTouchLeave
		// 4: Click inside collider, hold inside 1 sec, leave outside collider				-> Dispatch OnTouchDown, OnTouchHold, OnTouchLeave
		// 5: Click outside collider, hold inside 0.5 sec, leave inside collider			-> Dispatch OnTouchEnter, OnTouchUp
		// 6: Click outside collider, hold inside 1 sec, leave inside collider				-> Dispatch OnTouchEnter, OnTouchHold, OnTouchUp
		// 7: Click outside collider, hold inside 0.5 sec, leave outside collider			-> Nothing dispatch
		// 8: Click outside collider, hold inside 1 sec, leave outside collider				-> Nothing dispatch
		
		switch (touchPhase_){
			case TouchPhase.Began:	//start
				InitializeTouch(touchFingerId_, touchPosition_, hit.collider, hit.distance);
				DispatchEvent ((isClickedObject)? hit.collider.gameObject : null, _touchData, TouchEvent.OnTouchDown);					
				break;
			case TouchPhase.Moved: 		// drag
			case TouchPhase.Stationary:	// hold
				// if current collider is not the same as began, dispatch OnLeave and OnEnter
				if (touchBeganCollider[touchFingerId_] != null) {
					if (isClickedObject) {
						if (touchBeganCollider[touchFingerId_] != hit.collider){
							if (!leaveSent[touchFingerId_]){
								DispatchEvent (touchBeganCollider[touchFingerId_].gameObject, _touchData, TouchEvent.OnTouchLeave);	
								leaveSent[touchFingerId_] = true;
								enterSent[touchFingerId_] = false;
							}
						
							if (!enterSent[touchFingerId_]){
								DispatchEvent (hit.collider.gameObject, _touchData, TouchEvent.OnTouchEnter);
								enterSent[touchFingerId_] = true;
								leaveSent[touchFingerId_] = false;
							}
						} else {
							if (!enterSent[touchFingerId_]){
								DispatchEvent (touchBeganCollider[touchFingerId_].gameObject, _touchData, TouchEvent.OnTouchEnter);	
								enterSent[touchFingerId_] = true;
								leaveSent[touchFingerId_] = false;
							}
						}
					} else {
						if (!leaveSent[touchFingerId_]){
							DispatchEvent (touchBeganCollider[touchFingerId_].gameObject, _touchData, TouchEvent.OnTouchLeave);
							leaveSent[touchFingerId_] = true;
							enterSent[touchFingerId_] = false;
						}
					}
				}
			
				// calculate the drag distance, we state it as a drag if the distance is larger than the tolerance
				float dragDistance = Vector2.Distance(touchPosition_, touchBeganPosition[touchFingerId_]);
			
				if (!touchDragging[touchFingerId_]){
					if (dragDistance > _initialDragTolerance) {
						touchDragging[touchFingerId_] = true;
						touchBeganPosition[touchFingerId_] = touchPosition_;
						touchHoldBeganTime[touchFingerId_] = Time.time;
					}
				} else {
					if (dragDistance > _dragTolerance) {
						touchDragging[touchFingerId_] = true;
						touchBeganPosition[touchFingerId_] = touchPosition_;
						touchHoldBeganTime[touchFingerId_] = Time.time;
					}
				}		
			/*
				if (touchPhase_ == TouchPhase.Moved){
					if (dragDistance > _dragTolerance) {
						touchDragging[touchFingerId_] = true;
						touchBeganPosition[touchFingerId_] = touchPosition_;
						touchHoldBeganTime[touchFingerId_] = Time.time;
					}
				} else {
					if (dragDistance > _initialDragTolerance) {
						touchDragging[touchFingerId_] = true;
						touchBeganPosition[touchFingerId_] = touchPosition_;
						touchHoldBeganTime[touchFingerId_] = Time.time;
					}

				}
			*/
				if (touchDragging[touchFingerId_]) {
					// call OnTouchDrag in each frame, until the touch not move dragDefineDistance within clickDefineTime
					if (Time.time-touchHoldBeganTime[touchFingerId_] >= _holdTolerance) {
						touchDragging[touchFingerId_] = false;
					}
					if (touchBeganCollider[touchFingerId_] == null) {
						DispatchEvent(null,_touchData, TouchEvent.OnTouchDrag);
					} else {
						DispatchEvent(touchBeganCollider[touchFingerId_].gameObject,_touchData, TouchEvent.OnTouchDrag);
					}
				} else if (Time.time-touchBeganTime[touchFingerId_] >= _holdTolerance) {
					if (touchBeganCollider[touchFingerId_] == null) {
						DispatchEvent(null,_touchData, TouchEvent.OnTouchHold);
					} else {
						DispatchEvent(touchBeganCollider[touchFingerId_].gameObject,_touchData, TouchEvent.OnTouchHold);
					}
				}
			
				// OnTouchUpdate
				DispatchEvent(null,_touchData, TouchEvent.None);
				break;
			case TouchPhase.Canceled:		
			case TouchPhase.Ended:
				//DebugText.Log ("Parsing " + touchPhase_);
				if (touchBeganCollider[touchFingerId_]) {					
					if (touchBeganCollider[touchFingerId_] == hit.collider){ 
						//DebugText.Log ("Dispatch: OnTouchUp, Same collider as Out");	
						DispatchEvent( touchBeganCollider[touchFingerId_].gameObject ,_touchData, TouchEvent.OnTouchUp);
					} else { 
						//DebugText.Log ("Dispatch: OnTouchLeaveUp, Different collider as Out");
						DispatchEvent( touchBeganCollider[touchFingerId_].gameObject ,_touchData, TouchEvent.OnTouchLeaveUp);
					}
				} else {
					//DebugText.Log ("No began collider of touchFingerId_: " + touchFingerId_ );
					DispatchEvent(null ,_touchData, TouchEvent.OnTouchUp);
				}
					
				if (touchDragging[touchFingerId_]) {	// if its dragging, dispatch event drag up
					//DebugText.Log ("Dispatch: OnTouchDragUp, dragging before");
					if (touchBeganCollider[touchFingerId_] == null) {
						DispatchEvent(null,_touchData, TouchEvent.OnTouchDragUp);
					} else {
						DispatchEvent(touchBeganCollider[touchFingerId_].gameObject,_touchData, TouchEvent.OnTouchDragUp);
					}
				} else if (Time.time - touchBeganTime[touchFingerId_] < _clickTolerance) {
					//DebugText.Log ("Dispatch: OnTouchClick, time is short");
					if (touchBeganCollider[touchFingerId_] == null) {
						DispatchEvent(null,_touchData, TouchEvent.OnTouchClick);					
					} else {
						DispatchEvent(touchBeganCollider[touchFingerId_].gameObject,_touchData, TouchEvent.OnTouchClick);					
					}
				}					
				touchBeganCollider[touchFingerId_] = null;
				touchDragging[touchFingerId_] = false;
				break;
		}
		
		// save the last position
		touchLastPosition[touchFingerId_] = touchPosition_;
		
//		ClearQueue();
	}
	
	
	
	/**
		\fn	void	DispatchEvent ( GameObject gameObject_, TouchData touchData_, TouchEvent touchEvent_)
		\brief	Dispatch the event to corresponding object's script
		\param[in]	gameObject_	Dispatch the event to this object
		\param[in]	touchData_	The touch's data need to dispatch
		\param[in]	touchEvent_	Dispatch which type of event
	*/
	private void DispatchEvent ( GameObject gameObject_, TouchData touchData_, TouchEvent touchEvent_){
		
		// Send message with DontRequireReceiver
		String message = "";
		switch (touchEvent_) {
			case TouchEvent.OnTouchDown:		message = "OnTouchDown";		break;
			case TouchEvent.OnTouchUp:			message = "OnTouchUp";			break;
			case TouchEvent.OnTouchHold:		message = "OnTouchHold";		break;
			case TouchEvent.OnTouchClick:		message = "OnTouchClick";		break;
			case TouchEvent.OnTouchDrag:		message = "OnTouchDrag";		break;
			case TouchEvent.OnTouchDragUp:		message = "OnTouchDragUp";		break;
			case TouchEvent.OnTouchEnter:		message = "OnTouchEnter";		break;
			case TouchEvent.OnTouchLeave:		message = "OnTouchLeave";		break;
			case TouchEvent.OnTouchLeaveUp:		message = "OnTouchLeaveUp";		break;
			case TouchEvent.None:				message = "OnTouchUpdate";		break;
		}
		
		//gameObject_.SendMessage(message, touchData_, SendMessageOptions.DontRequireReceiver);
		//DebugText.Log ("TouchManager: Dispatch Event= " + message);
		if (gameObject_) {
			AddEventQueue (_priority, message, gameObject_, touchData_, touchEvent_);
		}
		
		if (disableAllTouch) {
			if (!dispatchUpMessage) {
				dispatchUpMessage = true;
				foreach (GameObject obj in _screenBaseEventListener) {
					obj.SendMessage("OnTouchUp", touchData_, SendMessageOptions.DontRequireReceiver);
				}
			}
			return;
		} else {
			dispatchUpMessage = false;
			foreach (GameObject obj in _screenBaseEventListener) {
				obj.SendMessage (message, touchData_, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	private static object _lock = new object();
	private static List<TouchDispatchData> eventList = new List<TouchDispatchData>();
	private static void AddEventQueue (int priority_, String message_, GameObject gameObject_, TouchData touchData_, TouchEvent touchEvent_){
		if (disableAllTouch)
			return;
		//DebugText.Log ("TestTouch TouchManager: Add Queue: " + message_ + " " + eventList.Count);
		if (checkPriority) {
			// Calculate the priority of touchID, if current one is lower than exists one, discard current one
			TouchDispatchData tempEventData = new TouchDispatchData(priority_, message_, gameObject_, touchData_, touchEvent_);
			
			lock (_lock) {
				Boolean isMatched = false;
				List<TouchDispatchData> bufferEventList = new List<TouchDispatchData>();
				for (int i=0; i<eventList.Count; i++){
					TouchDispatchData curEventList = eventList[i];
					TouchData listTouchData = curEventList.touchData;
					int listPriority = curEventList.priority;
					string message = curEventList.message;
					GameObject tempObject = curEventList.gameObject;
					
					// Check the priority if:
					// - have equal touchId
					// - have equal message
					// - have non-equal gameObject
					if (listTouchData.touchId == touchData_.touchId && 
					    message.Equals(message_)) {
						isMatched = true;
						if (gameObject_ != tempObject || message.Equals("OnTouchDrag")) {
							if (priority_ > listPriority){
								bufferEventList.Add (tempEventData);
							} else if (priority_ == listPriority){
								//DebugText.Log ("TestTouch2 TouchManager: Add Queue");
								bufferEventList.Add (curEventList);
								bufferEventList.Add (tempEventData);
							} else {
								bufferEventList.Add (curEventList);
							}
						}
					} else {
						bufferEventList.Add (curEventList);
					}
				}
				
				if (!isMatched)
					bufferEventList.Add (tempEventData);
				
				eventList = bufferEventList;
				//DebugText.Log (eventList.Count + "");
			}
		} else {
			lock (_lock) {
				gameObject_.SendMessage (message_, touchData_, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public static void ClearQueue(){
		callCounter++;
		if (callCounter < managerCounter){
			//DebugText.Log ("callCounter waiting now is: " + callCounter + " Should be : " + managerCounter );  	
			return;
		} else {
			//DebugText.Log ("callCounter now is: " + callCounter );  	
		}
		
		if (disableAllTouch) {
			eventList.Clear();
			callCounter = 0;
			return;
		}
		
		lock (_lock) {
			//DebugText.Log ("Start Looping");
			for (int i = 0; i < eventList.Count; i++) {
	
				TouchDispatchData curEvent = eventList[i];
				
				GameObject eventListener = curEvent.gameObject;
				String message = curEvent.message;
				TouchData touchData = curEvent.touchData;
				eventListener.SendMessage (message, touchData, SendMessageOptions.DontRequireReceiver);
//				DebugText.Log ("TestTouch Handle : " + i + " message " + message + " name " + eventListener.name);
			}
			eventList.Clear();
			callCounter = 0;
		}
	}
}