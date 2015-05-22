using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This script will set the Default value to AudioManager if have.
 * Attach to don't destroy object with Audio Source to provide fade in/out audio and continue audio while load level.
 */
public class AudioManagerScript : MonoBehaviour {
//	[HideInInspector]
//	public Dictionary<AudioType, List<CustomAudioSource>
	
	// button sfx attributes
	private AudioSource buttonSfxAudio;
	public AudioClip[] buttonSfxClipArray = null;
	
	// sfx attributes
	private AudioSource sfxAudio;
	public AudioClip[] sfxClipArray = null;
	public float otherClipDelayTime = 0.0f;
	public float sameClipDelayTime = 1.0f;
	
	// music attributes
	private AudioSource musicAudio;
	public AudioClip[] musicClipArray = null;
	
	// vo attributes
	private AudioSource voAudio;
	public AudioClip[] voClipArray = null;
	
	/**
		\fn	void Awake ( )
		\brief	Awake function inherit from MonoBehaviour
	*/
	void Awake () {
		buttonSfxAudio = gameObject.AddComponent<AudioSource>();
		sfxAudio = gameObject.AddComponent<AudioSource>();
		voAudio = gameObject.AddComponent<AudioSource>();
		musicAudio = gameObject.AddComponent<AudioSource>();
		
		if (buttonSfxAudio != null) {
			AudioManager.SetAudioSource(AudioType.BUTTON_SFX, buttonSfxAudio);
		}
		if (buttonSfxClipArray != null) {
			AudioManager.SetAudioClip(AudioType.BUTTON_SFX, buttonSfxClipArray);
		}

		if (sfxAudio != null) {
			AudioManager.SetAudioSource(AudioType.SFX, sfxAudio);
		}
		if (sfxClipArray != null) {
			AudioManager.SetAudioClip(AudioType.SFX, sfxClipArray);
		}
		if (otherClipDelayTime != 0.0f && AudioManager.otherClipDelayTime == 0.0f) {
			AudioManager.otherClipDelayTime = otherClipDelayTime;
		}
		if (sameClipDelayTime != 1.0f && AudioManager.sameClipDelayTime == 1.0f) {
			AudioManager.sameClipDelayTime = sameClipDelayTime;
		}
	
		if (musicAudio != null) {
			AudioManager.SetAudioSource(AudioType.MUSIC, musicAudio);
		}
		if (musicClipArray != null) {
			AudioManager.SetAudioClip(AudioType.MUSIC, musicClipArray);
		}

		if (voAudio != null) {
			AudioManager.SetAudioSource(AudioType.VO, voAudio);
		}
		if (voClipArray != null) {
			AudioManager.SetAudioClip(AudioType.VO, voClipArray);
		}
	}
	
	// Remember to release the unuse assets
	void OnLevelWasLoaded(int level) {
		Resources.UnloadUnusedAssets();
		AudioManager.CleanLoopAndFadeSfx();
	}
	
	/**
		\fn	void	Update ( )
		\brief	Update function inherit from MonoBehaviour
	*/
	void Update () {
		AudioManager.Update();
	}
}