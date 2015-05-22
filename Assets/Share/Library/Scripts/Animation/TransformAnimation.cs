using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
	\brief	Define the tranform attribute	(Animation Attribute)	
*/
public enum TransformType{
	Translate,	/**< Translate */		
	Rotate,		/**< Rotate */
	Scale			/**< Scale */		
}

// can further subdivide into vector3 and subclass transform
public class TransformAnimation : AnimationSystem<Vector3> {
	// for transform attributes
	public GameObject animObject;
	private Transform animXform;
	public TransformType transformType ;	/**< Define the TransformType (Animation Attribute) of the animation*/
	
	public GameObject startPointLocator;	/**< Reference of the start point game object, the start point of the system will use the gameobject's animation attribute */
	public GameObject endPointLocator;		/**< Reference of the end point game object, the end point of the system will use the gameobject's animation attribute */
	
	// assign delegates here
	private void Awake(){
		base.dGetCurrentPoint = new GetCurrentPointFunction (GetCurrentPointVector3);
		base.dGetDistance = new GetDistanceFunction(GetDistanceVector3);
		base.dCalculateMoveValue = new CalculateMoveValueFunction (CalculateMoveValueVector3);
		base.dApplyMoveValue = new ApplyMoveValueFunction (ApplyMoveValueVector3);
		base.dApplyEndValue = new ApplyEndValueFunction (ApplyEndValueVector3);
		base.dAccumulateEndValue = new AccumulateEndValueFunction (AccumulateEndValueVector3);
		base.dCalculateAcceleration = new CalculateAccelerationFunction (CalculateAccelerationVector3);
		base.dCalculateEndPoint = new CalculateEndPointFunction (CalculateEndPointVector3);

		InitData();
	}
	
	public void InitData(){
		if (animObject == null){
			animObject = gameObject;
		}
		
		animXform = animObject.transform;
		
		// set start end point
		if (startPointLocator != null){
			switch (transformType){
				case TransformType.Translate:	startPoint = startPointLocator.transform.localPosition;		break;
				case TransformType.Rotate:		startPoint = startPointLocator.transform.localEulerAngles;	break;
				case TransformType.Scale:		startPoint = startPointLocator.transform.localScale;		break;
			}
		}
		
		if (endPointLocator != null){
			switch (transformType){
				case TransformType.Translate:	endPoint = endPointLocator.transform.localPosition;		break;
				case TransformType.Rotate:		endPoint = endPointLocator.transform.localEulerAngles;	break;
				case TransformType.Scale:		endPoint = endPointLocator.transform.localScale;		break;
			}
		}
	}
	
	protected Vector3 GetCurrentPointVector3 (){
		Vector3 returnValue = Vector3.zero;
		switch (transformType){
			case TransformType.Translate:	returnValue = animXform.localPosition;		break;
			case TransformType.Rotate:		returnValue = animXform.localEulerAngles;	break;
			case TransformType.Scale:		returnValue = animXform.localScale;		break;
		}
		//return animXform.localPosition;
		return returnValue;
	}

	
	protected float GetDistanceVector3 (Vector3 source, Vector3 target){
		return Vector3.Distance (source, target);	
	}
	
	protected Vector3 CalculateMoveValueVector3 ( float moveTime, EaseType ease){
		Vector3 newValue = new Vector3();
		switch (animationPath){
			case AnimationPath.Linear:
				switch (calculationBase) {
					case CalculationBase.Point:
						newValue = Vector3.Lerp(startPoint, endPoint,  Ease.GetTime( moveTime/duration , ease));
						break;
					case CalculationBase.Velocity:
						newValue = startPoint + startVelocity * moveTime + acceleration * moveTime * moveTime / 2;
						break;
				}
				break;
			case AnimationPath.Sine:
				newValue = Vector3.Lerp(startPoint, endPoint,  Ease.GetTime( moveTime/duration , ease));
				
				newValue.x += Mathf.Sin(moveTime * frequency.x) * amplitude.x;
				newValue.y += Mathf.Sin(moveTime * frequency.y) * amplitude.y;
				newValue.z += Mathf.Sin(moveTime * frequency.z) * amplitude.z;					
				break;
		}
		return newValue;
	}
	
	protected void ApplyMoveValueVector3 (Vector3 newValue){
		switch (transformType){
			case TransformType.Translate:	animXform.localPosition = newValue;		break;
			case TransformType.Rotate:		animXform.localEulerAngles = newValue;	break;
			case TransformType.Scale:		animXform.localScale = newValue;			break;
		}
	}
	
	protected void ApplyEndValueVector3 (Vector3 endValue){
		ApplyMoveValueVector3(endValue);
	}
	
	protected Vector3 AccumulateEndValueVector3 (Vector3 endPoint_, Vector3 startPoint_){
		Vector3 diff = endPoint_ - startPoint_;
		return (endPoint_ + diff);
	}

	protected Vector3 CalculateAccelerationVector3 (Vector3 startVelocity_, Vector3 endVelocity_, float duration_) {
		Vector3 calAcceleration = (endVelocity_ - startVelocity_) / duration_;
		return calAcceleration;
	}

	protected Vector3 CalculateEndPointVector3 (Vector3 startPoint_, Vector3 startVelocity_, float duration_, Vector3 acceleration_){
		Vector3 calEndPoint = startPoint_ + (startVelocity_ * duration_) + acceleration_*duration_*duration_/2;
		return calEndPoint;
	}
	
	
}
