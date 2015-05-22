using System.Collections;
using UnityEngine;

public class DirectionData {
	public enum TouchDir {
		Center,	/**< Center */
		Left,
		Right,
		Top,
		Bottom
	}
	
	public int touchId;	/**< Touch ID */
	public Vector2 screenPosition;	/**< Touch Screen Position */
	public TouchDir horiDirection = TouchDir.Center;
	public TouchDir vertDirection = TouchDir.Center;
	public Vector3 targetScreenPoint;
	public float horiCenterRange;
	public float vertCenterRange;

	/**
		\brief	Class constructor 
	*/
	public DirectionData() {
	}

	/**
		\brief	Class constructor
		\param[in]	id_	Touch ID
		\param[in]	screenPosition_	Touch screen position, [0,0] is the bottom left corner
		\param[in]	targetScreenPoint_	world target point to screen position
		\param[in]	horiCenterRange_	define the horizontal center range in pixel
		\param[in]	vertCenterRange_	define the vertical center range in pixel
	*/
	public DirectionData(int id_, Vector2 screenPosition_, Vector3 targetScreenPoint_, float horiCenterRange_, float vertCenterRange_) {
		SetData( id_, screenPosition_, targetScreenPoint_, horiCenterRange_, vertCenterRange_);
	}
	
	/**
		\brief	Initialize direction data
		\fn	void SetData(int id_, Vector2 screenPosition_, Vector3 targetScreenPoint_)
		\param[in]	id_	Touch ID
		\param[in]	screenPosition_	Touch screen position, [0,0] is the bottom left corner
		\param[in]	targetScreenPoint_	world target point to screen position
		\param[in]	horiCenterRange_	define the horizontal center range in pixel
		\param[in]	vertCenterRange_	define the vertical center range in pixel
	*/
	public void SetData(int id_, Vector2 screenPosition_, Vector3 targetScreenPoint_, float horiCenterRange_, float vertCenterRange_) {
		touchId = id_;
		screenPosition = screenPosition_;
		targetScreenPoint = targetScreenPoint_;
		SetDirection(horiCenterRange_, vertCenterRange_);
	}
	
	public void SetData(DirectionData data_) {
		touchId = data_.touchId;
		screenPosition = data_.screenPosition;
		targetScreenPoint = data_.targetScreenPoint;
		horiDirection = data_.horiDirection;
		vertDirection = data_.vertDirection;
		horiCenterRange = data_.horiCenterRange;
		vertCenterRange = data_.vertCenterRange;
	}
	
	/**
		\brief	Initialize the direction
		\fn	void SetDirection(float horiCenterRange_, float vertCenterRange_)
		\param[in]	horiCenterRange_	define the horizontal center range in pixel
		\param[in]	vertCenterRange_	define the vertical center range in pixel
	*/
	public void SetDirection(float horiCenterRange_, float vertCenterRange_) {
		horiCenterRange = horiCenterRange_;
		vertCenterRange = vertCenterRange_;
		float horiDiff = screenPosition.x - targetScreenPoint.x;
		if (Mathf.Abs(horiDiff) < horiCenterRange_) {
			horiDirection = TouchDir.Center;
		} else if (horiDiff < 0) {
			horiDirection = TouchDir.Left;
		} else {
			horiDirection = TouchDir.Right;
		}
		
		float vertDiff = screenPosition.y - targetScreenPoint.y;
		if (Mathf.Abs(vertDiff) < vertCenterRange_) {
			vertDirection = TouchDir.Center;
		} else if (vertDiff < 0) {
			vertDirection = TouchDir.Bottom;
		} else {
			vertDirection = TouchDir.Top;
		}
	}
	
	public bool InHoriTolerance(DirectionData data_, float horiTolerance) {
		float horiDiff = screenPosition.x - targetScreenPoint.x;
		if (data_.horiDirection == TouchDir.Center) {
			if (Mathf.Abs(horiDiff) < horiCenterRange + horiTolerance) {
				return true;
			} else {
				return false;
			}
		} else {
			if (Mathf.Abs(horiDiff) < horiCenterRange - horiTolerance) {
				return false;
			} else if (horiDiff < 0) {
				return (TouchDir.Left == data_.horiDirection);
			} else {
				return (TouchDir.Right == data_.horiDirection);
			}
		}
	}
	
	public bool InVertTolerance(DirectionData data_, float vertTolerance) {
		float vertDiff = screenPosition.y - targetScreenPoint.y;
		if (data_.vertDirection == TouchDir.Center) {
			if (Mathf.Abs(vertDiff) < vertCenterRange + vertTolerance) {
				return true;
			} else {
				return false;
			}
		} else {
			if (Mathf.Abs(vertDiff) < vertCenterRange - vertTolerance) {
				return false;
			} else if (vertDiff < 0) {
				return (TouchDir.Bottom == data_.horiDirection);
			} else {
				return (TouchDir.Top == data_.horiDirection);
			}
		}
	}
}