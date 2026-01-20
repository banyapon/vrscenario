using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource BGSource { get; private set; }
    public AudioSource SFXSource { get; private set; }

    [Header("Volume Setting")]
    public float BGVolume = 0.5f;
    public float SFXVolume = 0.5f;

    [Header("Audio Clips")]
    public AudioClip bgClip;
    public AudioClip buttonClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        //BGSource Setting
        BGSource = gameObject.AddComponent<AudioSource>();
        BGSource.playOnAwake = false;
        BGSource.loop = true;
        BGSource.volume = BGVolume;
        SetBackgroundClip(bgClip, true);

        //SFXSource Setting
        SFXSource = gameObject.AddComponent<AudioSource>();
        SFXSource.playOnAwake = false;
        SFXSource.loop = false;
        SFXSource.volume = SFXVolume;
    }
    public void SetBackgroundClip(AudioClip clip, bool play = false)
    {
        if (clip == null) return;
        StopBackground();
        BGSource.clip = clip;
        if (play) PlayBackground();
    }

    public void PlayBackground()
    {
        if (!BGSource.isPlaying) BGSource.Play();
    }
    public void StopBackground()
    {
        if (BGSource.isPlaying) BGSource.Stop();
    }
    public void PlayButtonSFX()
    {
        if (buttonClip) SFXSource.PlayOneShot(buttonClip);
    }
    public void PlaySFX(AudioClip clip, float timeToDelay = 0)
    {
        if (clip == null) return;
        StartCoroutine(PlaySFXDelayed(clip, timeToDelay));
    }

    private IEnumerator PlaySFXDelayed(AudioClip clip, float timeToDelay = 0)
    {
        yield return new WaitForSeconds(timeToDelay);
        SFXSource.PlayOneShot(clip);
    }
}
