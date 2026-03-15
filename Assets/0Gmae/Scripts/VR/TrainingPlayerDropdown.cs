using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainingPlayerDropdown : MonoBehaviour
{
    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    [Header("UI")]
    public TMP_Dropdown dropdown;
    public Button confirmButton;

    Dictionary<ulong, int> clientIndexMap = new();
    List<PlayerData> players = new();

    ulong selectedClientId = ulong.MaxValue;

    public ulong SelectedClientId {
        get => selectedClientId;
        set {
            selectedClientId = value;
            if (confirmButton) confirmButton.interactable = selectedClientId != ulong.MaxValue;
        }
    }

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        TrainingPlayerList.Instance.OnListChanged += SyncDropdown;

        InitialBuild();
    }

    void OnDestroy()
    {
        if (TrainingPlayerList.Instance != null)
            TrainingPlayerList.Instance.OnListChanged -= SyncDropdown;

        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirm);
    }

    void InitialBuild()
    {
        var list = TrainingPlayerList.Instance.players;

        dropdown.ClearOptions();
        players.Clear();
        clientIndexMap.Clear();

        List<string> options = new();

        for (int i = 0; i < list.Count; i++)
        {
            var p = list[i];

            players.Add(p);
            clientIndexMap[p.OwnerClientId] = i;

            options.Add(GetDisplayName(p));
        }

        dropdown.AddOptions(options);
    }

    void SyncDropdown()
    {
        var list = TrainingPlayerList.Instance.players;

        // REMOVE
        for (int i = players.Count - 1; i >= 0; i--)
        {
            var p = players[i];

            if (!list.Contains(p))
            {
                ulong id = p.OwnerClientId;

                dropdown.options.RemoveAt(i);
                players.RemoveAt(i);
                clientIndexMap.Remove(id);

                RebuildIndexMap();

                if (selectedClientId == id)
                {
                    SelectedClientId = ulong.MaxValue;
                }
            }
        }

        // ADD
        foreach (var p in list)
        {
            if (clientIndexMap.ContainsKey(p.OwnerClientId))
                continue;

            int index = players.Count;

            players.Add(p);
            clientIndexMap[p.OwnerClientId] = index;

            dropdown.options.Add(new TMP_Dropdown.OptionData(GetDisplayName(p)));
        }

        // UPDATE NAME
        foreach (var p in players)
        {
            int index = clientIndexMap[p.OwnerClientId];
            dropdown.options[index].text = HighlightText(p);
        }

        dropdown.RefreshShownValue();

        RestoreSelection();
    }

    string HighlightText(PlayerData p)
    {
        string name = GetDisplayName(p);

        if (p.OwnerClientId == selectedClientId)
        {
            string hex = ColorUtility.ToHtmlStringRGB(highlightColor);
            return $"<color=#{hex}>{name}</color>";
        }

        return name;
    }

    void RestoreSelection()
    {
        if (selectedClientId == ulong.MaxValue)
            return;

        if (clientIndexMap.TryGetValue(selectedClientId, out int index))
            dropdown.SetValueWithoutNotify(index);
        else
        {
            SelectedClientId = ulong.MaxValue;
        }
    }

    void RebuildIndexMap()
    {
        clientIndexMap.Clear();

        for (int i = 0; i < players.Count; i++)
            clientIndexMap[players[i].OwnerClientId] = i;
    }

    void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= players.Count)
            return;

        SelectedClientId = players[index].OwnerClientId;

        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        foreach (var p in players)
        {
            int index = clientIndexMap[p.OwnerClientId];
            dropdown.options[index].text = HighlightText(p);
        }

        dropdown.RefreshShownValue();
    }

    void OnConfirm()
    {
        if (selectedClientId == ulong.MaxValue)
        {
            Debug.Log("No player selected");
            return;
        }

        gameObject.SetActive(false);
        Debug.Log($"Selected OwnerId: {selectedClientId}------------------");
    }
    string GetDisplayName(PlayerData p)
    {
#if UNITY_EDITOR
        return $"{p.UserName} ({p.OwnerClientId})";
#else
    return p.UserName;
#endif
    }
}