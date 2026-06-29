using UnityEngine;

namespace Edgar.Items.Core
{
    public class InspectionViewportController : MonoBehaviour
    {
        [SerializeField] private Transform _inspectionAnchor;
        [SerializeField] private float _rotationSensitivity = 0.3f;
        [SerializeField] private float _zoomSensitivity = 0.05f;
        [SerializeField] private float _minZoom = 0.5f;
        [SerializeField] private float _maxZoom = 3f;

        private float _currentZoom = 1f;

        public void ResetView()
        {
            _currentZoom = 1f;
            if (_inspectionAnchor != null)
            {
                _inspectionAnchor.localScale = Vector3.one;
                _inspectionAnchor.rotation = Quaternion.identity;
            }
        }

        public void ApplyRotation(Vector2 delta)
        {
            if (_inspectionAnchor == null) return;

            var rotY = Quaternion.AngleAxis(-delta.x * _rotationSensitivity, Vector3.up);
            var rotX = Quaternion.AngleAxis(-delta.y * _rotationSensitivity, Vector3.right);
            _inspectionAnchor.rotation = rotY * rotX * _inspectionAnchor.rotation;
        }

        public void ApplyZoom(float delta)
        {
            if (_inspectionAnchor == null) return;

            _currentZoom = Mathf.Clamp(_currentZoom + delta * _zoomSensitivity, _minZoom, _maxZoom);
            _inspectionAnchor.localScale = Vector3.one * _currentZoom;
        }
    }
}
