using UnityEngine;
using System;
using System.Collections;

public class BulletController : MonoBehaviour {
	public TransformAnimation bulletAnimation;
	private Action<BulletController> mMissCallback;

	public void SetBulletCallback (Action<BulletController> missCallback){
		bulletAnimation.callback.callbackAction = Missed;
		mMissCallback = missCallback;
	}

	public void Move(Vector3 from, Vector3 to, float speed){
		bulletAnimation.MoveFromToSpeed(from, to, speed);
	}

	public void Move(){
		bulletAnimation.ResumeMove();
	}
	
	public void Stop(){
		bulletAnimation.PauseMove();
	}

	public void Missed(){
		mMissCallback(this);
	}
}
