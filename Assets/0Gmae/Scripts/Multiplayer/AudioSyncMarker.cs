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

        source = GetComponent<AudioSource>();
        if (!source)
            source = gameObject.AddComponent<AudioSource>();

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
        print($"Play audio: {name}");
        Notify(AudioState.Play);
    }
    public void Pause() => Notify(AudioState.Pause);
    public void Stop() => Notify(AudioState.Stop);

    void Notify(AudioState state)
    {
        //ApplyState(state);
        controller?.NotifyAudioChange(this, state);
    }

    #endregion


    #region Apply (server only)

    public void ApplyState(AudioState state)
    {
        switch (state)
        {
            case AudioState.Play: source.Play(); break;
            case AudioState.Pause: source.Pause(); break;
            case AudioState.Stop: source.Stop(); break;
        }

    }

    public void SetMute(bool value)
    {
        print($"SetMute: {value}");
        if (source) source.mute = value;
    }

    #endregion
}
