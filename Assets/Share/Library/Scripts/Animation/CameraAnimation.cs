/**
Version:	1.0
Date:		05-05-2011
Author:		Amy
Description:
Control the camera zooming and moving.
*/

using UnityEngine;
using System.Collections;

/**
	\class	CameraZoom
 	\brief	CameraZoom class use to control the camera zooming and moving.
*/
public class CameraAnimation : MonoBehaviour {
	public Camera cam ; /**< Hold the reference of the main camera */
	public Transform camXform ;	/**< Hold the reference of the main camera's transform */
	public float cameraZoomDur = 1.0f;	/**< Default duration of the main camera's movement */
	public float cameraZoomScale = 0.7f;	/**< Default scale of the main camera's zooming */
	
	// Bound area
	public Vector3 initPos ;	/**< Main camera's init position */
	public float initOrthSize ;	/**< Main camera's init width */
	public float initOrthSizeY ;	/**< Main camera's init height */

	public float duration;	/**< Duration of the main camera's zooming and moving */
	public EaseType ease;	/**< Type of the EaseType */
	
	private float moveTime  = 0f;	/**< Current time of the main camera's zooming and moving */
	public bool isStop;	/**< If false, the main camera is zooming and moving */
	
	private float startOrthSize;	/**< Start value of orthographicSize movement*/
	private Vector3 targetPos;	/**< Target position of camera movement*/
	private float targetOrthSize ;	/**< Target value of orthographicSize movement*/
	
	private float startFOV;
	private float targetFOV;
	
	private Vector3 boundMin;	/**< Minimum value of scene bound */
	private Vector3 boundMax;	/**< Maximum value of scene bound */
	private Vector3 targetBoundMin;	/**< Temp minimum value of scene bound */
	private Vector3 targetBoundMax;	/**< Temp maximum value of scene bound */

	private TransformAnimation transformAnimation;
	public static CameraAnimation instance;	/**< Hold the reference of the CameraZoom instance */
	
	/**
		\fn	void	Awake()
		\brief	Awake function inherit from MonoBehaviour
	*/
	public void Awake(){
		instance = this;
	}
	
	/**
		\fn	void	Start()
		\brief	Start function inherit from MonoBehaviour
	*/
	public void Start(){
		ResetVar();
		
		initOrthSize = cam.orthographicSize;
		initOrthSizeY = initOrthSize / 2.4f * 4f;	// fit the screen with 800 pixel width and 480 height

		boundMin = new Vector3(initPos.x - initOrthSizeY, initPos.y - initOrthSize, initPos.z);
		boundMax = new Vector3(initPos.x + initOrthSizeY, initPos.y + initOrthSize, initPos.z);
		if(cam.orthographic)
			transformAnimation = cam.gameObject.AddComponent<TransformAnimation>() as TransformAnimation;
	}
	
	/**
		\fn	void	ResetVar()
		\brief	Init the variable
	*/
	public void ResetVar(){
		isStop = true;
		
		cam = Camera.main.camera;
		camXform = cam.transform;
		initPos = camXform.position;
	}
	
	/**
		\fn	void	Update()
		\brief	Update function inherit from MonoBehaviour
	*/
	public void Update () {
		if (cam == null){
			return;
		}
				
		if (isStop)
			return;
		
		if (moveTime <= duration ){	
			moveTime += Time.deltaTime;
			
			if(cam.orthographic)			
				cam.orthographicSize = Mathf.Lerp(startOrthSize, targetOrthSize, Ease.GetTime (moveTime/duration, ease));// Func.Ease ( moveTime/duration , ease));
			else{
				cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, Ease.GetTime (moveTime/duration, ease));// Time.deltaTime);
			}
			//objTranslate.Move();
		} else {
			isStop = true;
		}
	}
	
	public void StartZoom(float desiredFOV){
		isStop = false;
		startFOV = camera.fieldOfView;
		targetFOV = desiredFOV;
	}
	
	
	/**
		\fn	void	StartZoom(focusPos : Vector3, zoomScale : float, dur : float)
		\brief	Zoom to the target scale and moving the camera position to fit focusPos on the center of screen
		\param[in]	focusPos	Target position want to be the center of screen
		\param[in]	zoomScale	Target scale 
		\param[in]	dur	Duration of the camera movement
	*/
	public void StartZoom(Vector3 focusPos , float zoomScale , float dur ){
		startOrthSize = cam.orthographicSize;
		
		float targetOrthSizeY ;
		if (!isStop){
			targetOrthSize = targetOrthSize * zoomScale;
			targetOrthSizeY = targetOrthSize / 2.4f * 4f;	// fit the screen with 800 pixel width and 480 height
			duration = dur-moveTime;
			if (duration < 0.0f){
				duration = dur;
			}
			moveTime = 0.0f;
		} else{
			targetOrthSize = startOrthSize * zoomScale;
			targetOrthSizeY = targetOrthSize / 2.4f * 4f; // fit the screen with 800 pixel width and 480 height
			moveTime = 0.0f;
			duration = dur;
		}
		
		isStop = false;
		targetBoundMin = new Vector3(focusPos.x - targetOrthSizeY, focusPos.y - targetOrthSize, initPos.z);
		targetBoundMax = new Vector3(focusPos.x + targetOrthSizeY, focusPos.y + targetOrthSize, initPos.z);
		
		if (boundMin.x < targetBoundMin.x && boundMin.y < targetBoundMin.y && 
			boundMax.x > targetBoundMax.x && boundMax.y > targetBoundMax.y) {
			targetPos = focusPos;
		} else {
			if (targetOrthSize > initOrthSize){
				//DebugText.Log("can not large then init size");
				isStop = true;
				return;
			}
			targetPos = focusPos;
			if (boundMin.x > targetBoundMin.x){
				targetPos.x += (boundMin.x-targetBoundMin.x);
			}
			
			if (boundMin.y > targetBoundMin.y){
				targetPos.y += (boundMin.y-targetBoundMin.y);
			}
			
			if (boundMax.x < targetBoundMax.x){
				targetPos.x -= (targetBoundMax.x-boundMax.x);
			}
			
			if (boundMax.y < targetBoundMax.y){
				targetPos.y -= (targetBoundMax.y-boundMax.y);
			}
		}
		targetPos.z = initPos.z;
		transformAnimation.MoveToTime(targetPos, duration);
	}
	
	/**
		\fn	Vector3	GetCharPos(xform : Transform)
		\brief	Base on the specific transform to return the center offset value (obsolete)
		\param[in]	xform	Specific transform
		\return	Offseted position
	*/
	/*
	public Vector3 GetCharPos (Transform xform) {
		return xform.position - new Vector3(0f,-94f, 0f);
	}*/
}