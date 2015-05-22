using UnityEngine;
using System.Collections;

public class BulletEmitter : MonoBehaviour {
	public GameObject bulletPrefab;
	private float startZ = -3.5f;
	private float endZ = 5f;

	public BulletController EmitBullet(float x){
		Vector3 emittorPosition = gameObject.transform.localPosition;
		Vector3 startPosition = new Vector3 (x, emittorPosition.y, startZ);
		return EmitBullet(startPosition);
	}

	public BulletController EmitBullet (Vector3 startPosition){
		Vector3 endPosition = new Vector3 (startPosition.x, startPosition.y, endZ);
		float speed = 2;
		
		GameObject bulletGO = Instantiate (bulletPrefab, startPosition, Quaternion.identity) as GameObject;
		
		BulletController controller = bulletGO.GetComponent<BulletController>();
		controller.Move(startPosition, endPosition, speed);
		return controller;
	}
}
