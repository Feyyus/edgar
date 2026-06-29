using UnityEngine;
using Edgar.Characters.Core;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Visual debug action that shows a floating text popup above the character when clicked.
    /// Useful for testing character interactions with visible in-game feedback.
    /// </summary>
    public class TextPopupAction : MonoBehaviour, ICharacterAction
    {
        [Header("Popup Settings")]
        [SerializeField] private string _popupText = "Clicked!";
        [SerializeField] private Color _textColor = Color.yellow;
        [SerializeField] private float _popupDuration = 2f;
        [SerializeField] private float _floatSpeed = 1f;
        [SerializeField] private float _floatDistance = 1f;
        [SerializeField] private int _fontSize = 24;

        private GameObject _popupObject;
        private TextMesh _textMesh;
        private float _popupTimer;
        private Vector3 _startPosition;
        private bool _isShowingPopup;

        public void Execute(Character character)
        {
            ShowPopup(character.transform.position);
        }

        void ShowPopup(Vector3 position)
        {
            // Create popup object if it doesn't exist
            if (_popupObject == null)
            {
                CreatePopupObject();
            }

            // Reset popup state - this will restart the animation if already showing
            _popupTimer = 0f;
            _isShowingPopup = true;

            // Position the popup above the character
            _popupObject.transform.position = position + Vector3.up * 0.5f;
            _startPosition = _popupObject.transform.position;

            // Set text content
            if (_textMesh != null)
            {
                _textMesh.text = _popupText;
                _textMesh.color = _textColor;
                _textMesh.fontSize = _fontSize;
                _textMesh.characterSize = 0.1f;
                _textMesh.anchor = TextAnchor.MiddleCenter;
            }

            // Reset alpha
            if (_textMesh != null)
            {
                Color currentColor = _textMesh.color;
                currentColor.a = 1f;
                _textMesh.color = currentColor;
            }

            // Activate the popup
            _popupObject.SetActive(true);
        }

        void CreatePopupObject()
        {
            _popupObject = new GameObject("TextPopup");
            _textMesh = _popupObject.AddComponent<TextMesh>();

            // Configure TextMesh
            if (_textMesh != null)
            {
                _textMesh.alignment = TextAlignment.Center;
                _textMesh.fontStyle = FontStyle.Bold;
            }

            // Make sure it's initially inactive
            _popupObject.SetActive(false);
        }

        void Update()
        {
            if (_isShowingPopup && _popupObject != null && _popupObject.activeInHierarchy)
            {
                _popupTimer += Time.deltaTime;

                // Calculate floating animation
                float progress = _popupTimer / _popupDuration;
                float yOffset = Mathf.Sin(progress * Mathf.PI) * _floatDistance;

                // Update position - always start from current base position
                Vector3 newPosition = _startPosition + Vector3.up * (yOffset * _floatSpeed);
                _popupObject.transform.position = newPosition;

                // Fade out towards the end
                if (progress > 0.7f)
                {
                    float alpha = 1f - ((progress - 0.7f) / 0.3f);
                    if (_textMesh != null)
                    {
                        Color currentColor = _textMesh.color;
                        currentColor.a = alpha;
                        _textMesh.color = currentColor;
                    }
                }

                // Hide after duration
                if (_popupTimer >= _popupDuration)
                {
                    _isShowingPopup = false;
                    _popupObject.SetActive(false);
                }
            }
        }

        void OnDisable()
        {
            // Hide popup when component is disabled
            if (_popupObject != null)
            {
                _popupObject.SetActive(false);
                _isShowingPopup = false;
            }
        }

        void OnDestroy()
        {
            if (_popupObject != null)
            {
                Destroy(_popupObject);
            }
        }
    }

}