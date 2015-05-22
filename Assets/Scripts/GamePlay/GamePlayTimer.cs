using UnityEngine;
using System.Collections;

public enum GamePlayTimerState{
	Start,
	Stop
}	

public enum TimerMode{
	COUNTUP,
	COUNTDOWN
}	

public class GamePlayTimer : MonoBehaviour {
	private float mTime;
	public TextMesh timerText;

	private float mTimeout = 10f;
	public Callback timeoutCallback;
	
	public GamePlayTimerState state = GamePlayTimerState.Stop;
	public TimerMode mode = TimerMode.COUNTUP;

	public void SetupTimer(float timeout, Callback callback){		
		mTimeout = timeout;
		timeoutCallback = callback;
	}
	
	public void Update () {
		if (state == GamePlayTimerState.Start){
			if (mode == TimerMode.COUNTUP){
				mTime += Time.deltaTime;
				if (mTime > mTimeout){
					timeoutCallback.SendCallback(this);
				}
			} else {
				mTime -= Time.deltaTime;
				if (mTime <= 0){
					Debug.LogError ("Timer timeout, send callback!");
					timeoutCallback.SendCallback(this);
					state = GamePlayTimerState.Stop;
				}
			}
			UpdateText (timerText, mTime);
		}
	}
	
	private void UpdateText (TextMesh text, float time){
		// convert second to minute and second
		int minute = (int)(time / 60);
		int second = (int)(time % 60);
		
		if (minute > 59)
			minute = 59;
		
		
		string timerText = Padding (minute) + ":" + Padding(second);
		text.text = timerText;//time.ToString();	
	}
	
	private string Padding(float number){
		if (number < 10)
			return "0" + number;
		else 
			return number.ToString();
	}
	
	public void ResetTimer(){
		state = GamePlayTimerState.Stop;
		if (mode == TimerMode.COUNTUP){
			mTime = 0;	
		} else {
			mTime = mTimeout;	
		}
		UpdateText(timerText, mTime);
	}
	
	
	public void StartTimer(){
		state = GamePlayTimerState.Start;
	}
	
	public void StopTimer(){
		state = GamePlayTimerState.Stop;
	}

	public float GetTime(){
		return mTime;
	}

	public void SetTime(float time){
		mTime = time;
	}
}
