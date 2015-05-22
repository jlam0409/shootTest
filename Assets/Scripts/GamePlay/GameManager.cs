using UnityEngine;
using System.Collections;

public enum GameState{
	IN,
	RUNNING,
	PAUSE,
	OUT
}

// this class manage the start, pause, resume and end of the game, also determind how win/lose of the game
public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public GameState state = GameState.PAUSE;
	private Callback mWinGameCallback;
	private Callback mLoseGameCallback;

	public GameController gameController;
	public GameObject shootDetector;
	public GamePlayTimer timer;
	public XMLLoader loader;
	private static string GAMEPLAY_TIMER_KEY = "gameplayTimer";

	public void Awake(){
		instance = this;
		shootDetector.SetActive(false);
	}

	public void CreateGame(Callback winGameCallback, Callback loseGameCallback){
		mWinGameCallback = winGameCallback;
		mLoseGameCallback = loseGameCallback;

		// init all game data 
		LoadConfigDataFromFile();

		state = GameState.IN;
		// kick start the game using the data set
		timer.SetupTimer(GameConfigData.instance.timer, new Callback(LoseGame));
		timer.ResetTimer();

		gameController.Init(new Callback(WinGame), new Callback(LoseGame));
	}

	// enable shootDectector
	public void ResumeGame(){
		state = GameState.RUNNING;
		shootDetector.SetActive(true);

		timer.StartTimer();
		gameController.StartGenerate();
		gameController.ResumeObjectMovement();
	}

	public void PauseGame(){
		state = GameState.PAUSE;
		shootDetector.SetActive(false);

		timer.StopTimer();
		gameController.StopGenerate();
		gameController.PauseObjectMovement();
	}

	public void DestroyGame(){
		// destroy all gameObjects
		gameController.DestroyAllObject();
		state = GameState.OUT;
	}

	// when destroying the red debris, the game win
	public void WinGame(){
		StartCoroutine ("PlayWinGameAnimation");
	}

	private IEnumerator PlayWinGameAnimation(){
		Debug.Log ("Game Manager, GameWin! Play animation");
		// play animation
		yield return new WaitForSeconds (2f);
		mWinGameCallback.SendCallback(this);
	}

	public void LoseGame(){
		StartCoroutine("PlayLoseGameAnimation");
	}

	private IEnumerator PlayLoseGameAnimation(){
		Debug.Log ("Game Manager, GameLose! Play animation");

		yield return new WaitForSeconds (2f);
		mLoseGameCallback.SendCallback(this);
	}

	private void LoadConfigDataFromFile(){
		loader.LoadXML();
	}

	public void SaveStateData(){
		gameController.SaveStateData();
		PlayerPrefs.SetFloat (GAMEPLAY_TIMER_KEY, timer.GetTime() );
	}

	public void LoadStateData(){
		gameController.LoadStateData();
		timer.SetTime( PlayerPrefs.GetFloat (GAMEPLAY_TIMER_KEY) );
	}
}
