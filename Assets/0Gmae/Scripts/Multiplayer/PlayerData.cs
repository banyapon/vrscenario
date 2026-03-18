using Unity.Netcode;
using UnityEngine;
using System;
using Unity.Collections;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<bool> _isInspector = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> _isPlayer = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString64Bytes> _userName = new NetworkVariable<FixedString64Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public Action OnInspector;
    public Action OnPlayer;
    public Action OnNameChanged;
    public Action OnDespawn;

    public bool IsInspector
    {
        get => _isInspector.Value;
        set => _isInspector.Value = value;
    }

    public bool IsPlayer
    {
        get => _isPlayer.Value;
        set => _isPlayer.Value = value;
    }

    public string UserName
    {
        get => _userName.Value.ToString();
        set => _userName.Value = value;
    }

    public override void OnNetworkSpawn()
    {
        _isInspector.OnValueChanged += OnRoleChanged;
        _isPlayer.OnValueChanged += OnPlayerChanged;
        _userName.OnValueChanged += OnNameValueChanged;

        TrainingPlayerList.Instance?.Register(this);
    }

    public override void OnNetworkDespawn()
    {
        _isInspector.OnValueChanged -= OnRoleChanged;
        _isPlayer.OnValueChanged -= OnPlayerChanged;
        _userName.OnValueChanged -= OnNameValueChanged;

        OnDespawn?.Invoke();
    }

    void OnRoleChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Inspector: {newValue}");
        if (newValue) OnInspector?.Invoke();
    }

    void OnPlayerChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Player: {newValue}");
        if (newValue) OnPlayer?.Invoke();
    }

    void OnNameValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        Debug.Log($"Name: {newValue}");
        OnNameChanged?.Invoke();
    }
}