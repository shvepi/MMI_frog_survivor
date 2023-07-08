using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;
    [SerializeField]
    private AudioClip click, coin, death, bgMusic;
    [SerializeField]
    private AudioSource soundFX, bgMusicSource;

	// Use this for initialization
	void Awake ()
    {
        if (instance == null) instance = this;	
	}

    public void PlayClick()
    {
        soundFX.PlayOneShot(click);
    }

    public void PlayCoin()
    {
        soundFX.PlayOneShot(coin);
    }

    public void PlayDeath()
    {
        soundFX.PlayOneShot(death);
    }

    public void PlayMusic()
    {
        bgMusicSource.Play();
    }

}
