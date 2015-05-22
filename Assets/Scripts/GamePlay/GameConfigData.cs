using UnityEngine;
using System.Collections;

public class GameConfigData : MonoBehaviour {
	public static GameConfigData instance;
	public int timer;

	public float debrisInterval;
	public int debrisCount;
	public int debrisReward;

	public void Awake(){
		instance = this;
	}
}
