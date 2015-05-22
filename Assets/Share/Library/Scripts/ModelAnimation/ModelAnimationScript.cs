using UnityEngine;
using System.Collections;

public class ModelAnimationScript : MonoBehaviour {
//	private GameObject callbackListener;	/**< Hold the reference of call back game object receiver, will send a message named "AnimationSystemCallback" when PlayOnce finished */
//	private string callbackFunction;
//	private string callbackArgument;
	public Callback callback;
	
	public bool autoBlendToDefault = false;
	public string defaultAnimName;
	public Animation character;
	
	private ModelAnimationManager animManager;	
	
	public void Awake(){
		animManager = new ModelAnimationManager();
		animManager.defaultAnimName = defaultAnimName;	
	}
	
	public void SetAnimSpeed(float speed){
		animManager.SetAnimationSpeed(speed);	
	}
	
	public void PlayAnimation (string animKey){
		animManager.anim = character;
		animManager.CrossFadeAnimation(animKey);
	}
	
	public void Update(){
		ProcessAnimation();
	}
	
//	public void SetCallback(GameObject listener, string function, string argument){
//		callbackListener = listener;
//		callbackFunction = function;
//		callbackArgument = argument;
//	}
	
	/**
		\fn	void	ProcessAnimation()
		\brief	Process the Animation
	*/
	private void ProcessAnimation(){
		/*
		if (callbackListener == null){
			Debug.Log ("Animation processing, listener:" + callbackListener + "\tfunction:" + callbackFunction + "\targument" + callbackArgument);
		} else {
			Debug.LogWarning ("Animation processing, listener:" + callbackListener + "\tfunction:" + callbackFunction + "\targument" + callbackArgument);
		}
		*/
		// Play the default animation if the animation finished
		if (animManager.IsAnimPlaying() && !animManager.anim.isPlaying){
			
			if (autoBlendToDefault){
				if (animManager.defaultAnimName != animManager.currectAnimName){
					animManager.ResetAnimation();
				}
			}
			
//			if (callback != null)
//				callback.SendCallback(this);
			if (callback != null){
				Callback buffer = new Callback(callback);
				callback = null;
				buffer.SendCallback(this);

			}	
		} else {
			//Debug.Log ("Animation in progress");	
		}
	}
}
