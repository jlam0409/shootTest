//!
/*!	\file PageScript.cs
 *  \author michael
 * 	\version 0.1.0
 * 	\date 2012.03.29
 * 
 * 	\bug none at the moment, but maybe later ;p
 * 	\warning drugs kill
 * 
 */


/*! Uses Namespaces...
 * 	UnityEngine
 * 	System (for Action Class)
 * 	System.Collections 
 */
using UnityEngine;
using System.Collections;
using System;					//*!< for Action Class


//! \class PageScript
/*!
 * 	\brief An abstract base class for all game pages.
 * 
 * 	A page in game is made up of a 'Page' component (Page.cs) and a child class of PageScript (PageScript.cs).
 * 	All functionality / actions of a particular game page reside in the class that inherits this PageScript.
 * 
 * 	PageStart() must be implemented (though an optional PageStart(string) can be used instead)
 * 
 * \extends MonoBehaviour
 *	  	 
 */
public abstract class PageScript : MonoBehaviour {
	
	
	public static PageScript instance;		//*!< creates a static instance of type PageScript
	
	
	//! Wakey Wakey...
	/*! Sets instance inheriting child script. Default Unity Awake() 
	 */
	public void Awake(){
		instance = this;
	}
	
	public void Init(){
		
	}
	
	//! \note TODO need to double check whether both PageAwake() and PageAwake(string) will be called by PageManager
	//! \note TODO test whether method calls or commands finish before yield break is reached returning to PageManager???
	  
	/*! \brief Method (virtual) to initialize the page (before display).
	 * 
	 * 	PageAwake() is called before the page is displayed. So any setup required by the page goes here.
	 * 	Two implementations are provided PageAwake() and PageAwake(string), the latter can accept user
	 * 	defined "flags".  The former chains into the latter.
	 * 
	 * 	This method is virtual, so an inheriting class need not implement it (by default it will do nothing).
	 * 	To override type the following in your inheriting class.
	 * 
	 * 		public override IEnumerator PageAwake() 
	 * 		{ 
	 * 			...your initializing code here... 
	 * 			yield break;	// returns empty
	 * 		}
	 * 
	 * 	NOTE: test whether method calls or commands finish before yield break is reached returning to PageManager???
	 * 
	 * 	\param none
	 *	\returns nothing
	 * 
	 */	 
	public virtual IEnumerator PageAwake()
	{ 
		//PageAwake("Default");		
		yield break;
	}	
	/*! \deprecated void no longer possible due to load screen
	 * 
	 *	public virtual void PageAwake()	
	 *	{	PageAwake("Default");	}
	 */


	
	/*! \brief Method (virtual) to initialize the page (before display), takes in a flag of type string.
	 * 
	 * 	PageAwake(string) is called before the page is displayed. So any setup required by the page goes here.
	 * 	Two implementations are provided PageAwake() and PageAwake(string), the latter can accept user
	 * 	defined "flags".  The former chains into the latter.
	 * 
	 * 	This method is virtual, so an inheriting class need not implement it (by default it will do nothing).
	 * 	To override type the following in your inheriting class.
	 * 
	 * 		public override void PageAwake(string flag) 
	 * 		{ 
	 * 			switch(flag)
	 * 			{
	 * 				...your initializing code here... 
	 * 			}
	 * 			yield break;	// returns empty
	 * 		}
	 * 
	 * 	NOTE: test whether method calls or commands finish before yield break is reached returning to PageManager???
	 * 
	 * 	\param[in] flag A String value to define actions within PageAwake()
	 *	\returns nothing
	 *	\throws ArgumentException The flag provided isn't recognized
	 * 
	 */		
	public virtual IEnumerator PageAwake(string flag)
	{
		switch(flag)
		{
			case "Default"	:	yield break;								
								//break;
			default			:	throw new ArgumentException(String.Format("Unrecognized flag `{0}`!", flag));
		}		
	}
	/*! \deprecated void no longer possible due to load screen
	 * 
	 *	public virtual void PageAwake(string flag)
	 *	{	
	 *		switch(flag)
	 *		{
	 *			case "Default"	:	//PageAwake();
	 *								break;
	 *			default			:	throw new ArgumentException(String.Format("Unrecognized flag `{0}`!", flag));					
	 *		}
	 *	}	
	 */
		
	
	/*! \brief Method (abstract) designating the start of the game page (after display).
	 * 
	 *	PageStart() is called after the game page is initialized (see PageAwake()) and displayed.  
	 *	So any actions (if any) that need to be run once the page is shown should go here.
	 *	
	 *	Two methods are provided PageStart() and PageStart(string), the latter can accept user
	 *	defined "flags". While the former simplifies code. By default the latter chains into the former.
	 *
	 *	This method is abstract, so MUST be implemented in any inheriting class. If you require a flag,
	 *	then just override both PageStart() and PageStart(string), and leave PageStart() empty.
	 *	To override type the following in your inheriting class.
	 *
	 *		public override void PageStart()
	 *		{ ...your code goes here... }
	 *
	 *	\param none
	 *	\returns nothing
	 * 
	 */
	public abstract void PageStart();
	
	
	/*! \brief Method (virtual) designating the start of the game page (after display).
	 * 
	 *	PageStart(string) is called after the game page is initialized (see PageAwake()) and displayed.  
	 *	So any actions (if any) that need to be run once the page is shown should go here.
	 *	
	 *	Two methods are provided PageStart() and PageStart(string), the latter can accept user
	 *	defined "flags".  By default the latter chains into the former.
	 *
	 *	This method is virtual, so an inheriting class need not implement it.
	 *	To override type the following in your inheriting class.
	 *
	 *		public override void PageStart(string flag)
	 *		{ ...your code goes here... }
	 *
	 *	\param[in] flag A String value to define actions within PageStart()
	 *	\returns nothing
	 * 	\throws ArgumentException The flag provided isn't recognized
	 * 
	 */
	public virtual void PageStart(string flag)
	{	
		switch(flag)
		{
			case "Default"	:	PageStart(); break;
			default			:	throw new ArgumentException(String.Format("Unrecognized flag `{0}`!", flag));
		}
	}
	
	public virtual void PageOut(){
	}

	// NOTE : Maybe best to put this in a "multimedia" class
	// NOTE : alternatively, StartCoroutine(SoundManager.RequestAndWaitPlayVO(testVO, new Action(Continue)));
	// NOTE : playing a VO signifies end of a method
	// Description : Plays VO
	// Default : calls SoundManager to play VO
	// Input : AudioClip to play, a method in page to return to
	//		
	
	/*! \brief Helper method to play a VO with a callback
	 * 
	 * 	Plays a given VO. While the VO plays the game is held. 
	 * 
	 *	This ends a method
	 *
	 *	\param[in] theVO The VO to play of type AudioClip
	 *	\param[in] dynamicMethod The (void) method to return to when VO completes
	 *	\returns nothing
	 * 
	 */
	public virtual void PlayVO(AudioClip theVO, Action dynamicMethod) 
	{
		StartCoroutine(AudioManager.RequestAndWaitPlayVO(theVO, dynamicMethod));
	}
	public virtual void PlayVO(AudioClip theVO) 
	{
		StartCoroutine(AudioManager.RequestAndWaitPlayVO(theVO));
	}
	
	// NOTE : Maybe best to put this in a "multimedia" class
	// Description : Plays a movie
	// Input : a string of the filename (allows for subfolders)
	// Requirements : file must reside in Assets/StreamingAssets/
	//
	public virtual void PlayMov(string theMov)
    {
#if ON_ANDROID
		Handheld.PlayFullScreenMovie(theMov,Color.black, FullScreenMovieControlMode.Hidden);
#endif
	}
	//

}

