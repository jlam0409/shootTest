using UnityEngine;
using System.Collections;
using System;					// for Enum.Parse()



/*
// declared (and editable) in separate file
public enum PageName{
	NONE,
	LOADING,
	MAIN_MENU,	
	OPTIONS,
	GAME,	
}
*/


public enum PageState {
	NONE,
	IN,
	OUT
}

public enum PageType {
	NORMAL,
	POPUP
}

public class Page : MonoBehaviour {
	
	// Variables
	private Transform _pageXform;
	public PageName pageName;
	
	public PageType pageType = PageType.NORMAL;
	public PageState pageState = PageState.NONE;
	private PageScript pageScript;						// the custom PageScript attached to Page
	
	// Properties
	public PageScript ThePageScript
	{	
		get { return pageScript;	}	// read-only
	}
	
	
	public void Awake(){
		_pageXform = transform;
		if (pageType == PageType.NORMAL)
			PageManager.AddPage (this);
		else
			PageManager.AddPopupPage (this);		
		
		
		pageScript = gameObject.GetComponent<PageScript>() as PageScript;	// gets PageScript component

/*
		// if no PageScript component attached, add one	
		// NOTE : not possible as PageScript is an abstract class	
		if(!pageScript)
		{
			DebugText.Log("pagescript is null");
			pageScript = gameObject.AddComponent(typeof(PageScript)) as PageScript;
		}
*/

	}
	
	public void SetPageState( PageState state_){
		if ( state_ == pageState)
			return;
		
		pageState = state_;
		switch (state_){
			case PageState.IN:
				_pageXform.localPosition = PageManager.inPosition;
				break;
			case PageState.OUT:
				_pageXform.localPosition = PageManager.outPosition;
				break;
		}
	}

	// Removes page
	void OnDestroy()
	{
		//DebugText.Log("Destroying " + this.ToString());
		PageManager.RemovePage(this);	
	}
	
	public override string ToString ()
	{
		return pageName.ToString();
	}	
	
	// converts string to PageName (an enum)
	public static PageName ToPageName(string nameOfPage)
	{
		return (PageName)Enum.Parse(typeof(PageName), nameOfPage);		
	}
		

}