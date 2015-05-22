using UnityEngine;
using System.Collections;

// can further subdivide into float and subclass Alpha
public class AlphaAnimation : AnimationSystem<float> {
	public GameObject animObject;
	private Material animMaterial;
	
	private void Awake(){
		base.dGetCurrentPoint = new GetCurrentPointFunction (GetCurrentPointFloat);
		base.dGetDistance = new GetDistanceFunction(GetDistanceFloat);
		base.dCalculateMoveValue = new CalculateMoveValueFunction (CalculateMoveValueFloat);
		base.dApplyMoveValue = new ApplyMoveValueFunction (ApplyMoveValueFloat);
		base.dApplyEndValue = new ApplyEndValueFunction (ApplyEndValueFloat);
		base.dAccumulateEndValue = new AccumulateEndValueFunction (AccumulateEndValueFloat);
		
		InitData();
	}	
	
	public void InitData(){
		// auto assignment if not assign by user
		if (animObject == null)		animObject = gameObject;
		
		// assign private variables
		Renderer animRenderer = animObject.renderer;
		if (animRenderer != null)
			animMaterial = animRenderer.material;
//		animMaterial = (animObject.GetComponent<Renderer>() as Renderer).material;
	}
	
	protected float GetCurrentPointFloat (){
		if (animMaterial != null)
			return animMaterial.color.a;
		else 
			return 0f;
	}
	
	protected float GetDistanceFloat(float source, float target){
		return (target-source);
	}
	
	protected float CalculateMoveValueFloat ( float moveTime, EaseType ease){
		return Mathf.Lerp(startPoint, endPoint,  Ease.GetTime( moveTime/duration , ease));
	}
	
	protected void ApplyMoveValueFloat (float newValue){
		Color buffer = animMaterial.color;
		buffer.a = newValue;
		animMaterial.color = buffer;
	}
	
	protected void ApplyEndValueFloat (float endValue){
		ApplyMoveValueFloat (endValue);
	}
	
	protected float AccumulateEndValueFloat  (float endPoint_, float startPoint_){
		float diff = endPoint_ - startPoint_;
		return (endPoint_ + diff);
	}

}