using UnityEngine;
using System.Collections;

public class GamePlayManager : MonoBehaviour {
	public static GamePlayManager instance;

	public void Awake(){
		instance = this;
	}

	public void StartGame(){
		GameManager.instance.CreateGame(new Callback(WinGame), new Callback(LoseGame));
		GameManager.instance.ResumeGame();
	}

	public void PauseGame(){
		GameManager.instance.PauseGame();
		PageManager.GoToPage(PageName.GAMEPAUSE);
	}

	
	public void RestoreGame(){
		GameManager.instance.CreateGame (new Callback(WinGame), new Callback(LoseGame));
	}

	public void ExitGame(){
		GameManager.instance.PauseGame();
		GameManager.instance.DestroyGame();
		PageManager.GoToPage(PageName.MAINMENU);
	}
	

	public void WinGame(){
		GameManager.instance.PauseGame();
		GameManager.instance.DestroyGame();
		PageManager.GoToPage(PageName.GAMEWIN);
	}

	public void LoseGame(){
		GameManager.instance.PauseGame();
		GameManager.instance.DestroyGame();
		PageManager.GoToPage(PageName.GAMELOSE);

		if (PlayerData.bullet == 0){
			PlayerData.bullet = 10;
			GameLoseManager.instance.SetMessage("Oops, no bullets left.\nYou have received a gift from God: \n10 bullets", "Thanks God!");
		} else {
			GameLoseManager.instance.SetMessage("Don't give up, try again next time.", "OK");
		}
	}
}
