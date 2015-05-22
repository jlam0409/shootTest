using UnityEngine;
using System;
using System.Collections;

public enum DebrisType {
	BASIC = 0,
	ARMOURED = 1,
	BOSS
}

public enum DebrisState{
	NONE,
	LIVE,
	DAMAGED,
	DEAD
}

public class DebrisController : MonoBehaviour {
	public TransformAnimation debrisAnimation;
	private DebrisType mType = DebrisType.BASIC;
	public CollisionDetector collisionDetector;
	public Renderer debrisRenderer;
	public Color damagedColor;

	private Action<DebrisController> mMissCallback;
	private Action<CollisionData> mDestroyCallback;
	private Action<CollisionData> mDestroyBulletCallback;
	public  int mHP;
	private int mReward;
	private float mSpeed;

	private DebrisState mState;

	public void SetState(DebrisState state){
		mState = state;
		if (state == DebrisState.DAMAGED){
			debrisRenderer.material.SetColor("color", damagedColor);
		}
	}

	public DebrisState GetState(){
		return mState;
	}

	public void SetDebris (int type, int reward, int hp){
		mType = (DebrisType)type;
		mReward = reward;
		mHP = hp;
		SetType(mType);
	}

	private void SetType(DebrisType type){
		if (type == DebrisType.BOSS){
			gameObject.transform.localScale = new Vector3 (3f,3f,3f);
		}
	}


	public void SetDebrisCallback(Action<CollisionData> destroyCallback, Action<CollisionData> destroyBulletCallback, Action<DebrisController> missCallback){
		debrisAnimation.callback.callbackAction = Missed;
		mMissCallback = missCallback;
		mDestroyCallback = destroyCallback;
		mDestroyBulletCallback = destroyBulletCallback;
//		collisionDetector.onCollisionCallback = destroyCallback;
		collisionDetector.onCollisionCallback = (data) => { Hit(data); } ;
	}

	public void Hit(CollisionData data){
		mHP -= 1;
		if (mHP == 0){
			if (GetState() == DebrisState.DEAD){
				return;
			} else {
//				Debug.Log ("HP = 0!");
				SetState (DebrisState.DEAD);
				mDestroyCallback(data);
			}
		} else {
			SetState (DebrisState.DAMAGED);
			mDestroyBulletCallback(data);
		}
	}

	public int GetReward(){
		return mReward;
	}

	public DebrisType GetDebrisType(){
		return mType;
	}

	public float GetSpeed(){
		return mSpeed;
	}

	public int GetHP(){
		return mHP;
	}

	public void Move(Vector3 from, Vector3 to, float speed){
		mSpeed = speed;
		debrisAnimation.MoveFromToSpeed(from, to, mSpeed);
	}

	public void Move(){
		debrisAnimation.ResumeMove();
	}

	public void Stop(){
		debrisAnimation.PauseMove();
	}

	public void Missed(){
		mMissCallback(this);
	}
}
