using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlayerList : MonoBehaviour
{
    public static TrainingPlayerList Instance;
    public bool isSelected;
    public ulong selectedClientId = ulong.MaxValue;

    public List<PlayerData> players = new();

    public Action OnListChanged;

    void Awake()
    {
        Instance = this;
    }

    public void Register(PlayerData data)
    {
        data.OnPlayer += () => AddPlayer(data);
        data.OnInspector += () => RemovePlayer(data);
        data.OnNameChanged += () => NotifyChange();
        data.OnDespawn += () => RemovePlayer(data);

        if (data.IsPlayer)
            AddPlayer(data);
    }

    void AddPlayer(PlayerData data)
    {
        if (players.Contains(data))
            return;

        players.Add(data);
        NotifyChange();
    }

    void RemovePlayer(PlayerData data)
    {
        if (!players.Contains(data))
            return;

        players.Remove(data);
        NotifyChange();
    }

    void NotifyChange()
    {
        OnListChanged?.Invoke();
    }
}