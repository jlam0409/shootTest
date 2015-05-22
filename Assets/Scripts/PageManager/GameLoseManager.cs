using UnityEngine;
using System.Collections;

public class GameLoseManager : MonoBehaviour {
	public static GameLoseManager instance;
	public TextMesh messageText;
	public TextMesh buttonLabelText;

	public void Awake(){
		instance = this;
	}

	public void NextGame(){
		PageManager.GoToPage (PageName.MAINMENU);
	}

	public void SetMessage(string message, string buttonLabel){
		messageText.text = message;
		buttonLabelText.text = buttonLabel;
	}
}
