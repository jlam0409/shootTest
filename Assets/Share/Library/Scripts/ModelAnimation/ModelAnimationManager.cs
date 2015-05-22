using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelAnimationManager {
	public string defaultAnimName;	/**< Define the animation name which will play automatically when finished a "Once" animation*/
	private float defaultAnimSpeed = 1.0f;		/**< Default value of the animation speed, if no specific when play animation */	
	private bool isAnimationPlaying = false;	/**< If true, the model is playing animation */
	public Animation anim;						/**< Hold the reference of the Animation which is set by ModelControl automatically*/
	public string currectAnimName = "";		/**< Name of the current animation */
	private float animBlendHoldSec = 0.15f;			/**< Time in seconds of blend the animation by calling CrossFade  */
	
	private List<AnimationState> bnkPlayOnceState = null;
	private List<AnimationState> bnkDefaultState = null;
	
	private void BackupPlayOnceState() {
		if (bnkPlayOnceState == null && anim != null) {
			bnkPlayOnceState = new List<AnimationState>();
			bnkDefaultState = new List<AnimationState>();
			foreach (AnimationState state in anim) {
				if (state.wrapMode == WrapMode.Once){
					bnkPlayOnceState.Add(state);
				} else if (state.wrapMode == WrapMode.Default) {
					bnkDefaultState.Add(state);
				}
			}
		}
	}
	
	private void RevertPlayOnceState(string animName) {
		BackupPlayOnceState();
		foreach (AnimationState state in bnkPlayOnceState) {
			if (state.name.Equals(animName)) {
				state.wrapMode = WrapMode.Once;
			}
		}
		foreach (AnimationState state in bnkDefaultState) {
			if (state.name.Equals(animName)) {
				state.wrapMode = WrapMode.Default;
			}
		}
	}
	
	public bool IsAnimPlaying(){
		return isAnimationPlaying;
	}
	/**
		\overload	void	PlayAnimation(string animName)
		\brief	Play the animation with default animation speed
		\param[in]	animName	Name of specific animation which need play
	*/
	
	public void PlayAnimation (string animName){
		PlayAnimation (animName, defaultAnimSpeed);
	}
	
	/**
		\fn	void	AnimationPlay(animName : String, speed : float)
		\brief	Play the animation with specific animation speed
		\param[in]	animName	Name of specific animation which need play
		\param[in]	speed	Unity animation speed
	*/
	public void PlayAnimation(string animName, float speed){
		isAnimationPlaying = true;
		currectAnimName = animName;
		RevertPlayOnceState(animName);
		anim.Play(animName);
		SetAnimationSpeed(speed);
	}

	/**
		\overload	void	AnimationCrossFade(animName : String)
		\brief	Cross fade to the animation with default animation speed and blending time
		\param[in]	animName	Name of specific animation which need play
	*/
	public void CrossFadeAnimation(string animName){
		CrossFadeAnimation(animName, animBlendHoldSec);
	}

	/**
		\overload	void	AnimationCrossFade(animName : String, blendHoldSec : float)
		\brief	Cross fade to the animation with default animation speed and specific blending time
		\param[in]	animName	Name of specific animation which need play
		\param[in]	blendHoldSec	Time in seconds of blend the animation
	*/
	public void CrossFadeAnimation(string animName, float blendHoldSec){
		CrossFadeAnimation(animName, blendHoldSec, defaultAnimSpeed);
	}

	/**
		\fn	void	AnimationCrossFade(animName : String, blendHoldSec : float, speed : float)
		\brief	Cross fade to the animation with specific animation speed and blending time
		\param[in]	animName	Name of specific animation which need play
		\param[in]	blendHoldSec	Time in seconds of blend the animation
		\param[in]	speed	Unity animation speed
	*/
	public void CrossFadeAnimation(string animName, float blendHoldSec, float speed){
		if (!isAnimationPlaying){
			if (currectAnimName != ""){
				anim.Play(currectAnimName);
				foreach (AnimationState state in anim) {
					if (state.name.Equals(currectAnimName)) {
						state.time = state.length-0.001f;
						if (state.wrapMode == WrapMode.Once || state.wrapMode == WrapMode.Default) {
							state.wrapMode = WrapMode.ClampForever;
						}
					}
				}
			}
		}
		isAnimationPlaying = true;
		SetAnimationSpeed(speed);
		RevertPlayOnceState(animName);
		anim.CrossFade(animName, blendHoldSec);
		currectAnimName = animName;
	}	
	
	public void ResetAnimation (){
		isAnimationPlaying = false;
		if (defaultAnimName != null && !defaultAnimName.Equals("")){
		    if (defaultAnimName != currectAnimName){
				CrossFadeAnimation(defaultAnimName);
			}
		}
	}
	
	/**
		\fn	void	SetAnimationSpeed(speed : float)
		\brief	Set all animation's unity speed to specific speed
		\param[in]	speed	Value of unity animation speed
	*/
	public void SetAnimationSpeed(float speed){
		foreach (AnimationState state in anim) {
			state.speed = speed;
		}
	}
}
