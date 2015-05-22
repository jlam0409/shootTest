using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PageManagerScript : MonoBehaviour {
	
	//public static PageManagerScript instance;	// necessary???
	
	public static float transitionTime, currentTime = 0.0f;
//	private static float timeDelay = 0.1f;

	// add STATUS property????
	
	// set default page
	public PageName defaultPageName = PageName.MAINMENU;
	public PageName loadingPageName = PageName.LOADING;

	public Vector3 inPosition = Vector3.zero;
	public Vector3 outPosition = new Vector3(20.0f,0,0);
	
	
	public void Awake() 
	{
		/*
		if (instance == null) {
			instance = this;
		} else {
			DebugText.Log ("More than one instance found!");
		}
		*/
		//transitionTime = currentTime = 0.0f;
	}

	public void Start(){
		PageManager.inPosition = inPosition;
		PageManager.outPosition = outPosition;
		PageManager.SetLoadingPage(loadingPageName);
//		PageManager.InitPage(this);
		PageManager.InitPage();
		if (defaultPageName != PageName.NONE)
			PageManager.GoToPage(defaultPageName);
	}
	
	
	// checks
	public void Update(){
		// check if loading a page??? maybe not worth the call
//		if(PageManager.IsLoading()){
//			//Debug.Log("IsLoading()");
//			currentTime = Time.realtimeSinceStartup;
//			if((currentTime - transitionTime) > timeDelay)
//				PageManager.ShowLoadingPage();				
//		}
	}

	public static void StartTransitionTime()
	{
		transitionTime = Time.realtimeSinceStartup;
	}
	
}
