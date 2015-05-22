using UnityEngine;
using System.Collections;

public enum TouchDirectionEvent {
	None,	/**< None */
	OnDirectionDown,		/**< OnDirectionDown */			// Call when touch down
	OnDirectionUp,			/**< OnDirectionUp */			// Call when touch up
	OnDirectionHold,		/**< OnDirectionHold */			// Call when touch hold
	OnDirectionDrag,		/**< OnDirectionDrag */			// Call when Keep Drag
	OnDirectionUpdate,		/**< OnDirectionUpdate */		// Call when either one Direction Update?
	OnDirectionHoriUpdate,	/**< OnDirectionHoriUpdate */	// Call when Hori Direction Update
	OnDirectionVertUpdate,	/**< OnDirectionVertUpdate */	// Call when Vert Direction Update
}

public class TouchDirection : MonoBehaviour {
	public Camera renderCamera = null;
	public Transform refTarget = null;	// if equal null, use the center of the screen
	private bool shotRay = false;
	private Vector3 lastTargetPos = Vector3.zero;
	private Vector3 lastTargetScreenPos = Vector3.zero;
	public float horiCenterRange = 200f;	// in pixel
	public float vertCenterRange = 120f;	// in pixel
	public float horiTolerance = 10f;
	public float vertTolerance = 10f;
	public GameObject[] touchDirectionListener;
	
	private DirectionData directionData = new DirectionData();
	private DirectionData lastDirectionData = new DirectionData();
	
	void Awake() {
		if (renderCamera == null) {
			DebugText.LogError("TouchDirection("+gameObject.name+") : renderCamera == null");
		} else {
			#if UNITY_EDITOR
			TouchManagerScript touchManagerScript = renderCamera.gameObject.GetComponent<TouchManagerScript>();
			if (touchManagerScript != null) {
				bool added = false;
				GameObject _gameObj = gameObject;
				foreach(GameObject obj in touchManagerScript.screenBaseEventListener) {
					if (obj == _gameObj) {
						added = true;
						break;
					}
				}
				if (!added) {
					Debug.LogError("Please add " + _gameObj.name + " to " + renderCamera.gameObject.name + ":TouchManagerScript:screenBaseEventListener");
				}
			}
			#endif
		}
	}
	
	public void OnTouchDown(TouchData touchData_) {
		TouchDown(touchData_);
	}
	public void OnTouchHold(TouchData touchData_) {
		TouchHold(touchData_);
	}
	public void OnTouchDrag(TouchData touchData_) {
		TouchDrag(touchData_);
	}
	public void OnTouchUp(TouchData touchData_) {
		TouchUp(touchData_);
	}
	public void OnTouchLeaveUp(TouchData touchData_) {
		TouchUp(touchData_);
	}
	public void OnTouchUpdate(TouchData touchData_) {
		TouchUpdate(touchData_);
	}

	private void TouchDown(TouchData touchData_) {
		Vector3 targetPos = GetTargetScreenPos();
		directionData.SetData(touchData_.touchId, touchData_.screenPosition, targetPos, horiCenterRange, vertCenterRange);
		lastDirectionData.SetData(directionData);
		DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionDown);
	}
	private void TouchHold(TouchData touchData_) {
		Vector3 targetPos = GetTargetScreenPos();
		directionData.SetData(touchData_.touchId, touchData_.screenPosition, targetPos, horiCenterRange, vertCenterRange);
		DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionHold);
	}
	private void TouchDrag(TouchData touchData_) {
		Vector3 targetPos = GetTargetScreenPos();
		directionData.SetData(touchData_.touchId, touchData_.screenPosition, targetPos, horiCenterRange, vertCenterRange);
		DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionDrag);
	}
	private void TouchUp(TouchData touchData_) {
		Vector3 targetPos = GetTargetScreenPos();
		directionData.SetData(touchData_.touchId, touchData_.screenPosition, targetPos, horiCenterRange, vertCenterRange);
		lastDirectionData.SetData(directionData);
		DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionUp);
	}
	
	private void TouchUpdate(TouchData touchData_) {
		Vector3 targetPos = GetTargetScreenPos();
		directionData.SetData(touchData_.touchId, touchData_.screenPosition, targetPos, horiCenterRange, vertCenterRange);
		if (directionData.horiDirection == lastDirectionData.horiDirection && directionData.vertDirection == lastDirectionData.vertDirection) {
			return;
		}
		
		bool horiUpdate = !directionData.InHoriTolerance(lastDirectionData, horiTolerance);
		bool vertUpdate = !directionData.InVertTolerance(lastDirectionData, vertTolerance);
		
		if (horiUpdate) {
			DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionHoriUpdate);
		} else {
			directionData.horiDirection = lastDirectionData.horiDirection;
		}
		if (vertUpdate) {
			DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionVertUpdate);
		} else {
			directionData.vertDirection = lastDirectionData.vertDirection;
		}
		if (horiUpdate || vertUpdate) {
			DispatchEvent(touchDirectionListener, directionData, TouchDirectionEvent.OnDirectionUpdate);
		}
			
		lastDirectionData.SetData(directionData);
	}
	
	/**
		\fn	void DispatchEvent(GameObject[] gameObjects_, DirectionData directionData_, TouchDirectionEvent touchEvent_)
		\brief	Dispatch the event to corresponding object's script
		\param[in]	gameObjects_	Dispatch the event to those objects
		\param[in]	touchDirection_	The touch's direction data need to dispatch
		\param[in]	touchEvent_	Dispatch which type of event
	*/
	private void DispatchEvent(GameObject[] gameObjects_, DirectionData directionData_, TouchDirectionEvent touchEvent_) {
		// Send message with DontRequireReceiver
		string message = "";
		switch (touchEvent_) {
			case TouchDirectionEvent.OnDirectionDown:			message = "OnDirectionDown";		break;
			case TouchDirectionEvent.OnDirectionUp:				message = "OnDirectionUp";			break;
			case TouchDirectionEvent.OnDirectionHold:			message = "OnDirectionHold";		break;
			case TouchDirectionEvent.OnDirectionDrag:			message = "OnDirectionDrag";		break;
			case TouchDirectionEvent.OnDirectionUpdate:			message = "OnDirectionUpdate";		break;
			case TouchDirectionEvent.OnDirectionHoriUpdate:		message = "OnDirectionHoriUpdate";	break;
			case TouchDirectionEvent.OnDirectionVertUpdate:		message = "OnDirectionVertUpdate";	break;
		}
		
		foreach (GameObject obj in gameObjects_) {
//			DebugText.Log ("TouchDirection: Dispatch Event= " + message);
//			DebugText.Log ("TouchDirection: Dispatch Event Direction = " + directionData_.horiDirection);
//			DebugText.Log ("TouchDirection: Dispatch Event Direction = " + directionData_.vertDirection);
			obj.SendMessage(message, directionData_, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private Vector3 GetTargetScreenPos() {
		if (refTarget == null) {
			shotRay = false;
			lastTargetScreenPos = new Vector3(renderCamera.pixelWidth/2, renderCamera.pixelHeight/2, 0f);
		} else if (!shotRay) {
			shotRay = true;
			lastTargetPos = refTarget.position;
			lastTargetScreenPos = renderCamera.WorldToScreenPoint(lastTargetPos);
//		} else if (EqualPosition(refTarget.position, lastTargetPos)) {	// TODO need check camera move too
			//lastTargetPos = refTarget.position;
			//return lastTargetScreenPos;
		} else {
			lastTargetPos = refTarget.position;
			lastTargetScreenPos = renderCamera.WorldToScreenPoint(lastTargetPos);
		}
		return lastTargetScreenPos;
	}
	
	private bool EqualPosition(Vector3 ref1Pos, Vector3 ref2Pos) {
		float tolerance = 0.01f;
		return (Mathf.Abs(ref1Pos.x - ref2Pos.x) < tolerance && 
				Mathf.Abs(ref1Pos.y - ref2Pos.y) < tolerance && 
				Mathf.Abs(ref1Pos.z - ref2Pos.z) < tolerance);
	}
	
	void OnDrawGizmos() {
		#if UNITY_EDITOR
		Gizmos.color = new Color(0, 0, 1, 1);
		Vector3 tempPos = GetTargetScreenPos();
		
		Vector3 screenPos1 = new Vector3(tempPos.x-horiCenterRange, tempPos.y-vertCenterRange, renderCamera.nearClipPlane);
		Vector3 screenPos2 = new Vector3(tempPos.x-horiCenterRange, tempPos.y+vertCenterRange, renderCamera.nearClipPlane);
		Vector3 screenPos3 = new Vector3(tempPos.x+horiCenterRange, tempPos.y+vertCenterRange, renderCamera.nearClipPlane);
		Vector3 screenPos4 = new Vector3(tempPos.x+horiCenterRange, tempPos.y-vertCenterRange, renderCamera.nearClipPlane);
		
		Vector3 pos1 = renderCamera.ScreenToWorldPoint(screenPos1);
		Vector3 pos2 = renderCamera.ScreenToWorldPoint(screenPos2);
		Vector3 pos3 = renderCamera.ScreenToWorldPoint(screenPos3);
		Vector3 pos4 = renderCamera.ScreenToWorldPoint(screenPos4);
		
		Vector3 tolPos1 = screenPos1;		
			tolPos1.x -= horiTolerance;	
			tolPos1.y -= vertTolerance;	
			tolPos1 = renderCamera.ScreenToWorldPoint(tolPos1);
		Vector3 tolPos2 = screenPos2;		
			tolPos2.x -= horiTolerance;	
			tolPos2.y += vertTolerance;
			tolPos2 = renderCamera.ScreenToWorldPoint(tolPos2);
		Vector3 tolPos3 = screenPos3;		
			tolPos3.x += horiTolerance;
			tolPos3.y += vertTolerance;	
			tolPos3 = renderCamera.ScreenToWorldPoint(tolPos3);
		Vector3 tolPos4 = screenPos4;		
			tolPos4.x += horiTolerance;
			tolPos4.y -= vertTolerance;	
			tolPos4 = renderCamera.ScreenToWorldPoint(tolPos4);
		
		if (renderCamera.orthographic) {
			float tolerance = 0.01f;
			if (Mathf.Abs(pos1.x - pos3.x) < tolerance) {
				pos1.x = lastTargetPos.x;
				pos2.x = lastTargetPos.x;
				pos3.x = lastTargetPos.x;
				pos4.x = lastTargetPos.x;
			} else if (Mathf.Abs(pos1.y - pos3.y) < tolerance) {
				pos1.y = lastTargetPos.y;
				pos2.y = lastTargetPos.y;
				pos3.y = lastTargetPos.y;
				pos4.y = lastTargetPos.y;
			} else if (Mathf.Abs(pos1.z - pos3.z) < tolerance) {
				pos1.z = lastTargetPos.z;
				pos2.z = lastTargetPos.z;
				pos3.z = lastTargetPos.z;
				pos4.z = lastTargetPos.z;
			}
		}
		
		// draw centerZone
		Gizmos.DrawLine(lastTargetPos, pos1);
		Gizmos.DrawLine(lastTargetPos, pos2);
		Gizmos.DrawLine(lastTargetPos, pos3);
		Gizmos.DrawLine(lastTargetPos, pos4);
		
		Gizmos.DrawLine(pos1, pos2);
		Gizmos.DrawLine(pos2, pos3);
		Gizmos.DrawLine(pos3, pos4);
		Gizmos.DrawLine(pos4, pos1);
		
		
		// draw ToleranceZone
		Gizmos.color = new Color(1, 0, 1, 1);
		Gizmos.DrawLine(tolPos1, tolPos2);
		Gizmos.DrawLine(tolPos2, tolPos3);
		Gizmos.DrawLine(tolPos3, tolPos4);
		Gizmos.DrawLine(tolPos4, tolPos1);
		#endif
	}
}
