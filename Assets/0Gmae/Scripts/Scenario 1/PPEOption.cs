using UnityEngine;
using UnityEngine.UI;

namespace Boy
{
    public class PPEOption : MonoBehaviour
    {
        [Header("Validation")]
        public bool isCorrectOption = true;

        [Header("UI References")]
        public GameObject selectedIndicator;
        public GameObject unselectedIndicator;

        private Button optionButton;
        private PPESelector ppeSelector;

        private void Awake()
        {
            ppeSelector = GetComponentInParent<PPESelector>();

            optionButton = GetComponent<Button>();
            optionButton.onClick.AddListener(() =>
            {
                ppeSelector.ToggleOptionSelection(this);
            });

            UpdateSelectionVisual(false);
        }

        public void UpdateSelectionVisual(bool isSelected)
        {
            selectedIndicator.SetActive(isSelected);
            unselectedIndicator.SetActive(!isSelected);
        }
    }
}
