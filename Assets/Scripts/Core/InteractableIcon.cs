using UnityEngine;

/// <summary>
/// Displays a floating, camera-facing icon above any interactable object.
/// Add to the same GameObject as Character or InspectableItem.
/// Assign a sprite in the Inspector (e.g. a "?" or "!" icon).
/// </summary>
public class InteractableIcon : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private float heightPadding = 0.25f;
    [SerializeField] private float bobSpeed = 1.5f;
    [SerializeField] private float bobAmount = 0.06f;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float scale = 0.4f;

    private Transform _pivot;
    private float _baseHeight;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        _baseHeight = ComputeTopY() + heightPadding;

        var pivotGO = new GameObject("_InteractableIcon");
        pivotGO.transform.SetParent(transform, false);
        _pivot = pivotGO.transform;
        _pivot.localScale = Vector3.one * scale;

        var sr = pivotGO.AddComponent<SpriteRenderer>();
        sr.sprite = icon;
        sr.color = color;

        var interactable = GetComponent<IInteractable>();
        if (interactable != null)
            pivotGO.SetActive(interactable.IsInteractive);
    }

    void LateUpdate()
    {
        if (_pivot == null) return;

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
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
