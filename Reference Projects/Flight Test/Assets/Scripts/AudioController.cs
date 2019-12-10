using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Constants
	public static AudioController instance;

	// Public Variables
	public AudioSource jukeboxMusic;
	public AudioSource jukeboxBattle;
	public AudioSource jukeboxGameWhistle;

    // Private Constants
    private static float MUSIC_FADE_TIME = .25f;
    private static float MUSIC_MAX_VOLUME = 0f; //.07f;
    private static float MUSIC_QUIET_VOLUME = MUSIC_MAX_VOLUME * .42f;
    private static float BATTLE_MAX_VOLUME = .085f;
    private static float BATTLE_QUIET_VOLUME = BATTLE_MAX_VOLUME * .42f;

    // Private Variables
    private float time = 0f;
    private float musicVolume = 0f;
    private bool battleMusicPlaying = false;


	/*--- Lifecycle Methods ---*/

    void Start() {

    	// Set Instance
        instance = this;

        // Setup Music
        jukeboxMusic.volume = musicVolume;
        jukeboxMusic.Play();

        // Setup Battle Music
        jukeboxBattle.volume = 0f;
        jukeboxBattle.Play();

    }

    void Update() {

    	// Update Time
        time += Time.deltaTime;

        // Handle Music Fade-In
        if (musicVolume < MUSIC_MAX_VOLUME) {
            musicVolume = Mathf.Clamp01((time - MUSIC_FADE_TIME) / MUSIC_FADE_TIME) * MUSIC_MAX_VOLUME;
            jukeboxMusic.volume = musicVolume;
        }
    }


    /*--- Public Methods ---*/

    public void setMusicVolumeQuiet() {
    	jukeboxMusic.volume = MUSIC_QUIET_VOLUME;
    	if (battleMusicPlaying) {
    		jukeboxBattle.volume = BATTLE_QUIET_VOLUME;
    	}
    }

    public void setMusicVolumeNormal() {
    	jukeboxMusic.volume = MUSIC_MAX_VOLUME;
    	if (battleMusicPlaying) {
    		jukeboxBattle.volume = BATTLE_MAX_VOLUME;
    	}
    }

    public void setBattleVolumeSilent() {
    	battleMusicPlaying = false;
    	jukeboxBattle.volume = 0f;
    }

    public void setBattleVolumeNormal() {
    	battleMusicPlaying = true;
    	jukeboxBattle.volume = BATTLE_MAX_VOLUME;
    }

    public void playGameWhistle() {
    	if (battleMusicPlaying) {
    		jukeboxGameWhistle.Play();
    	}
    }


    /*--- Utility Methods ---*/



}
