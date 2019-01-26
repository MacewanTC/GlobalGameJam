using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioController : MonoBehaviour {
	public static AudioController instance = null;
	public PlayerState currentLocation = PlayerState.SAFE;
	
	public enum PlayerState {SAFE, EXPLORE, CAUTIOUS, DANGER};

	public AudioSource exploreSource, safeSource, cautiousSource, dangerSource;

	public AudioSource explore { get; private set; }
	public AudioSource safe { get; private set; }
	public AudioSource cautious { get; private set; }
	public AudioSource danger { get; private set; }
	public AudioSource game_sfx, player_sfx, ai_sfx;

	public float raiseVolBy = 0.1f, lowerVolBy = 0.01f;

	[Tooltip("This boolean determines whether both AudioSources will loop.  Only works prior to runtime.")]
	public bool loop = true;
	[Tooltip("While the game is running, press to apply the normalVol variable on the Normal Audio volume.")]
	public bool applyMusicVolume = false;
	[Tooltip("While the game is running, press on applyNormal to apply the normalVol variable on the Normal Audio volume.")]
	public float musicVolume = 1f;
	[Tooltip("This boolean determines if StartBass and EndBass can be used.")]
	private bool mute = false;

	public float generalSFXVolume = 0.4f;
	public float gameSFXVolume = 1f;

	public AudioClip defeatSFX, gameStartSFX, 
		doorOpenSFX, doorCloseSFX, 
		footstepSFX, pickupSFX;

	public enum SFXstatus { READY, ENTRY, EXIT };
	public SFXstatus soundStatus { get; private set; }
	public float sfxLength = 0.4f;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else if(instance != this) {
			Destroy(this.gameObject);
			return;
		}
	}

	// Use this for initialization
	void Start() {
		//Debug.Log ("AudioController Started");
		if(safe == null) safe = safeSource;
		if(explore == null) explore = exploreSource;
		if(cautious == null) cautious = cautiousSource;
		if(danger == null) danger = dangerSource;
		
		setAllMusicVolume(0f);
		
		soundStatus = SFXstatus.READY;
	}

	public void Update() {
		//Find our location

		updateMusicVolume(currentLocation);
	}

	public void updateMusicVolume(PlayerState biome) {
		// Raise Volume for the biome we are in
		raiseBiomeVolume(biome, raiseVolBy+lowerVolBy);

		// Lower Volume for biomes we are not in
		lowerAllBiomeVolume(biome, lowerVolBy);

		// applyMusicVolume
		if (applyMusicVolume) {
			raiseBiomeVolume(biome, musicVolume);
			applyMusicVolume = false;
		}
	}

	private void setAllMusicVolume(float vol) {
		safe.volume = vol;
		explore.volume = vol;
		cautious.volume = vol;
		danger.volume = vol;
	}

	private AudioSource lastSrc = null;
	private void raiseVolume(AudioSource src, float vol) {
		if(src.volume == 1f) return;
		if(!src.isPlaying) src.Play();

		if(lastSrc != null && !lastSrc.Equals(src)) {
			src.time = lastSrc.time;
		}
		
		src.volume += vol;

		lastSrc = src;
	}

	private void lowerVolume(AudioSource src, float vol) {
		if(!src.isPlaying) return;

		src.volume -= vol;

		if(src.volume == 0) src.Stop();
	}

	private void raiseBiomeVolume(PlayerState biome, float vol) {
		if (biome == PlayerState.SAFE) {
			raiseVolume(safe, vol);
		} else if (biome == PlayerState.EXPLORE) {
			raiseVolume(explore, vol);
		} else if (biome == PlayerState.CAUTIOUS) {
			raiseVolume(cautious, vol);
		} else if (biome == PlayerState.DANGER) {
			raiseVolume(danger, vol);
		}
	}

	private void lowerAllBiomeVolume(PlayerState currentBiome, float vol) {
		lowerVolume(safe, vol);
		lowerVolume(explore, vol);
		lowerVolume(cautious, vol);
		lowerVolume(danger, vol);
	}

	private float overallVolume = 1.0f;
	public void SetOverallVolume(float volume) {
		overallVolume = volume;
		AudioListener.volume = overallVolume;
	}

	public float GetOverallVolume() {
		return overallVolume;
	}

	public void SetMusicVolume(float volume) {
		musicVolume = volume;
		applyMusicVolume = true;
	}

	public float GetMusicVolume() {
		return musicVolume;
	}

	public void SetGeneralSFXVolume(float volume) {
		generalSFXVolume = volume;
	}

	public float GetGeneralSFXVolume() {
		return generalSFXVolume;
	}

	public void SetInGameSFXVolume(float volume) {
		gameSFXVolume = volume;
	}

	public float GetGameSFXVolume() {
		return gameSFXVolume;
	}

	public void StartSongs () {
		safe.time = 0;
		explore.time = 0;
		cautious.time = 0;
		danger.time = 0;

		safe.Play();
		explore.Play();
		cautious.Play();
		danger.Play();
	}

	public void PlayPickupSFX() {
		game_sfx.volume = gameSFXVolume;
		game_sfx.PlayOneShot(pickupSFX);
	}

	IEnumerator WaitForRealSeconds(float time) {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
			yield return null;
	}

	IEnumerator WaitUntilSFXReady() {
		while (soundStatus != SFXstatus.READY)
			yield return null;
	}

	public void OnDeath(string scene) {
		setAllMusicVolume(0f);

		game_sfx.volume = gameSFXVolume;
		game_sfx.PlayOneShot(defeatSFX);

		StartCoroutine (LoadAfterTime(defeatSFX.length+0.1f, scene));
	}

	IEnumerator LoadAfterTime(float time, string scene) {
		yield return StartCoroutine(WaitForRealSeconds(time));

		//SceneManager.LoadScene(scene);
	}
}
