using Unity.Netcode;
using UnityEngine;

public class AudioSyncMarker : SyncMarker
{
    public bool playOnEnable;
    AudioSource source;
    SyncAudioController controller;

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        SetAudioSource();

        source.playOnAwake = false;
        controller = GetComponentInParent<SyncAudioController>();
        controller?.RegisterMarker(this);
    }

    private void OnEnable()
    {
        if (playOnEnable) Play();
    }

    void OnDestroy()
    {
        controller?.UnregisterMarker(this);
    }

    #endregion


    #region Public API

    public void Play()
    {
        //print($"Play audio: {name}");
        Notify(AudioState.Play);
    }
    public void Pause() => Notify(AudioState.Pause);
    public void Stop() => Notify(AudioState.Stop);

    void Notify(AudioState state)
    {
        ApplyState(state);
        controller?.NotifyAudioChange(this, state);
    }

    #endregion


    #region Apply (server only)

    public void ApplyState(AudioState state)
    {
        if (source == null)
        {
            SetAudioSource();
            Debug.LogWarning($"AudioSource NULL on {name}");
        }

        switch (state)
        {
            case AudioState.Play: source.Play(); break;
            case AudioState.Pause: source.Pause(); break;
            case AudioState.Stop: source.Stop(); break;
        }

    }

    public void SetMute(bool value)
    {
        SetAudioSource();
        if (source) source.mute = value;
    }

    #endregion

    void SetAudioSource()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!source) source = gameObject.AddComponent<AudioSource>();
    }
}
