using UnityEngine;
using Edgar.Core;

namespace Edgar.UI
{
    /// <summary>
    /// Displays a floating, camera-facing icon above any interactable object.
    /// Add to the same GameObject as Character or InspectableObject.
    /// </summary>
    public class InteractableIcon : MonoBehaviour
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _heightPadding = 0.25f;
        [SerializeField] private float _bobSpeed = 1.5f;
        [SerializeField] private float _bobAmount = 0.06f;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private float _scale = 0.4f;

        private Transform _pivot;
        private float _baseHeight;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _baseHeight = ComputeTopY() + _heightPadding;

            var pivotGO = new GameObject("_InteractableIcon");
            pivotGO.transform.SetParent(transform, false);
            _pivot = pivotGO.transform;
            _pivot.localScale = Vector3.one * _scale;

            var sr = pivotGO.AddComponent<SpriteRenderer>();
            sr.sprite = _icon;
            sr.color = _color;
            sr.sortingOrder = 10;

            var interactable = GetComponent<IInteractable>();
            if (interactable != null)
                pivotGO.SetActive(interactable.IsInteractive);
        }

        private void LateUpdate()
        {
            if (_pivot == null) return;

            float bob = Mathf.Sin(Time.time * _bobSpeed) * _bobAmount;
            _pivot.position = transform.position + Vector3.up * (_baseHeight + bob);

            if (_cam != null)
                _pivot.rotation = _cam.transform.rotation;
        }

        private float ComputeTopY()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return 1f;

            var bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            return bounds.max.y - transform.position.y;
        }
    }
}