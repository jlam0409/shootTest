using System.Collections;
using UnityEngine;

public class TouchData {
	public int touchId;	/**< Touch ID */
	public Vector2 screenPosition;	/**< Touch Screen Position */
	public float rayDistance;
	private RaycastHit _hit;

	/**
		\brief	Class constructor 
	*/
	public TouchData ( ){
		
	}

	/**
		\brief	Class constructor
		\param[in]	id_	Touch ID
		\param[in]	screenPosition_	Touch screen position, [0,0] is the bottom left corner
		\param[in]	ray_	RaycastHit shoot from the touch position
	*/
	public TouchData( int id_, Vector2 screenPosition_ , RaycastHit hit_, float rayDistance_) {
		SetData( id_, screenPosition_, hit_, rayDistance_);
	}
	
	/**
		\brief	Initialize touch data
		\fn	void SetData ( int id_ , Vector2 screenPosition_, RaycastHit ray_ )
		\param[in]	id_	Touch ID
		\param[in]	screenPosition_	Touch screen position, [0,0] is the bottom left corner
		\param[in]	ray_	RaycastHit shoot from the touch position
	*/
	public void SetData ( int id_ , Vector2 screenPosition_, RaycastHit hit_, float rayDistance_){
		touchId = id_;
		screenPosition = screenPosition_;
		_hit = hit_;
		rayDistance = rayDistance_;
	}
	
	
	/**
		\brief	Get the hit world position from touch
		\fn	Vector3 GetWorldPosition ( )
		\return	Hit world position
	*/
	public Vector3 GetWorldPosition ( ){
		return (_hit.point);
	}
	
	/**
		\brief	Get the hit collider from touch
		\fn	Collider GetCollider ( )
		\return	Hit collider
	*/
	public Collider GetCollider () {
		return _hit.collider;
	}
}