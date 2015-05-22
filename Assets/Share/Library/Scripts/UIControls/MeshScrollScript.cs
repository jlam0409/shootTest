using UnityEngine;
using System.Collections;

public enum ScrollAxis{
	X,
	Y
}

public enum ScrollAlign{
	None,
	Min,
	Max
}

[RequireComponent (typeof (BoxCollider))]
public class MeshScrollScript : UIControl {	
	public ScrollAxis scrollAxis = ScrollAxis.X;
	public ScrollAlign scrollAlign = ScrollAlign.Min;
	public GameObject scrollObject; 
	private Transform _scrollXform = null;
	private TransformAnimation _scrollAnim;
	private float slideDuration = 0.2f;
	private float fastSlideDuration = 1.0f;
	private float fastSlideDistanceTolerance = 1.0f;
	public int dragTolerance = 5; // in screen pixel
	
	// scrollState
	public MeshScrollState scrollState;
	
	// for calculation of screen to world
	public Transform worldMinPositionLocator;		
	public Transform worldMaxPositionLocator;
	private Vector3 _worldMinPosition;
	private Vector3 _worldMaxPosition;
	private Vector2 _worldDistance;	
	
	// record the touch data
	private Vector2 _touchDistance;
	private int _downTouchId = -1;
	//private Vector2 _downTouchPosition;
	//private Vector3 _downPosition;
	private Vector2 _lastTouchPosition;
	private Vector2 _last2TouchPosition;
		
	// scroll boundaries data
	public Transform scrollMinPositionLocator;
	public Transform scrollMaxPositionLocator;	
	private Vector3 _scrollMinPosition;
	private Vector3 _scrollMaxPosition;
	private Vector2 _scrollDistance;
	
	// TODO: may try to query the bounding box later
	public Transform contentMinPositionLocator;
	public Transform contentMaxPositionLocator;
	private Vector3 _contentMinPosition;
	private Vector3 _contentMaxPosition;
	private Vector2 _contentDistance;
		
	private Vector3 _scrollXformMinPosition;		// storing the center pivot min position
	private Vector3 _scrollXformMaxPosition;		// storing the center pivot max position
	
	private bool _enableInitialAlignPosition = false;
	private Vector3 _scrollXformInitialAlignPosition;
//	private Vector3 _tempPosition;
	
	// store axis information
	public float _contentAxisDistance;
	public float _scrollAxisDistance;
	public float _contentMinAxisPosition;
	public float _contentMaxAxisPosition;
	public float _scrollMinAxisPosition;
	public float _scrollMaxAxisPosition;
	public float _scrollXformMinAxisPosition;
	public float _scrollXformMaxAxisPosition;
	public float currentXformAxisPosition;
	
	public bool initAtStart = false;
	private bool init = false;
	/*
	// store collider information
	private BoxCollider _collider;
	private Vector3 _colliderSize;
	//private Vector3 _colliderCenter;
	*/
	
	public void Awake(){
		_scrollXform = scrollObject.transform;
		_scrollAnim = scrollObject.GetComponent<TransformAnimation>() as TransformAnimation;
	}
	
	public void Start(){
		if (initAtStart)
			InitScroll();
	}
	
	public void InitScroll(){
		/*
		// cache collider information
		_collider = gameObject.GetComponent<BoxCollider>() as BoxCollider;
		_colliderSize = _collider.size;
		//_colliderCenter = _collider.center;
		*/
		_worldMinPosition = worldMinPositionLocator.position;
		_worldMaxPosition = worldMaxPositionLocator.position;
		_scrollMinPosition = scrollMinPositionLocator.position;
		_scrollMaxPosition = scrollMaxPositionLocator.position;
		_contentMinPosition = contentMinPositionLocator.position;
		_contentMaxPosition = contentMaxPositionLocator.position;
		
		_worldDistance.x = Mathf.Abs (_worldMaxPosition.x - _worldMinPosition.x);		
		_worldDistance.y = Mathf.Abs (_worldMaxPosition.z - _worldMinPosition.z);
		
		_scrollDistance.x = Mathf.Abs (_scrollMaxPosition.x - _scrollMinPosition.x);
		_scrollDistance.y = Mathf.Abs (_scrollMaxPosition.z - _scrollMinPosition.z);
		
		_contentDistance.x = Mathf.Abs (_contentMaxPosition.x - _contentMinPosition.x);
		_contentDistance.y = Mathf.Abs (_contentMaxPosition.z - _contentMinPosition.z);
		
		//_scrollXformMinPosition = _scrollMinPosition + (scrollObject.transform.position - _contentMinPosition);
		//_scrollXformMaxPosition = _scrollMaxPosition - (_contentMaxPosition - scrollObject.transform.position);
		/*
		DebugText.Log ("locator: " + scrollMaxPositionLocator.position);
		DebugText.Log ("_scrollMinPosition:" +_scrollMinPosition + "\t_scrollMaxPosition:" +_scrollMaxPosition);
		DebugText.Log ("_contentMinPosition:" + _contentMinPosition + "\t_contentMaxPosition:" + _contentMaxPosition);
		DebugText.Log (_scrollMinPosition - _contentMinPosition);
		*/
		_scrollXformMinPosition = _scrollXform.localPosition + (_scrollMinPosition - _contentMinPosition);
		_scrollXformMaxPosition = _scrollXform.localPosition + (_scrollMaxPosition - _contentMaxPosition);
		
		//_tempPosition = _scrollXform.position;
		
		// store axis information at Awake instead of query at update
		switch (scrollAxis){
			default:
			case ScrollAxis.X:	
				_contentMinAxisPosition = _contentMinPosition.x;
				_contentMaxAxisPosition = _contentMaxPosition.x;
				_scrollMinAxisPosition = _scrollMinPosition.x;
				_scrollMaxAxisPosition = _scrollMaxPosition.x;
				_scrollXformMinAxisPosition = _scrollXformMinPosition.x;
				_scrollXformMaxAxisPosition = _scrollXformMaxPosition.x;
			
				_scrollXformMinPosition = new Vector3 (_scrollXformMinPosition.x, _scrollXform.localPosition.y, _scrollXform.localPosition.z);
				_scrollXformMaxPosition = new Vector3 (_scrollXformMaxPosition.x, _scrollXform.localPosition.y, _scrollXform.localPosition.z);
			break;
			case ScrollAxis.Y:	
				_contentMinAxisPosition = _contentMinPosition.z;
				_contentMaxAxisPosition = _contentMaxPosition.z;
				_scrollMinAxisPosition = _scrollMinPosition.z;
				_scrollMaxAxisPosition = _scrollMaxPosition.z;
				_scrollXformMinAxisPosition = _scrollXformMinPosition.z;
				_scrollXformMaxAxisPosition = _scrollXformMaxPosition.z;
			
				_scrollXformMinPosition = new Vector3 (_scrollXform.localPosition.x, _scrollXform.localPosition.y, _scrollXformMinPosition.z);
				_scrollXformMaxPosition = new Vector3 (_scrollXform.localPosition.x, _scrollXform.localPosition.y, _scrollXformMaxPosition.z);
			break;
		}

		//DebugText.Log ("_scrollXformMinPosition" + _scrollXformMinPosition);
		//DebugText.Log ("_scrollXformMaxPosition" + _scrollXformMaxPosition);
		_contentAxisDistance = _contentMaxAxisPosition - _contentMinAxisPosition;
		_scrollAxisDistance = _scrollMaxAxisPosition - _scrollMinAxisPosition;
		
		if (_enableInitialAlignPosition){
			AlignInitialPosition();
		} else {
			StartAlign();
		}
		init = true;
	}
	
	public void ResetScroll(){
		//contentMinPositionLocator.position = _contentMinPosition;
		//contentMaxPositionLocator.position = _contentMaxPosition;
		switch (scrollAlign){			
			case ScrollAlign.Max:
				scrollObject.transform.localPosition = _scrollXformMaxPosition;
				break;
			case ScrollAlign.Min:
				scrollObject.transform.localPosition = _scrollXformMinPosition;
				break;
		}
	}
	
	public void StartAlign(){
		if (_contentAxisDistance <= _scrollAxisDistance){
			switch (scrollAlign){			
				case ScrollAlign.Max:
					scrollObject.transform.localPosition = _scrollXformMaxPosition;
					break;
				case ScrollAlign.Min:
					scrollObject.transform.localPosition = _scrollXformMinPosition;
					break;
				default:	break;
			}
		}
	}
	
	public void SetInitialAlign( Vector3 pos){
		switch (scrollAxis){
			default:
			case ScrollAxis.X:	
				_scrollXformInitialAlignPosition = new Vector3 (_scrollXformMaxPosition.x - pos.x, _scrollXform.localPosition.y, _scrollXform.localPosition.z );
				break;
			case ScrollAxis.Y:
				_scrollXformInitialAlignPosition = new Vector3 (_scrollXform.localPosition.x, _scrollXform.localPosition.y, _scrollXformMaxPosition.z - pos.z );
				break;
		}

		_enableInitialAlignPosition = true;
	}
	
	
	public void AlignInitialPosition(){
		scrollObject.transform.localPosition = _scrollXformInitialAlignPosition;
		SetState (MeshScrollState.FinishMove);	
	}
	
	public void Update(){
		if (!init)
			return;
		
		// update content min max position
		switch (scrollAxis){
			case ScrollAxis.Y:	
				currentXformAxisPosition = _scrollXform.localPosition.z;	
				_contentMinAxisPosition = contentMinPositionLocator.position.z;
				_contentMaxAxisPosition = contentMaxPositionLocator.position.z;
				break;
			case ScrollAxis.X:	
			default:
				currentXformAxisPosition = _scrollXform.localPosition.x;	
				_contentMinAxisPosition = contentMinPositionLocator.position.x;
				_contentMaxAxisPosition = contentMaxPositionLocator.position.x;
				break;
		}
		
		// start parse state
		switch (scrollState){
			case MeshScrollState.Move:
			case MeshScrollState.StartMove:
				_contentMinPosition = contentMinPositionLocator.position;
				_contentMaxPosition = contentMaxPositionLocator.position;
			
				if (_scrollAnim.animState != AnimationStates.Finished){
					_scrollAnim.StopMove ();
				}
				break;
			case MeshScrollState.FastSlide:	
				if (currentXformAxisPosition > _scrollXformMinAxisPosition || currentXformAxisPosition < _scrollXformMaxAxisPosition){
					scrollState = MeshScrollState.FinishMove;
				}
				break;
			case MeshScrollState.FinishMove:
				// two cases to handle
				// Case 1: content width  > scroll width
				//DebugText.Log ("Case 1: Slide to "+ scrollAlign  +"called");
				if (_contentAxisDistance > _scrollAxisDistance){
					if (currentXformAxisPosition > _scrollXformMinAxisPosition){
						//DebugText.Log ("Case 1: Slide to min called");
						SlideTo(ScrollAlign.Min);
					} else if (currentXformAxisPosition < _scrollXformMaxAxisPosition){
						SlideTo(ScrollAlign.Max);
					} else {
						SetState (MeshScrollState.Static);	
					}
				} else {	// Case 2: content width  <= scroll width
					//DebugText.Log ("Case 2: Slide to " + scrollAlign  +" called");
					switch (scrollAlign){
						case ScrollAlign.Max:
							if (currentXformAxisPosition != _scrollXformMaxAxisPosition){
								//DebugText.Log ("currentXformAxisPosition != _scrollXformMaxAxisPosition, should slide to max");	
								SlideTo(ScrollAlign.Max);
							} else{
								//DebugText.Log ("currentXformAxisPosition == _scrollXformMaxAxisPosition, should stop");	
								SetState (MeshScrollState.Static);	
							}
							break;
						default:
						case ScrollAlign.Min:
							if (currentXformAxisPosition != _scrollXformMinAxisPosition)
								SlideTo(ScrollAlign.Min);
							else
								SetState (MeshScrollState.Static);	
							break;
					}
				}
				break;
			default:
				break;
		}
		
		/*
		if (scrollState	== MeshScrollState.Move || scrollState == MeshScrollState.StartMove){
			_contentMinPosition = contentMinPositionLocator.position;
			_contentMaxPosition = contentMaxPositionLocator.position;
			
			if (!_scrollAnim.ObjectFinishedMove()){
				_scrollAnim.StopMove ();
			}
		} else if (scrollState == MeshScrollState.FastSlide){
			if (_scrollXform.position.x > _scrollXformMinPosition.x || _scrollXform.position.x < _scrollXformMaxPosition.x){
				scrollState = MeshScrollState.FinishMove;
			}
		} else if (scrollState == MeshScrollState.FinishMove){
			if (_scrollXform.position.x > _scrollXformMinPosition.x){
				SlideTo("min");
			} else if (_scrollXform.position.x < _scrollXformMaxPosition.x){
				SlideTo("max");
			} else {
				SetState (MeshScrollState.Static);	
			}
		}
		*/
		
		//_tempPosition = _scrollXform.position;
	}
	 
	
	public void OnTouchDown(TouchData touchData_){
		//DebugText.Log ("HorizontalScrollScript: OnTouchDown called, downPosition:" + touchData_.screenPosition);
		SetState (MeshScrollState.StartMove);
			
		if (_downTouchId < 0){
			_downTouchId = touchData_.touchId;
			
			_lastTouchPosition = touchData_.screenPosition;
			_last2TouchPosition = touchData_.screenPosition;
			
			//_downTouchPosition = touchData_.screenPosition;
			//_downPosition = _scrollXform.position;
		}
	}
	
	public void OnTouchDrag(TouchData touchData_){
		//DebugText.Log ("HorizontalScrollScript: OnTouchDrag called, touchPosition: " + touchData_.screenPosition);
		if (touchData_.touchId != _downTouchId )
			return;
		
		/*
		_touchDistance.x = touchData_.screenPosition.x - _lastTouchPosition.x;
		_touchDistance.y = touchData_.screenPosition.y - _lastTouchPosition.y;
		
		float worldTravel;
		switch (scrollAxis){
			case ScrollAxis.Y:
				if ( Mathf.Abs(_touchDistance.y) < dragTolerance)	
					return;
				worldTravel = _touchDistance.y / Screen.height * _worldDistance.z;
			break;
			default:
			case ScrollAxis.X:
				if ( Mathf.Abs(_touchDistance.x) < dragTolerance)	
					return;	
				worldTravel = _touchDistance.x / Screen.width * _worldDistance.x;
			break;
		}
		*/
		float worldTravel = 0f;
		float touchDistance = 0f;
		if ( !GetWorldTravel (touchData_,_lastTouchPosition, ref worldTravel, ref touchDistance) ){
			return;
		}
		//DebugText.Log ("worldTravel:" + worldTravel + "touchDistance:" + touchDistance);
		SetState (MeshScrollState.Move);

		Vector3 newPosition = _scrollXform.position;		
		float newAxisPosition = 0f;
		
		// one behaviour of the scroll view is check if the content is already exceed the boundary, drag distance decrease half
		// calculate the drag distance
		if (_contentMinAxisPosition > _scrollMinAxisPosition){
			//float positionDifference = Mathf.Abs (_contentMinPosition.x - _scrollMinAxisPosition);
			float positionDifference = Mathf.Abs (_contentMinAxisPosition - _scrollMinAxisPosition);
			float newWorldTravel = worldTravel/(1f+positionDifference*2);
			newAxisPosition = newWorldTravel;
		} else if (_contentMaxAxisPosition < _scrollMaxAxisPosition){
			//float positionDifference = Mathf.Abs (_scrollMaxAxisPosition - _contentMaxPosition.x);
			float positionDifference = Mathf.Abs (_scrollMaxAxisPosition - _contentMaxAxisPosition);
			float newWorldTravel = worldTravel/(1f+positionDifference*2);
			newAxisPosition= newWorldTravel;
		} else {
			newAxisPosition = worldTravel;
		}
		
		newAxisPosition = worldTravel;
		switch (scrollAxis){
			case ScrollAxis.Y:		newPosition.z += newAxisPosition;	break;
			default:
			case ScrollAxis.X:		newPosition.x += newAxisPosition;	break;
		}
		
		/*
		DebugText.Log ("=======================Start checking========================");
		DebugText.Log ("_contentMinAxisPosition: " + _contentMinAxisPosition  + "_contentMinPosition.x:" + _contentMinPosition.x);
		DebugText.Log ("_contentMaxAxisPosition: " + _contentMaxAxisPosition  + "_contentMaxPosition.x:" + _contentMaxPosition.x);
		DebugText.Log ("_scrollMinAxisPosition: " + _scrollMinAxisPosition + "_scrollMinPosition.x:" + _scrollMinPosition.x);
		DebugText.Log ("_scrollMaxAxisPosition: " + _scrollMaxAxisPosition + "_scrollMaxPosition.x:" + _scrollMaxPosition.x);
		DebugText.Log ("=======================Finished checking========================");
		*/
		
		/*
		if (_contentMinPosition.x > _scrollMinPosition.x){
			float positionDifference = Mathf.Abs (_contentMinPosition.x - _scrollMinPosition.x);
			float newWorldTravel = worldTravel/(1f+positionDifference*2);
			newPosition.x += newWorldTravel;
		} else if (_contentMaxPosition.x < _scrollMaxPosition.x){
			float positionDifference = Mathf.Abs (_scrollMaxPosition.x - _contentMaxPosition.x);
			float newWorldTravel = worldTravel/(1f+positionDifference*2);
			newPosition.x += newWorldTravel;
		} else {
			newPosition.x += worldTravel;
		}
		*/
		
		//DebugText.Log ("OnTouchDrag: dragVelocity=" + ((newPosition-_scrollXform.position) / Time.deltaTime) + "\tcurrentPosition: " + _scrollXform.position + "\tnewPosition" + newPosition);
		_scrollXform.position = newPosition;
		
		// cache last touch position
		_last2TouchPosition = _lastTouchPosition;
		_lastTouchPosition = touchData_.screenPosition;
		
	}
		
	public void OnTouchHold(TouchData touchData_){
		//DebugText.Log ("HorizontalScrollScript: OnTouchHold called, reset key position");
		_last2TouchPosition = _lastTouchPosition;
		_lastTouchPosition = touchData_.screenPosition;
	}
	
	public void OnTouchUp (TouchData touchData_){
		//DebugText.Log ("HorizontalScrollScript: OnTouchUp called");
		OnTouchDragUp(touchData_);
	}
	
	public void OnTouchLeaveUp (TouchData touchData_){
		OnTouchDragUp(touchData_);	
	}

	public void OnTouchDragUp (TouchData touchData_){
		//DebugText.Log ("MeshScrollScript - OnTouchDragUp received");
		if (touchData_.touchId != _downTouchId )
			return;
		
		_downTouchId = -1;
		//_downTouchPosition = Vector2.zero;
		/*
		float touchDistance = touchData_.screenPosition.x - _lastTouchPosition.x;		
		float worldTravel = _touchDistance.x / Screen.width * _worldDistance.x;
		*/
		//DebugText.Log ("OnTouchDragUp called");
		float touchDistance = 0f;
		float worldTravel = 0f;
		if ( !GetWorldTravel (touchData_, _lastTouchPosition, ref touchDistance, ref worldTravel)){
			SetState (MeshScrollState.FinishMove);
			return;	
		}
		
		//DebugText.Log ("_scrollXform is null? " + (_scrollXform == null));
		Vector3 newPosition = _scrollXform.localPosition;
		
		switch (scrollAxis){
			case ScrollAxis.Y:
				newPosition.z += worldTravel;
				break;
			default:
			case ScrollAxis.X:
				newPosition.x += worldTravel;
				break;
		}
		
		//DebugText.Log ("OnTouchDragUp: dragVelocity1=" + ((newPosition-_scrollXform.position) / Time.deltaTime));
		
		float dragVelocity = worldTravel / Time.deltaTime;
		// DebugText.Log ("OnTouchDragUp: dragVelocity1.5=" + dragVelocity);
		// DebugText.Log ("HorizontalScrollScript: OnTouchDragUp called, calculate key position: " + touchDistance + "\ttouchPosition: " + touchData_.screenPosition);
		// we discover that the touchData is always the same as the touchDrag data
		// so we go to try compare if it is the same, if yes, we go further check the last2 touchData		
		if (Mathf.Abs (touchDistance) < 10){
			newPosition = _scrollXform.localPosition;
			
			switch (scrollAxis){
				case ScrollAxis.Y:
					touchDistance = touchData_.screenPosition.y - _last2TouchPosition.y;
					worldTravel = _touchDistance.y / Screen.height * _worldDistance.y;
					newPosition.z += worldTravel;
					break;
				default:
				case ScrollAxis.X:
					touchDistance = touchData_.screenPosition.x - _last2TouchPosition.x;
					worldTravel = _touchDistance.x / Screen.width * _worldDistance.x;
					newPosition.x += worldTravel;
					break;
			}
			 
			//GetWorldTravel (touchData_, _last2TouchPosition, ref touchDistance, ref worldTravel);
			/*
			if ( !GetWorldTravel (touchData_, _last2TouchPosition, ref touchDistance, ref worldTravel)){
				return;	
			}
			*/
			dragVelocity = worldTravel / (Time.deltaTime);
			if (Mathf.Abs (touchDistance) > fastSlideDistanceTolerance ){
				//DebugText.Log ("OnTouchDragUp: dragVelocity2= " + dragVelocity + "\tnewPosition:" + newPosition);
				//_scrollXform.position = newPosition;
				_scrollXform.localPosition = newPosition;
				SlideFastTo (newPosition, dragVelocity);
			} else {
				SetState (MeshScrollState.FinishMove);
			}
		} else{
			//DebugText.Log ("OnTouchDragUp: dragVelocity2= " + dragVelocity + "\tnewPosition:" + newPosition);
			_scrollXform.localPosition = newPosition;
			SlideFastTo (newPosition, dragVelocity);
		}
	}
	
	public bool GetWorldTravel ( TouchData touchData_, Vector2 lastTouchPosition_, ref float worldTravel, ref float touchDistance){
		//_touchDistance.x = touchData_.screenPosition.x - lastTouchPosition_.x;
		//_touchDistance.y = touchData_.screenPosition.y - lastTouchPosition_.y;
		
		_touchDistance.x = touchData_.screenPosition.x - _lastTouchPosition.x;
		_touchDistance.y = touchData_.screenPosition.y - _lastTouchPosition.y;
		
		//float worldTravel;
		switch (scrollAxis){
			case ScrollAxis.Y:
				if ( Mathf.Abs(_touchDistance.y) < dragTolerance){
					return false;
				}
				worldTravel = _touchDistance.y / Screen.height * _worldDistance.y;
				touchDistance = _touchDistance.y;
				break;
			default:
			case ScrollAxis.X:
				if ( Mathf.Abs(_touchDistance.x) < dragTolerance){
					return false;
				}
				worldTravel = _touchDistance.x / Screen.width * _worldDistance.x;
				touchDistance = _touchDistance.x;
				break;
		}
		
		//return worldTravel;
		return true;
	}
	
	public void SetState(MeshScrollState state_){
		scrollState = state_;
	}
	
	public void SlideTo ( ScrollAlign align ){
		SetState(MeshScrollState.Slide);
		switch (align){
			case ScrollAlign.Min:				
				_scrollAnim.MoveFromToTime (_scrollXform.localPosition, _scrollXformMinPosition, slideDuration);		
				break;
			case ScrollAlign.Max:
				_scrollAnim.MoveFromToTime (_scrollXform.localPosition, _scrollXformMaxPosition, slideDuration);
				break;
		}
		_scrollAnim.StartMove();
	}
	
	public void SlideFastTo (Vector3 newPosition, float dragVelocity_){
		SetState (MeshScrollState.FastSlide);
		Vector3 dragVelocity = Vector3.zero;
		switch (scrollAxis){
			case ScrollAxis.Y:	
				dragVelocity.z = dragVelocity_;
				break;
			default:
			case ScrollAxis.X:
				dragVelocity.x = dragVelocity_;
				break;
		}
		
		Vector3 endVelocity = Vector3.zero;
		
		_scrollAnim.MoveFromToVelocity (newPosition, dragVelocity, endVelocity, fastSlideDuration);
		_scrollAnim.StartMove();
	}
	
	public void AnimationSystemCallback(){
		//DebugText.Log ("AnimationSystemCallback");
		if (scrollState == MeshScrollState.FastSlide){
			SetState (MeshScrollState.FinishMove);
		} else {
			SetState (MeshScrollState.Static);
		}
	}
	/*
	public void EnableCollider(){
		_collider.size = _colliderSize;
	}
	
	public void DisableCollider(){
		_collider.size = Vector3.zero;
	}*/
}
