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

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        TrainingPlayerList.Instance.OnListChanged += SyncDropdown;
    }

    void OnEnable()
    {
        InitialBuild();

        if (players.Count > 0)
        {
            dropdown.SetValueWithoutNotify(0);
            TrainingPlayerList.Instance.selectedClientId = players[0].OwnerClientId;
            UpdateHighlight();
        }
        else
        {
            TrainingPlayerList.Instance.selectedClientId = ulong.MaxValue;
        }
    }

    void OnDestroy()
    {
        if (TrainingPlayerList.Instance != null)
            TrainingPlayerList.Instance.OnListChanged -= SyncDropdown;

        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirm);
    }

    // -------------------- CORE --------------------

    void SyncDropdown()
    {
        if (dropdown.IsExpanded) dropdown.Hide();

        var list = TrainingPlayerList.Instance.players;

        bool wasEmptyBefore = players.Count == 0;
        ulong currentSelected = TrainingPlayerList.Instance.selectedClientId;

        // ---------------- REMOVE ----------------
        for (int i = players.Count - 1; i >= 0; i--)
        {
            var p = players[i];

            if (!list.Contains(p))
            {
                ulong removedId = p.OwnerClientId;

                dropdown.options.RemoveAt(i);
                players.RemoveAt(i);
                clientIndexMap.Remove(removedId);

                RebuildIndexMap();

                if (currentSelected == removedId)
                {
                    currentSelected = ulong.MaxValue;
                }
            }
        }

        // ---------------- ADD ----------------
        foreach (var p in list)
        {
            if (clientIndexMap.ContainsKey(p.OwnerClientId))
                continue;

            int index = players.Count;

            players.Add(p);
            clientIndexMap[p.OwnerClientId] = index;

            dropdown.options.Add(new TMP_Dropdown.OptionData(GetDisplayName(p)));

            if (wasEmptyBefore)
            {
                currentSelected = p.OwnerClientId;
                dropdown.SetValueWithoutNotify(index);
                wasEmptyBefore = false;
            }
        }

        if (currentSelected == ulong.MaxValue && players.Count > 0)
        {
            currentSelected = players[0].OwnerClientId;
            dropdown.SetValueWithoutNotify(0);
        }

        TrainingPlayerList.Instance.selectedClientId = currentSelected;

        // ---------------- UPDATE TEXT ----------------
        foreach (var p in players)
        {
            int index = clientIndexMap[p.OwnerClientId];
            dropdown.options[index].text = HighlightText(p);
        }

        dropdown.RefreshShownValue();

        RestoreSelection();
    }

    // -------------------- UI --------------------

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

    void RestoreSelection()
    {
        if (TrainingPlayerList.Instance.selectedClientId == ulong.MaxValue)
            return;

        if (clientIndexMap.TryGetValue(TrainingPlayerList.Instance.selectedClientId, out int index))
            dropdown.SetValueWithoutNotify(index);
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

        TrainingPlayerList.Instance.selectedClientId = players[index].OwnerClientId;

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

    string HighlightText(PlayerData p)
    {
        string name = GetDisplayName(p);

        if (p.OwnerClientId == TrainingPlayerList.Instance.selectedClientId)
        {
            string hex = ColorUtility.ToHtmlStringRGB(highlightColor);
            return $"<color=#{hex}>{name}</color>";
        }

        return name;
    }

    // -------------------- ACTION --------------------

    void OnConfirm()
    {
        if (TrainingPlayerList.Instance.selectedClientId == ulong.MaxValue)
        {
            Debug.Log("No player selected");
            return;
        }

        gameObject.SetActive(false);

        ulong targetId = TrainingPlayerList.Instance.selectedClientId;

        bool foundScenario = false;

        Scenario[] scenarios = FindObjectsByType<Scenario>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Scenario scenario in scenarios)
        {
            if (scenario.OwnerClientId == targetId)
            {
                scenario.InspectorSetup();
                foundScenario = true;
            }
        }

        VRManager[] managers = FindObjectsByType<VRManager>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (VRManager manager in managers)
        {
            if (manager.IsOwner)
            {
                manager.environment.SetActive(false);
            }

            if (manager.OwnerClientId == targetId)
            {
                if (foundScenario)
                    manager.OpenMock();
                else
                    manager.InspectorSetup();
            }
        }
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