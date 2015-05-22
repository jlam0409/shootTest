using UnityEngine;
using System.Collections;
using System;

// RC1
// Blends camera from origin to destination, static class to call
// Michael

public enum BlendType
{
	SPEED,	// a rate of speed
	TIME	// a time of duration
}
	

// user calls BlendTo, which adds a BlendCameraScript to the given camera.  
// BlendCameraScript uses monobehaviour Update() to animate, destroying itself (component) when destination is reached

public static class SimpleBlendCam //: MonoBehaviour 
{	
	public static void BlendFromTo(GameObject cam, GameObject start, GameObject destination, BlendType type, float timeValue){
		BlendFromTo(cam, start, destination, type, timeValue, null);
	}
	
	public static void BlendFromTo(GameObject cam, GameObject start, GameObject destination, BlendType type, float timeValue, Action callback){
		cam.transform.position = start.transform.position;
		BlendTo(cam, destination, type, timeValue, callback);
	}
	
	public static void BlendTo(GameObject cam, GameObject destination, BlendType type, float timeValue)
	{	
		if(cam)
			BlendTo(cam, destination, type, timeValue, null);	
	}
	public static void BlendTo(GameObject cam, GameObject destination, BlendType type, float timeValue, Action callback)
	{
		if(cam)
			BlendTo(cam, destination.transform.position, destination.transform.forward, type, timeValue, callback);		
	}
	public static void BlendTo(GameObject cam, Vector3 destination, Vector3 direction, BlendType type, float timeValue)
	{	
		if(cam)
			BlendTo(cam, destination, direction, type, timeValue, null);		
	}
	
	public static void BlendTo(GameObject cam, Vector3 destination, Vector3 direction, BlendType type, float timeValue, Action callback)
	{
		if(cam)
		{
			SimpleBlendCamScript camComponent = cam.GetComponent<SimpleBlendCamScript>() as SimpleBlendCamScript;
			if(!camComponent)		// isn't already blending, so add a blend
				camComponent = cam.AddComponent<SimpleBlendCamScript>();			
			
			camComponent.ChangeBlend(cam.transform.position, destination, cam.transform.forward, direction, type, timeValue, callback);
		}
	}
	
	public static void BlendFrom(GameObject cam, Vector3 start, Vector3 destination, Vector3 direction, BlendType type, float timeValue)
	{
		cam.transform.position = start;
		BlendTo(cam, destination, direction, type, timeValue);		
	}
	
	public static bool IsBlending(GameObject cam)
	{
		var camComponent = cam.GetComponent<SimpleBlendCamScript>();
		if(camComponent)
			return true;	// is blending
		
		return false;		// is not blending
	}
	
	public static void DestroyBlend(GameObject cam)
	{
		var camComponent = cam.GetComponent<SimpleBlendCamScript>();
		if(camComponent)
			Component.Destroy(camComponent);		
	}
}	
