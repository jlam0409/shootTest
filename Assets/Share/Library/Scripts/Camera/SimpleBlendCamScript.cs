using UnityEngine;
using System.Collections;
using System;					// for Math.Round

// RC1
// Blends camera from origin to destination, the script thats added to camera
// Michael


// \note Mathf.Approximately may have trouble on iOS devices (http://forum.unity3d.com/threads/61102-Mathf.Approximately-not-working-correctly-on-iPhone.)

public class SimpleBlendCamScript : MonoBehaviour {
	
	private Vector3 startPosition;
	private Vector3 endPosition;
	private Vector3 startDirection;
	private Vector3 endDirection;
	
	private BlendType blendBy; 
	private float rate = 0.0f, duration = 0.0f;		// just to be EXPLICIT use two variables
	private float moveTime = 0f;
	
	Action callbackMethod = null;
	
	// Properties (is null?)
	public float timeValue
	{
		get	{	return rate;	}
		set	{	rate = duration = value;	}
	}
	public Vector3 destination
	{
		get	{	return endPosition;		}
		set	{	endPosition = value;	}		
	}
	public Vector3 destinationDirection
	{
		get	{	return endDirection;	}
		set	{	endDirection = value;	}
	}
	public BlendType blendType
	{
		get	{	return blendBy;		}	
		set	{	blendBy = value;	}
	}
	
	
	
	// Modification Functions
	public void ChangeBlend(SimpleBlendCamScript newSettings)
	{
		moveTime = 0f;
		endPosition = newSettings.destination;
		endDirection = newSettings.destinationDirection;
		rate = duration = newSettings.timeValue;
		blendBy = newSettings.blendType;
	}
	
	public void ChangeBlend(Vector3 endPos, Vector3 endDir)
	{
		moveTime = 0f;
		endPosition = endPos;
		endDirection = endDir;
	}
	
	public void ChangeBlend(BlendType blendby, float time)
	{	
		moveTime = 0f;
//		blendBy = blendby;
//		rate = duration = time;
		duration = time;
	}
	public void ChangeBlend(float time)
	{	
		moveTime = 0f;
//		rate = duration = time;
		duration = time;
	}
	public void ChangeBlend(Vector3 startPos, Vector3 endPos, Vector3 startDir, Vector3 endDir, BlendType blendby, float time)
	{
		moveTime = 0f;
		startPosition = startPos;
		startDirection = startDir;
		endPosition = endPos;
		endDirection = endDir;
		blendBy = blendby;
		SetDuration(blendby, time, startPos, endPos);
		callbackMethod = null;
//		rate = duration = time;
	}
	public void ChangeBlend(Vector3 startPos, Vector3 endPos, Vector3 startDir, Vector3 endDir, BlendType blendby, float time, Action callback)
	{
		ChangeBlend(startPos, endPos, startDir, endDir, blendby, time);
		callbackMethod = callback;
	}
	
	
	
	private void SetDuration(BlendType type, float time, Vector3 startPoint, Vector3 endPoint){
		if (blendBy==BlendType.SPEED)
			duration = SpeedToDuration(startPoint, endPoint, time);
		else 
			duration = time;
	}
	// Update is called once per frame
	void Update () {
	
		// gets distance between positions, rounds it to two places and then tests against approximation to 0.00f
		// if(transform.position != endPosition)...will never get to exact endPosition
//		if(!Mathf.Approximately((float)Math.Round(Vector3.Distance(transform.position, endPosition), 2), 0.00f)){	// could also compare 'Magnitude' or x/y/z	
		if (moveTime < duration ){
			//Debug.Log("UPDATING [" + (float)Math.Round(Vector3.Distance(transform.position, endPosition), 2) + "] : " + transform.position);
			moveTime += Time.deltaTime;
//			switch(blendBy)
//			{
//				// by speed
//				case BlendType.SPEED	:	
//					transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * rate);
//					transform.forward = Vector3.Lerp(transform.forward, endDirection, Time.deltaTime * rate);
//					break;
//				// by duration
//				case BlendType.TIME	:	
//					transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime / duration);
//					transform.forward = Vector3.Lerp(transform.forward, endDirection, Time.deltaTime / duration);	
					transform.position = Vector3.Lerp(startPosition, endPosition, Ease.GetTime( moveTime/duration, EaseType.None));
					transform.forward = Vector3.Lerp(startDirection, endDirection, Ease.GetTime( moveTime/duration, EaseType.None));	
//					break;
//			}
		}
		else
		{	if(callbackMethod != null)
				callbackMethod();			
						
			Destroy(GetComponent<SimpleBlendCamScript>());		// finished so remove camera blend component
		}	
		
	}
	
	protected float SpeedToDuration ( Vector3 source , Vector3 destination, float speed){
		float dis = GetDistanceVector3 (source, destination);
		return dis / speed ;
	}
	
	protected float GetDistanceVector3 (Vector3 source, Vector3 target){
		return Vector3.Distance (source, target);	
	}
	
}
