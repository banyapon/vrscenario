using UnityEngine;

public class AudioSyncMarker : SyncMarker
{
    AudioSource source;
    ScenarioAudioController controller;

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        source = GetComponent<AudioSource>();
        if (!source)
            source = gameObject.AddComponent<AudioSource>();

        controller = GetComponentInParent<ScenarioAudioController>();
        controller?.RegisterMarker(this);
    }

    void OnDestroy()
    {
        controller?.UnregisterMarker(this);
    }

    #endregion


    #region Public API

    public void Play() => Notify(AudioState.Play);
    public void Pause() => Notify(AudioState.Pause);
    public void Stop() => Notify(AudioState.Stop);

    void Notify(AudioState state)
    {
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
        source.mute = value;
    }

    #endregion
}
