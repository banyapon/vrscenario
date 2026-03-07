using Unity.Netcode;
using UnityEngine;
using System;

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

    public Action OnInspector;
    public Action OnPlayer;

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

    public override void OnNetworkSpawn()
    {
        _isInspector.OnValueChanged += OnRoleChanged;
        _isPlayer.OnValueChanged += OnPlayerChanged;
    }

    public override void OnNetworkDespawn()
    {
        _isInspector.OnValueChanged -= OnRoleChanged;
        _isPlayer.OnValueChanged -= OnPlayerChanged;
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
}