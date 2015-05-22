using UnityEngine;
using System.Collections;

public class TouchManagerScript : MonoBehaviour {
	private TouchManager _touchManager;	
	private TouchManager.TouchState _touchState;	/**< Current TouchStateType */
	
	public int touchPriority = 0;
	public float clickTolerance = 0.5f;	/**< If the touchEndTime-touchBeganTime < clickDefineTime, means that touch is a click */
	public float initialDragTolerance = 8.0f;
	public float dragTolerance= 1.0f;	/**< If the touch's draged distance more then dragDefineDistance, means that touch is dragging */
	public float holdTolerance = 1.0f;
	
	private float farClipPlane;
	private Touch touch;
	private bool[] startTimer = new bool[5];
	private float[] touchTimer = new float[5];
	private bool[] haveTouch = new bool[5];
	
	public bool sendTimeOutEvent = false;		/* < If true and Time.timeSinceLevelLoad - lastTouchTime >= noTouchTimeOut, send time out event. */
	private float lastTouchTime = 0.0f;			/* < Time of last touch */
	public float noTouchTimeOut = 180.0f;		/* < Default is 180sec, 3min no touch will send out event */
	public GameObject timeOutEventListener = null;		/* < Event Listener to receive time out event */
	public string timeOutEventCmd = "OnNoTouchTimeOutEvent";	/* < Called function name on Event Listener */
	
	public GameObject[] screenBaseEventListener = null;
	
	/**
		\fn	void Awake ( )
		\brief	Awake function inherit from MonoBehaviour
	*/
	void Awake () {
		farClipPlane = camera.farClipPlane;		
		_touchManager = new TouchManager (screenBaseEventListener, camera, touchPriority, farClipPlane, dragTolerance, initialDragTolerance, clickTolerance, holdTolerance);
		_touchManager.SetState (_touchState);
	}
	void OnDestroy(){
		_touchManager.RemoveManager();
	}
	/**
		\fn	void	Update ( )
		\brief	Update function inherit from MonoBehaviour
	*/
	void Update () {

		if (_touchState ==  TouchManager.TouchState.Lock){
//			Debug.Log ("TouchManager: TouchState: " + _touchState);
			return;
		}		
		
		// Save last touch time
		if (Input.touchCount != 0 || Input.GetMouseButtonDown(0)){
			lastTouchTime = Time.timeSinceLevelLoad;
		}
		// Send Time out Event to the specific listener 
		if (sendTimeOutEvent && Time.timeSinceLevelLoad - lastTouchTime >= noTouchTimeOut)
		{
			if (timeOutEventListener != null && timeOutEventCmd != null && !timeOutEventCmd.Equals("")){
				timeOutEventListener.SendMessage(timeOutEventCmd, null, SendMessageOptions.DontRequireReceiver);
			}
			lastTouchTime = Time.timeSinceLevelLoad;
			return;
		}

//		Debug.Log ("TouchManager: Input.touchCount: " + Input.touchCount);
//		Debug.Log ("TouchManager: Input.GetMouseButtonDown(0): " + Input.GetMouseButtonDown(0));

		for (int touchCount = 0; touchCount < Input.touchCount; ++touchCount) {
			touch = Input.GetTouch(touchCount);
			if (touch.fingerId >= 5){
				continue;
			}

//			Debug.Log("TouchManager: touch.phase: " + touch.phase);
			if (touch.phase == TouchPhase.Began) {
				if (haveTouch[touch.fingerId]) {
//					Debug.Log ("TouchManager: Event= CancelTouch");
					_touchManager.CancelTouch(touch.fingerId);
				}
				haveTouch[touch.fingerId] = true;
			} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
				if (!haveTouch[touch.fingerId]) {
					DebugText.Log ("TestTouch Not TouchDown before TouchEnd or TouchCancel");
					_touchManager.ParseEvent(touch.fingerId, touch.position, TouchPhase.Began);
				}
				haveTouch[touch.fingerId] = false;
			} else {
				if (!haveTouch[touch.fingerId]) {
					haveTouch[touch.fingerId] = true;
					DebugText.Log ("TestTouch Not TouchDown before TouchMove or TouchHold");
					_touchManager.ParseEvent(touch.fingerId, touch.position, TouchPhase.Began);
				}
			}
			
			_touchManager.ParseEvent(touch.fingerId, touch.position, touch.phase);
			
			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved){
				startTimer[touch.fingerId] = true;
				touchTimer[touch.fingerId] = Time.time;
				return;
			} else {
				startTimer[touch.fingerId] = false;
			}
		}
				
		for (int i=0; i<touchTimer.Length; i++){
			if (startTimer[i]){
				if ((Time.time - touchTimer[i]) > 1.0 ){
					DebugText.Log ("Should have TouchUp, but did not see a touchUp event after 1 second");
					_touchManager.CancelTouch(i);
					touchTimer[i] = 0f;
					startTimer[i] = false;
				}
			}
		}

//		Debug.Log ("TouchManager: Input.multiTouchEnabled: " + Input.multiTouchEnabled);
		// use mouse control to do the same thing for debuging in unity3D
		if (Input.touchCount == 0 && Input.multiTouchEnabled) {
//			Debug.Log ("TouchManager: Mouse Event");

			if (Input.GetMouseButtonDown(0)) {
//				Debug.Log ("TouchManager: GetMouseButtonDown");
				_touchManager.ParseEvent (0, Input.mousePosition, TouchPhase.Began);
			} else if (Input.GetMouseButtonUp(0)) {
//				Debug.Log ("TouchManager: GetMouseButtonUp");
				_touchManager.ParseEvent (0, Input.mousePosition, TouchPhase.Ended);
			} else if (Input.GetMouseButton(0)) {
//				Debug.Log ("TouchManager: GetMouseButton");
				_touchManager.ParseEvent (0, Input.mousePosition, TouchPhase.Moved);
			}
		}
		TouchManager.ClearQueue();
	}

	private void SetTouchState ( TouchManager.TouchState touchState_){
		_touchState = touchState_;
		_touchManager.SetState (touchState_);
	}
	
	public void OnPause(){
		_touchManager.SetTouchPropery ("rayDistance", 0.0001f);
	}
	
	public void OnResume(){
		_touchManager.SetTouchPropery ("rayDistance", farClipPlane);
	}
}