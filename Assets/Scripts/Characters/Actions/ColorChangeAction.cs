using UnityEngine;
using Edgar.Characters.Core;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Visual debug action that temporarily changes the character's color when clicked.
    /// Provides immediate visual feedback for interaction testing.
    /// </summary>
    public class ColorChangeAction : MonoBehaviour, ICharacterAction
    {
        [Header("Color Settings")]
        [SerializeField] private Color _highlightColor = Color.red;
        [SerializeField] private float _highlightDuration = 0.5f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private float _highlightTimer;
        private bool _isHighlighting;
        private bool _hasOriginalColor;

        public void Execute(Character character)
        {
            StartHighlight(character);
        }

        void StartHighlight(Character character)
        {
            // Get the sprite renderer
            _spriteRenderer = character.GetComponent<SpriteRenderer>();

            if (_spriteRenderer == null)
            {
                Debug.LogWarning("[ColorChangeAction] No SpriteRenderer found on character");
                return;
            }

            // Store original color only once to prevent accumulation
            if (!_hasOriginalColor)
            {
                _originalColor = _spriteRenderer.color;
                _hasOriginalColor = true;
            }

            // Reset timer and start highlighting
            _highlightTimer = 0f;
            _isHighlighting = true;

            // Apply highlight color
            _spriteRenderer.color = _highlightColor;
        }

        void Update()
        {
            if (_isHighlighting && _spriteRenderer != null)
            {
                _highlightTimer += Time.deltaTime;

                // Calculate fade progress
                float progress = _highlightTimer / _highlightDuration;

                // Fade back to original color
                if (progress > 0.5f)
                {
                    float fadeProgress = (progress - 0.5f) / 0.5f;
                    _spriteRenderer.color = Color.Lerp(_highlightColor, _originalColor, fadeProgress);
                }

                // Reset after duration
                if (_highlightTimer >= _highlightDuration)
                {
                    _isHighlighting = false;
                    _spriteRenderer.color = _originalColor;
                }
            }
        }

        void OnDisable()
        {
            // Reset color when component is disabled
            if (_spriteRenderer != null && _hasOriginalColor)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        void OnDestroy()
        {
            // Reset color when component is destroyed
            if (_spriteRenderer != null && _hasOriginalColor)
            {
                _spriteRenderer.color = _originalColor;
            }
        }
    }

}