using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebrisEmitter : MonoBehaviour {
	public GameObject debrisPrefab;
	public Vector2 positionRange;
	public float endX; // fixed destination
	public List<Vector2> speedRangeList;

	public DebrisController EmitDebris(bool isBoss){
		int debrisType = Random.Range (0,speedRangeList.Count-1);
		int debrisReward = (debrisType+1) * GameConfigData.instance.debrisReward;


		int hp = 1;

		if (isBoss){
			hp = 10;
			debrisType = 2;
			debrisReward = 20;
		} else if (debrisType == 1){
			hp = 2;
		}

		Vector3 emittorPosition = gameObject.transform.localPosition;
		float startZ = Random.Range(positionRange.x, positionRange.y);
		Vector3 startPosition = new Vector3 (emittorPosition.x, emittorPosition.y, startZ);
		float speed = Random.Range(speedRangeList[debrisType].x, speedRangeList[debrisType].y);

		return EmitDebris (debrisType, debrisReward, startPosition, speed, hp);
	}

	public DebrisController EmitDebris(int debrisType, int debrisReward, Vector3 startPosition, float speed, int hp){
		Vector3 endPosition = new Vector3 (endX, startPosition.y, startPosition.z);
		GameObject debrisGO = Instantiate (debrisPrefab, startPosition, Quaternion.identity) as GameObject;

		DebrisController debrisController = debrisGO.GetComponent<DebrisController>();
		debrisController.Move(startPosition, endPosition, speed);
		debrisController.SetDebris(debrisType, debrisReward,hp);
		return debrisController;
	}
}
