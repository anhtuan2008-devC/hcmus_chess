using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // tao bien luu tru audio source
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;
    // tao bien luu tru audio clip
    public AudioClip musicClip;
    public AudioClip moveClip;
    public AudioClip winClip;
    public AudioClip selectClip;
    public AudioClip startGameClip;
    public AudioClip whiteTurnClip;
    public AudioClip blackTurnClip;
    public AudioClip whiteWinClip;
    public AudioClip blackWinClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }
    public void PlaySFX(AudioClip sfxClip)
    {
        vfxAudioSource.clip = sfxClip;
        vfxAudioSource.PlayOneShot(sfxClip);
    } 
}
