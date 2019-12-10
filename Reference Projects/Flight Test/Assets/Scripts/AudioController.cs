using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Constants
	public static AudioController instance;

	// Public Variables
	public AudioSource jukeboxMusic;

    // Private Constants
    private static float MUSIC_FADE_TIME = .25f;
    private static float MUSIC_MAX_VOLUME = .07f;
    private static float MUSIC_QUIET_VOLUME = MUSIC_MAX_VOLUME * .33f;

    // Private Variables
    private float time = 0f;
    private float musicVolume = 0f;


	/*--- Lifecycle Methods ---*/

    void Start() {

    	// Set Instance
        instance = this;

        // Play Music
        jukeboxMusic.volume = musicVolume;
        jukeboxMusic.Play();

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
    }

    public void setMusicVolumeNormal() {
    	jukeboxMusic.volume = MUSIC_MAX_VOLUME;
    }


    /*--- Utility Methods ---*/

    

}
