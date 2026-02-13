using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScenarioAudioController : NetworkBehaviour
{
    public AudioSource BGSource { get; private set; }
    public AudioSource SFXSource { get; private set; }

    [Header("Volume Setting")]
    [Range(0, 1)] public float BGVolume = 1;
    [Range(0, 1)] public float SFXVolume = 1;

    [Header("Audio Clips")]
    public AudioClip bgClip;
    public AudioClip buttonClip;

    readonly Dictionary<string, AudioSyncTarget> markers = new();

    bool isMuted;

    class AudioSyncTarget
    {
        public string id;
        public AudioSyncMarker marker;
    }

    #region Unity

    void Awake()
    {
        SetupSources();
    }

    public override void OnNetworkSpawn()
    {
        RegisterAllMarkers();
        UpdateMuteState();
    }

    //public override void OnOwnershipChanged(ulong prev, ulong current)
    //{
    //    UpdateMuteState();
    //}

    #endregion


    #region Setup
    void SetupSources()
    {
        BGSource = gameObject.AddComponent<AudioSource>();
        BGSource.playOnAwake = false;
        BGSource.loop = true;
        BGSource.volume = BGVolume;

        SFXSource = gameObject.AddComponent<AudioSource>();
        SFXSource.playOnAwake = false;
        SFXSource.loop = false;
        SFXSource.volume = SFXVolume;

        if (bgClip)
            BGSource.clip = bgClip;
    }

    void RegisterAllMarkers()
    {
        var all = GetComponentsInChildren<AudioSyncMarker>(true);
        foreach (var m in all)
            RegisterMarker(m);
    }

    #endregion

    #region Marker Registration

    public void RegisterMarker(AudioSyncMarker marker)
    {
        if (!markers.ContainsKey(marker.Id))
        {
            markers[marker.Id] = new AudioSyncTarget
            {
                id = marker.Id,
                marker = marker
            };
        }
    }

    public void UnregisterMarker(AudioSyncMarker marker)
    {
        markers.Remove(marker.Id);
    }

    #endregion

    #region Mute

    void UpdateMuteState()
    {
        SetMute(!IsOwner);
    }

    void SetMute(bool value)
    {
        if (isMuted == value) return;

        isMuted = value;

        BGSource.mute = value;
        SFXSource.mute = value;

        foreach (var t in markers.Values)
            t.marker.SetMute(value);
    }

    #endregion

    #region ===== Background Control (Owner -> Server) =====

    public void SetBackgroundClip(AudioClip clip, bool play = false)
    {
        if (!IsOwner || clip == null) return;

        RequestSetBGClipServerRpc(play);
        BGSource.clip = clip;
    }

    public void PlayBackground()
    {
        if (!IsOwner) return;

        RequestPlayBGServerRpc();
    }

    public void StopBackground()
    {
        if (!IsOwner) return;

        RequestStopBGServerRpc();
    }

    [ServerRpc]
    void RequestSetBGClipServerRpc(bool play, ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        if (play)
            ApplyPlayBG();
    }

    [ServerRpc]
    void RequestPlayBGServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        ApplyPlayBG();
    }

    [ServerRpc]
    void RequestStopBGServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        ApplyStopBG();
    }

    void ApplyPlayBG()
    {
        if (!BGSource.isPlaying)
            BGSource.Play();
    }

    void ApplyStopBG()
    {
        if (BGSource.isPlaying)
            BGSource.Stop();
    }

    #endregion

    #region ===== SFX Control (Owner -> Server) =====

    public void PlayButtonSFX()
    {
        if (!IsOwner || buttonClip == null) return;

        RequestPlayButtonServerRpc();
    }

    public void PlaySFX(AudioClip clip, float delay = 0f)
    {
        if (!IsOwner || clip == null) return;

        StartCoroutine(RequestPlaySFXDelayed(clip, delay));
    }

    IEnumerator RequestPlaySFXDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        RequestPlaySFXServerRpc();
    }

    [ServerRpc]
    void RequestPlayButtonServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        SFXSource.PlayOneShot(buttonClip);
    }

    [ServerRpc]
    void RequestPlaySFXServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        SFXSource.Play();
    }

    #endregion

    #region ===== Marker Notify =====

    public void NotifyAudioChange(AudioSyncMarker marker, AudioState state)
    {
        if (!IsOwner || !IsSpawned) return;

        RequestSetAudioServerRpc(marker.Id, state);
    }

    [ServerRpc]
    void RequestSetAudioServerRpc(string id, AudioState state, ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams)) return;

        ApplyAudio(id, state);
    }

    void ApplyAudio(string id, AudioState state)
    {
        if (!markers.TryGetValue(id, out var t) || t.marker == null)
            return;

        t.marker.ApplyState(state);
    }

    #endregion

    #region Utils

    bool IsSenderOwner(ServerRpcParams rpcParams)
    {
        return rpcParams.Receive.SenderClientId == OwnerClientId;
    }

    #endregion
}

public enum AudioState
{
    Stop,
    Play,
    Pause
}
