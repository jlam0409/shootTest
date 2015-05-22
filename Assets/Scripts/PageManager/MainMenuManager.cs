using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public static MainMenuManager instance;

	public void Awake(){
		instance = this;
	}

	public void StartGame(){
		PageManager.GoToPage(PageName.GAMEPLAY);
		GamePlayManager.instance.StartGame();
	}
}
