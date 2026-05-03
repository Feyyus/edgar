using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton that owns the item inspection state machine.
/// Spawns a copy of the clicked item on the InspectionLayer, handles rotation/zoom,
/// and coordinates enabling/disabling of other input systems.
/// </summary>
public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private Transform inspectionAnchor;
    [SerializeField] private InspectionUI inspectionUI;
    [SerializeField] private InspectionInputHandler inputHandler;
[SerializeField] private NavigationUI navigationUI;

    [Header("Inspection Settings")]
    [SerializeField] private float rotationSensitivity = 0.3f;
    [SerializeField] private float zoomSensitivity = 0.05f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 3f;

    [Header("Original Item Behavior")]
    [SerializeField] private InspectionOriginalBehavior originalBehavior = InspectionOriginalBehavior.Hide;

    private InspectableItem _currentItem;
    private GameObject _inspectionCopy;
    private float _currentZoom = 1f;

    public bool IsInspecting => _currentItem != null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // InputHandler starts disabled — enabled only during inspection
        if (inputHandler != null)
            inputHandler.enabled = false;
    }

    public void OpenInspection(InspectableItem item)
    {
        if (IsInspecting) return;

        _currentItem = item;
        _currentZoom = 1f;
        inspectionAnchor.localScale = Vector3.one;
        inspectionAnchor.rotation = Quaternion.identity;

        _inspectionCopy = SpawnCopy(item);

        // Hide or highlight the original
        if (originalBehavior == InspectionOriginalBehavior.Hide)
            item.gameObject.SetActive(false);

        // Hand off input
        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = false;
        if (navigationUI != null) navigationUI.enabled = false;
        if (inputHandler != null) inputHandler.enabled = true;

        inspectionUI.Show(item.Data);
        item.FireActions(InspectionTrigger.OnOpen);
    }

    public void CloseInspection()
    {
        if (!IsInspecting) return;

        _currentItem.FireActions(InspectionTrigger.OnClose);
        _currentItem.EndInspection();

        // Restore original
        if (originalBehavior == InspectionOriginalBehavior.Hide)
            _currentItem.gameObject.SetActive(true);

        Destroy(_inspectionCopy);
        _inspectionCopy = null;

        // Restore input
        if (inputHandler != null) inputHandler.enabled = false;
        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = true;
        if (navigationUI != null) navigationUI.enabled = true;

        inspectionUI.Hide();
        _currentItem = null;
        inspectionAnchor.localScale = Vector3.one;
        inspectionAnchor.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Called by InspectionInputHandler every frame while dragging.
    /// </summary>
    public void ApplyRotation(Vector2 delta)
    {
        if (_inspectionCopy == null) return;

        var rotY = Quaternion.AngleAxis(-delta.x * rotationSensitivity, Vector3.up);
        var rotX = Quaternion.AngleAxis(-delta.y * rotationSensitivity, Vector3.right);
        inspectionAnchor.rotation = rotY * rotX * inspectionAnchor.rotation;
    }

    /// <summary>
    /// Called by InspectionInputHandler on scroll or pinch.
    /// </summary>
    public void ApplyZoom(float delta)
    {
        _currentZoom = Mathf.Clamp(_currentZoom + delta * zoomSensitivity, minZoom, maxZoom);
        inspectionAnchor.localScale = Vector3.one * _currentZoom;
    }

    private GameObject SpawnCopy(InspectableItem item)
    {
        GameObject copy;

        if (item.Data.meshPrefab != null)
        {
            copy = Instantiate(item.Data.meshPrefab, inspectionAnchor.position, Quaternion.identity, inspectionAnchor);
        }
        else
        {
            copy = Instantiate(item.gameObject, inspectionAnchor.position, Quaternion.identity, inspectionAnchor);

            // Strip BillboardSprite — the player controls rotation manually
            var billboard = copy.GetComponent<BillboardSprite>();
            if (billboard != null) Destroy(billboard);

            // Convert SpriteRenderer to a 3D quad so it rotates freely in 3D
            var sr = copy.GetComponent<SpriteRenderer>();
            if (sr != null) ConvertSpriteToQuad(copy, sr);
        }

        // Strip components that have no purpose on the copy
        foreach (var col in copy.GetComponents<Collider>())
            Destroy(col);

        var itemComp = copy.GetComponent<InspectableItem>();
        if (itemComp != null) Destroy(itemComp);

        var rb = copy.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        foreach (var action in copy.GetComponents<MonoBehaviour>())
        {
            if (action is IItemAction)
                Destroy(action);
        }

        // Strip visual indicators — outline and floating icon
        var outline = copy.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // triggers OnDisable → strips materials immediately
            Destroy(outline);
        }

        var icon = copy.GetComponent<InteractableIcon>();
        if (icon != null) Destroy(icon);

        var iconPivot = copy.transform.Find("_InteractableIcon");
        if (iconPivot != null) Destroy(iconPivot.gameObject);

        SetLayerRecursive(copy, LayerMask.NameToLayer("InspectionLayer"));
        copy.transform.localScale = item.Data.inspectionScale;

        // Offset copy so its geometric center aligns with the anchor,
        // so rotation always spins around the visual centre of the mesh.
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

        // Shift the copy so its bounds centre sits at the anchor position
        copy.transform.position += inspectionAnchor.position - bounds.center;
    }

    private void ConvertSpriteToQuad(GameObject copy, SpriteRenderer sr)
    {
        var sprite = sr.sprite;
        Destroy(sr);

        if (sprite == null) return;

        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(quad.GetComponent<MeshCollider>());
        quad.transform.SetParent(copy.transform, false);

        // Preserve sprite aspect ratio
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
