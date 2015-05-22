using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour {
	public static AppManager instance;

	private static string RESTORE_KEY = "restoreDataExists";
	private static string PLAYER_BULLET_KEY = "playerBullet";
	private static string PLAYER_SCORE_KEY = "playerScore";

	public void Awake(){
		instance = this;
	}

	public IEnumerator Start(){

		LoadPlayerData();
		// auto add 10 bullets if the player has 0 bullets
		if (PlayerData.bullet == 0)
			PlayerData.bullet = 10;

		// should display loading and wait until all game objects initialized
		yield return new WaitForSeconds(0.5f);
		int restoreDataExists = PlayerPrefs.GetInt (RESTORE_KEY);
		if (restoreDataExists == 1){
			PlayerPrefs.DeleteKey (RESTORE_KEY);
			GamePlayManager.instance.RestoreGame();
			GameManager.instance.LoadStateData();
			GamePlayManager.instance.PauseGame();
		}
	}

	public void LoadPlayerData(){
		if (PlayerPrefs.HasKey (PLAYER_BULLET_KEY))	
			PlayerData.bullet = PlayerPrefs.GetInt (PLAYER_BULLET_KEY);

		if (PlayerPrefs.HasKey (PLAYER_SCORE_KEY))
			PlayerData.score = PlayerPrefs.GetInt (PLAYER_SCORE_KEY);
	}

	public void SavePlayerData(){
		PlayerPrefs.SetInt (PLAYER_BULLET_KEY, PlayerData.bullet);
		PlayerPrefs.GetInt (PLAYER_SCORE_KEY, PlayerData.score);
	}

	public void OnApplicationPause(bool pause){
		if (pause){
			GamePlayManager.instance.PauseGame();
		}
	}

	public void OnApplicationQuit(){
		if (GameManager.instance.state != GameState.OUT){
			PlayerPrefs.SetInt (RESTORE_KEY, 1);
			GameManager.instance.SaveStateData();
		}
		SavePlayerData();
		PlayerPrefs.Save();
	}
}