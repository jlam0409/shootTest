using UnityEngine;
using System.Collections;

public class GameWinManager : MonoBehaviour {
	public static GameWinManager instance;

	public void Awake(){
		instance = this;
	}

	public void NextGame(){
		PageManager.GoToPage(PageName.MAINMENU);
	}
}
