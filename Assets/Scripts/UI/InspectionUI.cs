using UnityEngine;
using TMPro;
using Edgar.Dossier.Core;
using Edgar.Items.Core;

namespace Edgar.UI
{
    /// <summary>
    /// Manages the UI panel shown during item inspection.
    /// Wire up via InspectionManager's inspector field.
    /// </summary>
    public class InspectionUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        [Tooltip("Close button shown on mobile. Wire its OnClick to OnCloseButtonClicked.")]
        [SerializeField] private GameObject _closeButton;

        private void Awake()
        {
            Hide();
        }

        public void Show(ClueData data)
        {
            if (_titleText != null) _titleText.text = data.title;
            if (_descriptionText != null) _descriptionText.text = data.description;

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            if (_closeButton != null)
                _closeButton.SetActive(true);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;

            if (_closeButton != null)
                _closeButton.SetActive(false);
        }

        public void OnCloseButtonClicked()
        {
            InspectionManager.Instance.CloseInspection();
        }
    }
}