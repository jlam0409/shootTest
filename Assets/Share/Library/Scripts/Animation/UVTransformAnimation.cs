using UnityEngine;
using System.Collections;

/**
	\brief	Define the tranform attribute	(Animation Attribute)	
*/
public enum UVTransformType{
	//Translate,	/**< Translate */		
	//Rotate,		/**< Rotate */
	Scale			/**< Scale */		
}

// can further subdivide into vector3 and subclass transform
public class UVTransformAnimation : AnimationSystem<Vector2> {
	// for transform attributes
	public GameObject animObject;
	private Material animMaterial;
	public UVTransformType uvTransformType ;	/**< Define the TransformType (Animation Attribute) of the animation*/
		
	// assign delegates here
	private void Awake(){
		base.dGetCurrentPoint = new GetCurrentPointFunction (GetCurrentPointVector2);
		base.dGetDistance = new GetDistanceFunction(GetDistanceVector2);
		base.dCalculateMoveValue = new CalculateMoveValueFunction (CalculateMoveValueVector2);
		base.dApplyMoveValue = new ApplyMoveValueFunction (ApplyMoveValueVector2);
		base.dApplyEndValue = new ApplyEndValueFunction (ApplyEndValueVector2);
		base.dAccumulateEndValue = new AccumulateEndValueFunction (AccumulateEndValueVector2);
		
		InitData();
	}
	
	public void InitData(){
		// auto assignment if not assign by user
		if (animObject == null)		animObject = gameObject;
		
		animMaterial = (animObject.GetComponent<Renderer>() as Renderer).material;
	}
	
	// return tiling
	protected Vector2 GetCurrentPointVector2 (){
		return animMaterial.GetTextureScale("_MainTex");
	}

	protected float GetDistanceVector2 (Vector2 source, Vector2 target){
		return Vector2.Distance (source, target);	
	}
	
	protected Vector2 CalculateMoveValueVector2 ( float moveTime, EaseType ease){
		Vector2 newValue = new Vector2();
		switch (animationPath){
			default:
			case AnimationPath.Linear:	
				// calculate new value
				newValue = Vector2.Lerp(startPoint, endPoint,  Ease.GetTime ( moveTime/duration , ease));
				break;
		}
		return newValue;
	}
	
	protected void ApplyMoveValueVector2 (Vector2 newValue){	
		newValue = InvertSclaeValue(newValue);
		switch (uvTransformType){
			default:
			case UVTransformType.Scale:		
				Vector2 newOffset = GetOffset (newValue);
				animMaterial.SetTextureScale ("_MainTex", newValue);
				animMaterial.SetTextureOffset ("_MainTex", newOffset);
				break;
		}
	}
	
	protected void ApplyEndValueVector2 (Vector2 endValue){
		ApplyMoveValueVector2(endValue);
	}
	
	protected Vector2 AccumulateEndValueVector2  (Vector2 endPoint_, Vector2 startPoint_){
		Vector2 diff = endPoint_ - startPoint_;
		return (endPoint_ + diff);
	}
	
	private Vector2 GetOffset(Vector2 tiling){
		return new Vector2 ( (tiling.x-1)/-2f, (tiling.y-1)/-2f);
	}
	
	
	// this method is to invert the scale value, UV scale in computing is different from transform scale
	// invert the value can make tuning the value more intuitive and conform with transform scale
	private Vector2 InvertSclaeValue(Vector2 scaleValue){
		scaleValue.x = 1/scaleValue.x;
		scaleValue.y = 1/scaleValue.y;
		return scaleValue;
	}
}
