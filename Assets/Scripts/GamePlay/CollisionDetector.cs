using UnityEngine;
using System;
using System.Collections;

public struct CollisionData{
	public DebrisController debrisController;
	public BulletController bulletController;
}

public class CollisionDetector : MonoBehaviour {
	public Action<CollisionData> onCollisionCallback;
	
	// for rigidbody triggers
	public void OnCollisionEnter(Collision collision){
//		Debug.LogWarning ("OnCollisionEnter, trigger name: " + collision.gameObject.name);


//		onCollisionCallback.callbackAction1 (collisionData);
	}
	
//	public void OnCollisionStay(Collision collision){
////		Debug.LogWarning ("OnCollisionStay");
//	}
//	
	// for collider triggers, colliders must check "Is Trigger"
	public void OnTriggerEnter(Collider collider){
//		Debug.LogWarning ("OnTriggerEnter, collider name: " + collider.gameObject.name);

		BulletController bulletController = collider.gameObject.transform.parent.gameObject.GetComponent<BulletController>();
		DebrisController debrisController = gameObject.transform.parent.gameObject.GetComponent<DebrisController>();
		CollisionData collisionData = new CollisionData();
		collisionData.debrisController = debrisController;
		collisionData.bulletController = bulletController;

		onCollisionCallback(collisionData);
	}
	
//	public void OnTriggerStay (Collider collider){
//		Debug.LogWarning ("OnTriggerStay collider stays: " + collider.gameObject.name);
		//		onCollisionCallback.SendCallback();
//	}
}
