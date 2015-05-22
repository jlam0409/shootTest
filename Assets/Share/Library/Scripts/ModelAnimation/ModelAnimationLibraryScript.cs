using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelAnimationLibraryScript : MonoBehaviour {
	
	public List<Animation> allAnimation;
	
	public void AddAnimation(Animation anim){
		allAnimation.Add (anim);
	}
	
	public Animation GetAnimation(string animName){
		foreach (Animation eachAnimation in allAnimation){
			if (eachAnimation.name	 == animName){
				return eachAnimation;	
			}
		}
		return null;
	}
}
