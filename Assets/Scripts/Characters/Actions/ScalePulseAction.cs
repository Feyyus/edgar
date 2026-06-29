using UnityEngine;
using Edgar.Characters.Core;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Visual debug action that makes the character pulse/scale when clicked.
    /// Provides clear visual feedback for interaction testing.
    /// </summary>
    public class ScalePulseAction : MonoBehaviour, ICharacterAction
    {
        [Header("Pulse Settings")]
        [SerializeField] private float _pulseScale = 1.2f;
        [SerializeField] private float _pulseDuration = 0.5f;
        [SerializeField] private AnimationCurve _pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Transform _characterTransform;
        private Vector3 _originalScale;
        private float _pulseTimer;
        private bool _isPulsing;
        private bool _hasOriginalScale;

        public void Execute(Character character)
        {
            StartPulse(character);
        }

        void StartPulse(Character character)
        {
            _characterTransform = character.transform;

            // Only store original scale once to prevent accumulation
            if (!_hasOriginalScale)
            {
                _originalScale = _characterTransform.localScale;
                _hasOriginalScale = true;
            }

            _pulseTimer = 0f;
            _isPulsing = true;
        }

        void Update()
        {
            if (_isPulsing && _characterTransform != null)
            {
                _pulseTimer += Time.deltaTime;

                // Calculate pulse progress
                float progress = _pulseTimer / _pulseDuration;
                float curveValue = _pulseCurve.Evaluate(progress);

                // Calculate scale based on pulse curve - use smooth sine wave
                float scaleFactor = 1f + (_pulseScale - 1f) * Mathf.Sin(curveValue * Mathf.PI);
                _characterTransform.localScale = _originalScale * scaleFactor;

                // Reset after duration
                if (_pulseTimer >= _pulseDuration)
                {
                    _isPulsing = false;
                    _characterTransform.localScale = _originalScale;
                }
            }
        }

        void OnDisable()
        {
            // Reset scale when component is disabled
            if (_characterTransform != null && _hasOriginalScale)
            {
                _characterTransform.localScale = _originalScale;
            }
        }

        void OnDestroy()
        {
            // Reset scale when component is destroyed
            if (_characterTransform != null && _hasOriginalScale)
            {
                _characterTransform.localScale = _originalScale;
            }
        }
    }

}