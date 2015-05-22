using UnityEngine;
using System.Collections;

public class CameraMaskScript : MonoBehaviour {
	public Transform maskObject;
	
	private float cameraSize;	// calculate the camera size: maskMesh.scaleZ /2
	private Vector2 normalizedViewPortPosition;
	private Vector2 normalizedViewPortScale;
	
	private Vector3 worldMinPosition = new Vector3 (-4f, 0f, -2.4f);
	private Vector3 worldMaxPosition = new Vector3 (4f, 0f, 2.4f);
	private Vector3 worldDistance;
	
	private float worldScaleFactor = 1000f;	// i.e. when the resolution is 800 * 480, a mesh with 0.8 * 0.48 can fit the screen, the scale factor is 1/100
	private Vector2 resolution = new Vector2 (800f, 480f);
	private Vector3 worldScale;
	public void Awake(){		
		CalculateWorldValue();
		AdjustCameraTransformValue();
		AdjustCameraValue();
	}
	
	public void CalculateWorldValue(){
		worldDistance = new Vector3 (worldMaxPosition.x - worldMinPosition.x, worldMaxPosition.y - worldMinPosition.y, worldMaxPosition.z - worldMinPosition.z);
		worldScale = new Vector3 (resolution.x / worldScaleFactor, 0f, resolution.y / worldScaleFactor);
	}
	
	public void CalculateValue(){
		cameraSize = maskObject.lossyScale.z / 0.2f;	
		
		float normalizedViewPortPositionX = (maskObject.localPosition.x - worldMinPosition.x) / worldDistance.x;
		float normalizedViewPortPositionY = (maskObject.localPosition.z - worldMinPosition.z) / worldDistance.z;
		float normalizedViewPortWidth = maskObject.lossyScale.x / worldScale.x;
		float normalizedViewPortHeight = maskObject.lossyScale.z / worldScale.z;
		
		normalizedViewPortPosition = new Vector2 (normalizedViewPortPositionX, normalizedViewPortPositionY);
		normalizedViewPortScale = new Vector2 (normalizedViewPortWidth, normalizedViewPortHeight);
	}
	
	public void ApplyCameraValue (){
		camera.orthographicSize = cameraSize;
		camera.rect  = new Rect (normalizedViewPortPosition.x, normalizedViewPortPosition.y, normalizedViewPortScale.x, normalizedViewPortScale.y);
	}
	
	public void AdjustCameraValue(){
		CalculateValue();
		ApplyCameraValue();	
		AdjustCameraTransformValue();
	}
	
	public void AdjustCameraTransformValue(){
		Vector3 center = maskObject.gameObject.renderer.bounds.center;
		center.y = 4f;
		this.transform.position = center;
	}
	
	/*
	public void Update(){
		AdjustCameraValue();	
	}*/
}
