using UnityEngine;
using System.Collections;

public class GamePauseManager : MonoBehaviour {
	public static GamePauseManager instance;

	public void Awake(){
		instance = this;
	}

	public void ResumeGame(){
		GameManager.instance.ResumeGame();
		PageManager.GoToPage(PageName.GAMEPLAY);
	}

	public void QuitGame(){
		GamePlayManager.instance.ExitGame();	
	}
}
