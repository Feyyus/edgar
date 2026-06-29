using UnityEngine;
using TMPro;
using Edgar.Inspection.Core;

namespace Edgar.UI
{
    /// <summary>
    /// Manages the UI panel shown during item inspection.
    /// Displays title and description from InspectableData.
    /// </summary>
    public class InspectionUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private GameObject _closeButton;

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            Hide();
        }

        public void Show(InspectableData data)
        {
            Debug.Log($"[InspectionUI] Show called with data: {(data != null ? data.title : "null")}");

            if (data == null)
            {
                _titleText.text = "Unknown Object";
                _descriptionText.text = "";
                Debug.LogWarning("[InspectionUI] Show called with null InspectableData");
            }
            else
            {
                if (_titleText != null) _titleText.text = data.title;
                if (_descriptionText != null) _descriptionText.text = data.description;
            }

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            _closeButton?.SetActive(true);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _closeButton?.SetActive(false);
        }

        public void OnCloseButtonClicked()
        {
            InspectionManager.Instance.CloseInspection();
        }
    }
}
