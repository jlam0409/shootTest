using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Please remember to set the languagePathKey after LanguageManager get the language setting, default use English VO.
 * 
 * Example:
 */
public enum AudioState {
	START,
	PLAY,
	PAUSE,
	STOP,
	FADE,
	FINISH
}

public enum AudioType {
	MUSIC = 0,
	SFX,
	BUTTON_SFX,
	VO
}

public enum AudioPlayMode {
	REQUEST_PLAY,
	NORMAL_PLAY,
	FORCE_PLAY,
	QUEUE_PLAY
}

public class AudioManager {
	private static string enableMusicKey = "EnableMusic";
	private static string enableSFXKey = "EnableSoundEffect";
	private static string enableVOKey = "EnableVoiceOver";
	
	private static bool initPlayerPrefsMusic = false;
	private static bool initPlayerPrefsSFX = false;
	private static bool initPlayerPrefsVO = false;
	
	private static bool _enableMusic  = true;
	private static bool _enableSFX  = true;
	private static bool _enableVO = true;
	
	private static int playAndLoopSfxCounter = 0;
	private static List<GameObject> loopSfxList = new List<GameObject>();
	private static List<GameObject> fadeSfxList = new List<GameObject>();
	
	public static void CleanLoopAndFadeSfx()
	{
		if(loopSfxList.Count > 0)
		{
			foreach(GameObject loopSfx in loopSfxList)
			{
				GameObject.Destroy(loopSfx);
			}
			loopSfxList.Clear();
		}
		if(fadeSfxList.Count > 0)
		{
			foreach(GameObject fadeSfx in fadeSfxList)
			{
				GameObject.Destroy(fadeSfx);
			}
		}
		playAndLoopSfxCounter = 0;
	}
	
	public static void SetExternalSfxVolume(AudioSource inSource) {
		if (!_enableSFX) {
			inSource.volume = 0f;
			inSource.mute = true;
		}
	}
	
    public static bool enableMusic {
        set {
			_enableMusic = value;
			SetMusicEnable(_enableMusic);
			PlayerPrefs.SetInt(enableMusicKey, Convert.ToInt32(_enableMusic));
			PlayerPrefs.Save();
		}
		get {
			if (!initPlayerPrefsMusic) {
				initPlayerPrefsMusic = true;
				if (! PlayerPrefs.HasKey (enableMusicKey)){
					PlayerPrefs.SetInt (enableMusicKey,1);
					_enableMusic = true;
					PlayerPrefs.Save();
				} else {
					_enableMusic = PlayerPrefs.GetInt (enableMusicKey) == 1;	
				}
			}
			return _enableMusic;
		}
    }
	
    public static bool enableSFX {
        set {
			_enableSFX = value;
			PlayerPrefs.SetInt(enableSFXKey, Convert.ToInt32(_enableSFX));
			PlayerPrefs.Save();
		}
		get {
			if (!initPlayerPrefsSFX) {
				initPlayerPrefsSFX = true;
				if (! PlayerPrefs.HasKey (enableSFXKey)){
					PlayerPrefs.SetInt (enableSFXKey,1);
					_enableSFX = true;
					PlayerPrefs.Save();
				} else {
					_enableSFX = PlayerPrefs.GetInt (enableSFXKey) == 1;	
				}
			}
			return _enableSFX;
		}
    }
	
    public static bool enableVO {
        set {
			_enableVO = value;
			PlayerPrefs.SetInt(enableVOKey, Convert.ToInt32(_enableVO));
			PlayerPrefs.Save();
		}
		get {
			if (!initPlayerPrefsVO) {
				initPlayerPrefsVO = true;
				if (! PlayerPrefs.HasKey (enableVOKey)){
					PlayerPrefs.SetInt (enableVOKey,1);
					_enableVO = true;
					PlayerPrefs.Save();
				} else {
					_enableVO = PlayerPrefs.GetInt (enableVOKey) == 1;	
				}
			}
			return _enableVO;
		}
    }
	
	private static AudioManagerScript _script = null;
    private static AudioManagerScript script {
        set {
			_script = value;
		}
		get {
			if (_script == null) {
				GameObject audioManager = new GameObject();
				audioManager.name = "AudioManager";
				audioManager.transform.position = Vector3.zero;
				_script = audioManager.AddComponent(typeof(AudioManagerScript)) as AudioManagerScript;
				// _script.Init();
			}
			return _script;
		}
    }
	
	// button sfx attributes
	private static AudioSource _buttonSfxAudio;
    private static AudioSource buttonSfxAudio {
        set {
			_buttonSfxAudio = value;
		}
		get {
			if (script == null) {
				DebugText.Log("Create the AudioManager gameObject");
			}
			return _buttonSfxAudio;
		}
    }
	
	private static AudioClip[] buttonSfxClipArray = null;
	
	// pitched sfx attributes
	//private static AudioSource pitchedAudioSource;
	
	// sfx attributes
	private static AudioSource _sfxAudio;
    private static AudioSource sfxAudio {
        set {
			_sfxAudio = value;
		}
		get {
			if (script == null) {
				DebugText.Log("Create the AudioManager gameObject");
			}
			return _sfxAudio;
		}
    }
	
	static AudioClip[] sfxClipArray = null;
	public static float otherClipDelayTime = 0.0f;
	public static float sameClipDelayTime = 1.0f;
	private static AudioClip lastAudioClip = null;
	private static float lastPlayTime;
	
	// music attributes
	private static AudioSource _musicAudio;
    private static AudioSource musicAudio {
        set {
			_musicAudio = value;
			_musicAudio.loop = true;
		}
		get {
			if (script == null) {
				DebugText.Log("Create the AudioManager gameObject");
			}
			return _musicAudio;
		}
    }
	static AudioClip[] musicClipArray = null;
	
	// Fade in/out music variable (use by AudioManagerScript)
	private static AudioState musicState = AudioState.STOP;
	private static float startVol = 0.0f;	/**< Start volume of the background music when fading in/out */
	private static float endVol = 1.0f;	/**< End volume of the background music when fading in/out */
	private static float fadeDuration = 0.0f;
	private static float fadeDiff = 0.0f;
	
	// vo attributes
	private static AudioSource _voAudio;
    private static AudioSource voAudio {
        set {
			_voAudio = value;
		}
		get {
			if (script == null) {
				DebugText.Log("Create the AudioManager gameObject");
			}
			return _voAudio;
		}
    }
	static AudioClip[] voClipArray = null;
	
	// Chinese, English, French, Spanish
	private static string languagePathKey = "English";
	
	//private static String nextMusicName = "";
	//private static Boolean isCutMusic= false;
	
	//private static float lastTimeScale = 1.0f;
	
	//private static float fadeInMusicTime= 0.0f;
	
	//private static Boolean isPause = false;
	//private static float tempVolume  = 0.0f;

	// Set Audio Source At Awake By Game Manager
	public static void SetAudioSource(AudioType audioType, AudioSource source) {
		switch (audioType) {
			case AudioType.BUTTON_SFX:
				buttonSfxAudio = source;
				break;
			case AudioType.MUSIC:
				musicAudio = source;
				break;
			case AudioType.SFX:
				sfxAudio = source;
				break;	
			case AudioType.VO:
				voAudio = source;
				break;
			default:
				DebugText.LogError ("Unknown type: " + audioType + " in SetAudioSource");
				break;
		}
	}
	
	// Set the language path, for example Chinese, English, French, Spanish
	public static void SetLanguagePathKey(string key_) {
		languagePathKey = key_;
	}

	// Set Audio Clips At Awake By Game Manager	for play by clipname
	public static void SetAudioClip(AudioType audioType, AudioClip[] clipArray) {
		switch (audioType) {
			case AudioType.SFX:
				sfxClipArray = clipArray;
				break;
			case AudioType.MUSIC:
				musicClipArray = clipArray;
				break;
			case AudioType.BUTTON_SFX:
				buttonSfxClipArray = clipArray;
				break;
			case AudioType.VO:
				voClipArray = clipArray;
				break;
		}
	}
	
	public static void SetMusicEnable(Boolean enable) {
		if (enable) {
			SetMusicVolume(1.0f);
		} else {
			SetMusicVolume(0.0f);
		}
		musicAudio.mute = !enable;
	}
	
	public static void SetMusicVolume(float volume) {
		musicAudio.volume = volume;
	}
	
	// Play Music by clipName
	public static void RequestPlayMusic(String clipName) {
		RequestPlayMusic(GetAudioClip(AudioType.MUSIC, clipName));
	}
	
	public static void RequestPlayMusic(AudioClip curAudioClip) {
		SetMusicEnable(enableMusic);
		musicAudio.clip = curAudioClip;
		musicAudio.Play();
	}
	
	public static bool IsMusicEnable(){
		return !musicAudio.mute;
	}
	
	public static bool IsPlayingMusic() {
		return musicAudio.isPlaying;
	}
	
	public static float GetMusicVolume(){
		return musicAudio.volume;	
	}
	
	public static AudioClip GetCurrentMusic(){
		return musicAudio.clip;	
	}
	
	/**
	 * For Example:
	 * 	AudioManager.RequestFadeMusic(1.0f, 0.0f, 15.0f);
	 * Fade the current music from 1.0f to 0.0f within 15sec
	 */
	public static void RequestFadeMusic(float startVol_, float endVol_, float fadeDuration_) {
		musicState = AudioState.FADE;
		SetMusicVolume(startVol_);
		if (!musicAudio.isPlaying) {
			musicAudio.Play();
		}
		startVol = startVol_;
		endVol = endVol_;
		fadeDuration = fadeDuration_;
		fadeDiff = 0.0f;
	}
	
	/**
		\fn	void	Update ( )
		\brief	Update function call by AudioManagerScript
	*/
	public static void Update() {
		if (musicState == AudioState.FADE) {
			fadeDiff += Time.deltaTime;
			float curVol = 0.0f;
			if (fadeDiff < fadeDuration) {
				curVol = startVol + ((endVol - startVol) / fadeDuration) * fadeDiff;
			} else {
				curVol = endVol;
				musicState = AudioState.PLAY;
			}
			//DebugText.Log("curVol: " + curVol);
			SetMusicVolume(curVol);
		}
	}

	public static void RequestPlayButtonSfx(String clipName) {
		RequestPlayButtonSfx(GetAudioClip(AudioType.BUTTON_SFX, clipName));
	}
	
	public static void RequestPlayButtonSfx(AudioClip curAudioClip) {
		if (!enableSFX) {
			return;
		}
		buttonSfxAudio.PlayOneShot(curAudioClip);
	}
	
	public static void RequestPlayButtonSfx(String clipName, float pitchValue, float volume) {
		if (!enableSFX) {
			return;
		}
		buttonSfxAudio.clip = GetAudioClip (AudioType.BUTTON_SFX, clipName);
		buttonSfxAudio.volume = volume;
		buttonSfxAudio.pitch = pitchValue;
		buttonSfxAudio.Play();
	}
	
	public static void ResetPitchSfx(float val) {
		sfxAudio.pitch = val;
	}
	
	public static void RequestPlaySfx(String clipName) {
		RequestPlaySfx(GetAudioClip(AudioType.SFX, clipName));
	}
	
	public static void RequestPlaySfx(AudioClip curAudioClip) {
		if (!enableSFX) {
			return;
		}
		sfxAudio.PlayOneShot(curAudioClip);
	}
	
	public static IEnumerator RequestPlayAndFadeSfx(AudioClip curAudioClip, float playDuration, float fadeDuration)
	{
		if (!enableSFX) {
			yield break;
		}
		playAndLoopSfxCounter++;
		GameObject fadeSfxHolder =  new GameObject("fadeSfxHolder");
		fadeSfxList.Add(fadeSfxHolder);
		AudioSource fadeSfxSource = fadeSfxHolder.AddComponent<AudioSource>();
		fadeSfxSource.volume = 1f;
		fadeSfxSource.clip = curAudioClip;
		fadeSfxSource.Play();
		yield return new WaitForSeconds(playDuration);
		
		float sfxFadeDiff = 0.01f;
		float timer = 0f;
		float sfxFadeStep = sfxFadeDiff/fadeDuration;
		while(timer < fadeDuration)
		{
			fadeSfxSource.volume = Mathf.Clamp((fadeSfxSource.volume-sfxFadeStep), 0f, 1f);
			timer += sfxFadeDiff;
			yield return new WaitForSeconds(sfxFadeDiff);
		}
		
		fadeSfxSource.Stop();
		fadeSfxList.Remove(fadeSfxHolder);
		MonoBehaviour.Destroy(fadeSfxHolder);
		playAndLoopSfxCounter--;
	}
	
	public static IEnumerator RequestPlayAndFadeSfx(String clipName, float playDuration, float fadeDuration)
	{
		yield return script.StartCoroutine(RequestPlayAndFadeSfx(GetAudioClip(AudioType.SFX, clipName), playDuration, fadeDuration));
	}
	
	public static AudioSource RequestPlayLoopSfx(AudioClip curAudioClip)
	{
		if (!enableSFX) {
			return null;
		}
		playAndLoopSfxCounter++;
		GameObject loopSfxHolder =  new GameObject(curAudioClip.name);
		AudioSource loopSfxSource = loopSfxHolder.AddComponent<AudioSource>();
		loopSfxSource.clip = curAudioClip;
		loopSfxSource.loop = true;
		loopSfxList.Add(loopSfxHolder);
		loopSfxSource.Play();
		return loopSfxSource;
	}	
	
	public static void RequestPlayLoopSfx(String clipName)
	{
		RequestPlayLoopSfx(GetAudioClip(AudioType.SFX, clipName));
	}
	
	public static bool IsPlayingLoopSFX(AudioClip curAudioClip){
		return IsPlayingLoopSFX	(curAudioClip.name);
	}
	
	public static bool IsPlayingLoopSFX(string clipName){
		foreach(GameObject loopSfx in loopSfxList)
		{
			if(loopSfx.name == clipName)
			{
				return loopSfx.audio.isPlaying;
			}
		}
		return false;
	}
	
	public static void RequestStopLoopSfx(AudioClip curAudioClip)
	{
		string clipName = curAudioClip.name;
		foreach(GameObject loopSfx in loopSfxList)
		{
			if(loopSfx.name == clipName)
			{
				loopSfxList.Remove(loopSfx);
				GameObject.Destroy(loopSfx);
				playAndLoopSfxCounter--;
				break;
			}
		}
	}
	
	public static void RequestStopLoopSfx(String clipName)
	{
		RequestStopLoopSfx(GetAudioClip(AudioType.SFX, clipName));
	}
	
	public static IEnumerator RequestFadeLoopSfx(AudioClip curAudioClip, float fadeDuration)
	{
		string clipName = curAudioClip.name;
		foreach(GameObject loopSfx in loopSfxList)
		{
			if(loopSfx.name == clipName)
			{
				AudioSource fadeSfxSource = loopSfx.GetComponent<AudioSource>();
				fadeSfxSource.volume = 1f;
				
				float sfxFadeDiff = 0.01f;
				float timer = 0f;
				float sfxFadeStep = sfxFadeDiff/fadeDuration;
				while(timer < fadeDuration)
				{
					fadeSfxSource.volume = Mathf.Clamp((fadeSfxSource.volume-sfxFadeStep), 0f, 1f);
					timer += sfxFadeDiff;
					yield return new WaitForSeconds(sfxFadeDiff);
				}
				
				fadeSfxSource.Stop();
				loopSfxList.Remove(loopSfx);
				GameObject.Destroy(loopSfx);
				playAndLoopSfxCounter--;
				yield break;
			}
		}
		yield break;
	}
	
	public static IEnumerator RequestFadeLoopSfx(String clipName, float fadeDuration)
	{
		yield return script.StartCoroutine(RequestFadeLoopSfx(GetAudioClip(AudioType.SFX, clipName), fadeDuration));
	}
	
	public static IEnumerator RequestFadeLoopSfx(AudioSource audioSource, float fadeDuration)
	{
		float sfxFadeDiff = 0.01f;
		float timer = 0f;
		float sfxFadeStep = sfxFadeDiff/fadeDuration;
		while(timer < fadeDuration)
		{
			audioSource.volume = Mathf.Clamp((audioSource.volume-sfxFadeStep), 0f, 1f);
			timer += sfxFadeDiff;
			yield return new WaitForSeconds(sfxFadeDiff);
		}
		
		audioSource.Stop();
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithoutDelay(AudioClip[] voQueue)
	{
		if (!enableVO) {
			yield break;
		}
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithSameDelay(voQueue, 0f));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithoutDelay(AudioClip[] voQueue, Action callback)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(callback == null)
		{
			DebugText.LogError("The callback is set to null.");
			yield break;
		}
		
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithSameDelay(voQueue, 0f));
		callback();
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithoutDelay(string[] voQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithoutDelay(audioQueue));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithoutDelay(string[] voQueue, Action callback)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		if(callback == null)
		{
			DebugText.LogError("The callback is set to null.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithoutDelay(audioQueue, callback));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithSameDelay(AudioClip[] voQueue, float delayTime)
	{
		if (!enableVO) {
			yield break;
		}
		
		float[] delayTimeQueue = new float[voQueue.Length];
		for(int i = 0; i < delayTimeQueue.Length; i++)
		{
			delayTimeQueue[i] = delayTime;
		}
		
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithDiffDelay(voQueue, delayTimeQueue));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithSameDelay(AudioClip[] voQueue, float delayTime, Func<IEnumerator>[] callbackQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(callbackQueue == null || callbackQueue.Length == 0)
		{
			DebugText.LogError("The callbackQueue is set to null.");
			yield break;
		}
		
		float[] delayTimeQueue = new float[voQueue.Length];
		for(int i = 0; i < delayTimeQueue.Length; i++)
		{
			delayTimeQueue[i] = delayTime;
		}
		
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithDiffDelay(voQueue, delayTimeQueue, callbackQueue));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithSameDelay(string[] voQueue, float delayTime)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithSameDelay(audioQueue, delayTime));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithSameDelay(string[] voQueue, float delayTime, Func<IEnumerator>[] callbackQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		if(callbackQueue == null || callbackQueue.Length == 0)
		{
			DebugText.LogError("The callbackQueue is set to null.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithSameDelay(audioQueue, delayTime, callbackQueue));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithDiffDelay(AudioClip[] voQueue, float[] delayTimeQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		
		if(voQueue.Length != delayTimeQueue.Length)
		{
			DebugText.LogError("The length of VO array doesn't equal to the length of delay time array.");
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		for(int i = 0; i < voQueue.Length; i++)
		{
			yield return script.StartCoroutine(RequestAndWaitPlayVO(voQueue[i]));
			yield return new WaitForSeconds(delayTimeQueue[i]);
		}
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithDiffDelay(AudioClip[] voQueue, float[] delayTimeQueue, Func<IEnumerator>[] callbackQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		
		if(voQueue.Length != delayTimeQueue.Length)
		{
			DebugText.LogError("The length of VO array doesn't equal to the length of delay time array.");
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		if(callbackQueue == null || callbackQueue.Length == 0)
		{
			DebugText.LogError("The callbackQueue is set to null.");
			yield break;
		}
		
		for(int i = 0; i < voQueue.Length; i++)
		{
			yield return script.StartCoroutine(RequestAndWaitPlayVO(voQueue[i]));
			if(callbackQueue[i] != null)
			{
				yield return script.StartCoroutine(callbackQueue[i]());
			}
			else
			{
				DebugText.LogWarning("callbackQueue[" + i + "] is set to null.");
			}
		}
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithDiffDelay(string[] voQueue, float[] delayTimeQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithDiffDelay(audioQueue, delayTimeQueue));
	}
	
	public static IEnumerator RequestPlayAndWaitVoQueueWithDiffDelay(string[] voQueue, float[] delayTimeQueue, Func<IEnumerator>[] callbackQueue)
	{
		if (!enableVO) {
			yield break;
		}
		
		if(voQueue == null || voQueue.Length == 0)
		{
			DebugText.LogError("There's no vo in the voQueue.");
			yield break;
		}
		
		if(callbackQueue == null || callbackQueue.Length == 0)
		{
			DebugText.LogError("The callbackQueue is set to null.");
			yield break;
		}
		
		AudioClip[] audioQueue = GetAudioQueue(voQueue);
		yield return script.StartCoroutine(RequestPlayAndWaitVoQueueWithDiffDelay(audioQueue, delayTimeQueue, callbackQueue));
	}
	
	private static AudioClip[] GetAudioQueue(string[] voQueue)
	{
		AudioClip[] audioQueue = new AudioClip[voQueue.Length];
		for(int i = 0; i < voQueue.Length; i++)
		{
			audioQueue[i] = GetAudioClip(AudioType.VO, voQueue[i]);
		}
		return audioQueue;
	}
	
	static void RequestQueueSfx(AudioClip curAudioClip) {
		if (!enableSFX) {
			return;
		}
		//DebugText.Log("RequestSoundPlay: " + sfx.name);
		Boolean canPlaySfx = false;
		if (lastAudioClip == null || 
			(lastAudioClip != curAudioClip && Time.time - lastPlayTime > lastAudioClip.length + otherClipDelayTime) || 
			(lastAudioClip == curAudioClip && Time.time - lastPlayTime > lastAudioClip.length + sameClipDelayTime)) {
			canPlaySfx = true;
		}
		
		if (canPlaySfx) {
			//DebugText.Log("PlayOneShot: " + sfx.name);
			sfxAudio.PlayOneShot(curAudioClip);
			
			lastAudioClip = curAudioClip;
			lastPlayTime = Time.time;
		}
	}
	
	static AudioClip GetAudioClip(AudioType audioType, String clipName) {
		AudioClip[] bufferClip = null;
		switch (audioType) {
			case AudioType.SFX:
				bufferClip = sfxClipArray;
				break;
			case AudioType.MUSIC:
				bufferClip = musicClipArray;
				break;
			case AudioType.BUTTON_SFX:
				bufferClip = buttonSfxClipArray;
				break;
			case AudioType.VO:
				bufferClip = voClipArray;
				break;
			default:
				DebugText.LogError ("Unknown type: " + audioType + " in GetAudioClip!");
				break;
		}
		
		if (bufferClip != null) {
			foreach (AudioClip each in bufferClip) {
				if (each.name == clipName) {
					return each;
				}
			}
		}
		if (audioType == AudioType.SFX) {			
			AudioClip returnClip = (AudioClip)Resources.Load("Audio/SFX/" + clipName, typeof(AudioClip));
			if (returnClip == null) 
			{ 				
				DebugText.LogError ("Request Play Clip: [" + "Audio/SFX/" + clipName + "] does not exists!");
			} 
			return returnClip;
		} else if (audioType == AudioType.VO) {
			AudioClip returnClip = (AudioClip)Resources.Load("Audio/VO/" + languagePathKey + "/" + clipName, typeof(AudioClip));
			if (returnClip == null)
			{
				DebugText.LogError ("Request Play Clip: [" + "Audio/VO/" + languagePathKey + "/" + clipName + "] does not exists!");
			}
			return returnClip;
		}
		DebugText.LogError ("Request Play Clip: " + clipName + " does not exists!");
		return null;
	}
	
	/*
	function Pause(isPause : boolean) {
		if (audio == null)
			return;
			
		if (isPause){
			if (audio.volume != 0.0) {
				tempVolume = audio.volume;
				audio.volume = 0.0;
			}
		} else {
			audio.volume = tempVolume;
		}
	}*/
	public static void RequestPlayVO(String clipName) {
		RequestPlayVO(GetAudioClip(AudioType.VO, clipName));
	}
	
	public static void RequestPlayVO(AudioClip curAudioClip) {
		if (!enableVO) {
			return;
		}
		voAudio.clip = curAudioClip;
		voAudio.Play();
	}
	public static bool IsPlayingSFX() {
		return (sfxAudio.isPlaying && playAndLoopSfxCounter == 0 && fadeSfxList.Count == 0);
	}
	
	public static void SetSfxVolume(float volume){
		sfxAudio.volume = volume;
		buttonSfxAudio.volume = volume;
	}
	
	public static float GetSfxVolume(){
		return sfxAudio.volume;	
	}
	
//	public static bool IsPlayingSFX(AudioClip clip){
//		if (sfxAudio.clip != clip)
//			return false;
//		else 
//			return IsPlayingSFX();
//	}
	
	public static bool IsPlayingVO() {
		return voAudio.isPlaying;
	}
	
	public static void StopSfx() {
		sfxAudio.Stop();
		if(fadeSfxList.Count > 0)
		{
			foreach(GameObject fadeSfx in fadeSfxList)
			{
				fadeSfx.GetComponent<AudioSource>().Stop();
			}
		}
	}
	public static void StopVO() {
		voAudio.Stop();
	}
	
	/*
	 * public IEnumerator PlayVO() {
	 * 	DebugText.Log("Before VO play");
	 * 	yield return StartCoroutine(AudioManager.RequestAndWaitPlayVO("ClipName"));
	 * 	DebugText.Log("After VO play");
	 * }
	 */
	public static IEnumerator RequestAndWaitPlayVO(String clipName) {
		yield return script.StartCoroutine(RequestAndWaitPlayVO(GetAudioClip(AudioType.VO, clipName)));
	}
	
	public static IEnumerator RequestAndWaitPlayVO(AudioClip curAudioClip) {
		if (!enableVO) {
			yield return 0;
		} else {
			if(curAudioClip == null)
			{
				DebugText.LogError("The audio clip is null.");
				yield break;
			}
			voAudio.clip = curAudioClip;
			voAudio.Play();
			yield return script.StartCoroutine(WaitVO());
		}
	}
	
	// overload delegate method
	public static IEnumerator RequestAndWaitPlayVO(String clipName, Action callbackFunction) {
		yield return script.StartCoroutine(RequestAndWaitPlayVO(GetAudioClip(AudioType.VO, clipName)));
		callbackFunction();
	}
	
	// overload delegate method
	/*
	 * public void PlayVO() {
	 * 	DebugText.Log("Before VO play 1");
	 * 	StartCoroutine(AudioManager.RequestPlayVO("ClipName", new Action(CallbackFunction)));
	 * 	DebugText.Log("Before VO play 2");
	 * }
	 * public void CallbackFunction() {
	 * 	DebugText.Log("After VO play");
	 * }
	 */
	public static IEnumerator RequestAndWaitPlayVO(AudioClip clipName, Action callbackFunction) {
		yield return script.StartCoroutine(RequestAndWaitPlayVO(clipName));
		callbackFunction();
	}
	
	public static IEnumerator WaitVO() {
		do{
			yield return new WaitForSeconds(0.1f);
		} while(AudioManager.IsPlayingVO());
	}
}
