using UnityEngine;
using Edgar.Dossier.Core;
using Edgar.Interaction;
using Edgar.Items.Input;
using Edgar.UI;
using Edgar.Characters.Core;

namespace Edgar.Items.Core
{
    /// <summary>
    /// Singleton that owns the item inspection state machine.
    /// Spawns a copy of the clicked item on the InspectionLayer, handles rotation/zoom,
    /// and coordinates enabling/disabling of other input systems.
    /// </summary>
    public class InspectionManager : MonoBehaviour
    {
        public static InspectionManager Instance { get; private set; }

        [Header("Scene References")]
        [SerializeField] private Transform _inspectionAnchor;
        [SerializeField] private InspectionUI _inspectionUI;
        [SerializeField] private InspectionInputHandler _inputHandler;
        [SerializeField] private NavigationUI _navigationUI;

        [Header("Inspection Settings")]
        [SerializeField] private float _rotationSensitivity = 0.3f;
        [SerializeField] private float _zoomSensitivity = 0.05f;
        [SerializeField] private float _minZoom = 0.5f;
        [SerializeField] private float _maxZoom = 3f;

        [Header("Original Item Behavior")]
        [SerializeField] private InspectionOriginalBehavior _originalBehavior = InspectionOriginalBehavior.Hide;

        private InspectableObject _currentItem;
        private GameObject _inspectionCopy;
        private float _currentZoom = 1f;

        public bool IsInspecting => _currentItem != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_inputHandler != null)
                _inputHandler.enabled = false;
        }

        public void OpenInspection(InspectableObject item)
        {
            if (IsInspecting) return;

            _currentItem = item;
            _currentZoom = 1f;
            _inspectionAnchor.localScale = Vector3.one;
            _inspectionAnchor.rotation = Quaternion.identity;

            _inspectionCopy = SpawnCopy(item);

            if (_originalBehavior == InspectionOriginalBehavior.Hide)
                item.gameObject.SetActive(false);

            if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = false;
            if (_navigationUI != null) _navigationUI.enabled = false;
            if (_inputHandler != null) _inputHandler.enabled = true;

            _inspectionUI.Show(null);

            // Fire actions (if any)
            // item.FireActions(InspectionTrigger.OnOpen); // Keep this if you have actions
        }

        public void CloseInspection()
        {
            if (!IsInspecting) return;

            // item.FireActions(InspectionTrigger.OnClose);
            _currentItem.EndInspection();

            if (_originalBehavior == InspectionOriginalBehavior.Hide)
                _currentItem.gameObject.SetActive(true);

            Destroy(_inspectionCopy);
            _inspectionCopy = null;

            if (_inputHandler != null) _inputHandler.enabled = false;
            if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = true;
            if (_navigationUI != null) _navigationUI.enabled = true;

            _inspectionUI.Hide();
            _currentItem = null;
            _inspectionAnchor.localScale = Vector3.one;
            _inspectionAnchor.rotation = Quaternion.identity;
        }

        public void ApplyRotation(Vector2 delta)
        {
            if (_inspectionCopy == null) return;

            var rotY = Quaternion.AngleAxis(-delta.x * _rotationSensitivity, Vector3.up);
            var rotX = Quaternion.AngleAxis(-delta.y * _rotationSensitivity, Vector3.right);
            _inspectionAnchor.rotation = rotY * rotX * _inspectionAnchor.rotation;
        }

        public void ApplyZoom(float delta)
        {
            _currentZoom = Mathf.Clamp(_currentZoom + delta * _zoomSensitivity, _minZoom, _maxZoom);
            _inspectionAnchor.localScale = Vector3.one * _currentZoom;
        }

        private GameObject SpawnCopy(InspectableObject item)
        {
            GameObject copy;

            // ✅ Use InspectionPrefab from the component
            if (item.InspectionPrefab != null)
            {
                copy = Instantiate(item.InspectionPrefab, _inspectionAnchor.position, Quaternion.identity, _inspectionAnchor);
            }
            else
            {
                copy = Instantiate(item.gameObject, _inspectionAnchor.position, Quaternion.identity, _inspectionAnchor);

                var billboard = copy.GetComponent<BillboardSprite>();
                if (billboard != null) Destroy(billboard);

                var sr = copy.GetComponent<SpriteRenderer>();
                if (sr != null) ConvertSpriteToQuad(copy, sr);
            }

            // Strip components that have no purpose on the copy
            foreach (var col in copy.GetComponents<Collider>())
                Destroy(col);

            var itemComp = copy.GetComponent<InspectableObject>();
            if (itemComp != null) Destroy(itemComp);

            var rb = copy.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            // Strip visual indicators
            var outline = copy.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
                Destroy(outline);
            }

            var icon = copy.GetComponent<InteractableIcon>();
            if (icon != null) Destroy(icon);

            var iconPivot = copy.transform.Find("_InteractableIcon");
            if (iconPivot != null) Destroy(iconPivot.gameObject);

            SetLayerRecursive(copy, LayerMask.NameToLayer("InspectionLayer"));

            // ✅ Use InspectionScale from the component
            copy.transform.localScale = item.InspectionScale;

            CenterOnAnchor(copy);

            return copy;
        }

        private void CenterOnAnchor(GameObject copy)
        {
            var renderers = copy.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            var bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            copy.transform.position += _inspectionAnchor.position - bounds.center;
        }

        private void ConvertSpriteToQuad(GameObject copy, SpriteRenderer sr)
        {
            var sprite = sr.sprite;
            Destroy(sr);

            if (sprite == null) return;

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<MeshCollider>());
            quad.transform.SetParent(copy.transform, false);

            float aspect = sprite.rect.width / sprite.rect.height;
            quad.transform.localScale = new Vector3(aspect, 1f, 1f);

            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.mainTexture = sprite.texture;
            quad.GetComponent<MeshRenderer>().material = mat;

            SetLayerRecursive(quad, LayerMask.NameToLayer("InspectionLayer"));
        }

        private void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }

    public enum InspectionOriginalBehavior
    {
        Hide,
        Highlight,
        Nothing
    }
}