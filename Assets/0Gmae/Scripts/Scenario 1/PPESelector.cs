using DG.Tweening;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boy
{
    public class PPESelector : MonoBehaviour
    {
        public Transform pPEGroup;

        [Header("Settings")]
        public int maxSelectionCount = 7;

        [Header("UI References")]
        public GameObject baseUI;
        public TMP_Text selectionCountText;
        public Button confirmSelectionButton;

        [Space(10)]
        public GameObject selectionWarningPanel;
        public GameObject incorrectSelectionPanel;
        public GameObject correctSelectionPanel;

        [Header("Runtime Data")]
        public List<PPEOption> selectedOptions = new();
        public List<GameObject> selectedUI = new();

        public Action<bool> OnSelectionValidated;
        NetworkOwnershipContext context;

        private void Awake()
        {
            ResetSelection();
            context = GetComponentInParent<NetworkOwnershipContext>();
            confirmSelectionButton.onClick.AddListener(ValidateSelection);
        }

        private void OnEnable()
        {
            baseUI.SetActive(true);
            ResetSelection();
            //if (pPEGroup) Utility.ShuffleChildren(pPEGroup);
        }

        private void Update()
        {
            UpdateSelectionCountUIOnServer();
        }

        public void ToggleOptionSelection(PPEOption option)
        {
            bool isSelected = selectedOptions.Contains(option);

            if (isSelected)
            {
                selectedOptions.Remove(option);
                option.UpdateSelectionVisual(false);
            }
            else if(CanSelectMore())
            {
                selectedOptions.Add(option);
                option.UpdateSelectionVisual(true);
            }

            UpdateSelectionCountUI();
        }

        Tween delay = null;
        public void ValidateSelection()
        {
            delay?.Kill();
            HideAllFeedbackPanels();

            delay = DOVirtual.DelayedCall(2, () =>
            {
                baseUI.SetActive(true);
                HideAllFeedbackPanels();
            });

            baseUI.SetActive(false);

            if (selectedOptions.Count < maxSelectionCount)
            {
                selectionWarningPanel.SetActive(true);
                return;
            }

            bool isSelectionCorrect = true;
            foreach (var option in selectedOptions)
            {
                if (!option.isCorrectOption)
                {
                    isSelectionCorrect = false;
                    break;
                }
            }

            incorrectSelectionPanel.SetActive(!isSelectionCorrect);
            correctSelectionPanel.SetActive(isSelectionCorrect);

            if (isSelectionCorrect) delay?.Kill();

            OnSelectionValidated?.Invoke(isSelectionCorrect);
        }

        public void UpdateSelectionCountUI()
        {
            selectionCountText.text =
                $"{selectedOptions.Count}/{maxSelectionCount}";
        }

        public void UpdateSelectionCountUIOnServer()
        {
            if (context == null || context.IsOwner) return;
            if (!context.IsServer) return;

            int count = 0;
            foreach (var s in selectedUI)
            {
                if (!s.activeInHierarchy) continue;
                count++;
            }
            selectionCountText.text = $"{count}/{maxSelectionCount}";
        }

        public bool CanSelectMore()
        {
            return selectedOptions.Count < maxSelectionCount;
        }

        public void HideAllFeedbackPanels()
        {
            selectionWarningPanel.SetActive(false);
            incorrectSelectionPanel.SetActive(false);
            correctSelectionPanel.SetActive(false);
        }
        public void ResetSelection()
        {
            foreach (var option in selectedOptions)
                option.UpdateSelectionVisual(false);

            selectedOptions.Clear();
            UpdateSelectionCountUI();
            HideAllFeedbackPanels();
        }
    }
}
