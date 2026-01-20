using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boy
{
    public class PPESelector : MonoBehaviour
    {
        [Header("Settings")]
        public int maxSelectionCount = 7;

        [Header("UI References")]
        public TMP_Text selectionCountText;
        public Button confirmSelectionButton;

        public GameObject selectionWarningPanel;
        public GameObject incorrectSelectionPanel;
        public GameObject correctSelectionPanel;

        [Header("Runtime Data")]
        public List<PPEOption> selectedOptions = new();

        public Action<bool> OnSelectionValidated;

        private void Awake()
        {
            ResetSelection();
            confirmSelectionButton.onClick.AddListener(ValidateSelection);
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
                confirmSelectionButton.interactable = true;
                HideAllFeedbackPanels();
            });

            if (selectedOptions.Count < maxSelectionCount)
            {
                selectionWarningPanel.SetActive(true);
                return;
            }

            confirmSelectionButton.interactable = false;

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

            OnSelectionValidated?.Invoke(isSelectionCorrect);
        }

        public void UpdateSelectionCountUI()
        {
            selectionCountText.text =
                $"{selectedOptions.Count}/{maxSelectionCount}";
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
