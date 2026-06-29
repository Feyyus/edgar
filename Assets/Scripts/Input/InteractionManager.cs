using UnityEngine;
using UnityEngine.InputSystem;
using Edgar.Core;

namespace Edgar.Interaction
{
    /// <summary>
    /// Handles all player tap/click input for the scene.
    /// Raycasts once per tap — items take priority over characters.
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        [Header("Raycast Settings")]
        [SerializeField] private float _maxRaycastDistance = 100f;
        [SerializeField] private LayerMask _interactionMask;

        private Camera _mainCamera;
        private InputAction _interactAction;
        private float _lastInteractionTime;
        private Vector2 _lastPointerPosition;
        private const float INTERACTION_COOLDOWN = 0.2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _mainCamera = Camera.main;

            // Try to get the action from the Input Action Asset
            var asset = InputSystem.actions;
            if (asset != null)
            {
                _interactAction = asset.FindAction("Player/Interact");
            }

            // Fallback: create a simple action
            if (_interactAction == null)
            {
                _interactAction = new InputAction(type: InputActionType.Button);
                _interactAction.AddBinding("<Mouse>/leftButton");
                _interactAction.AddBinding("<Touchscreen>/primaryTouch/tap");
                _interactAction.AddBinding("<Touchscreen>/touch*/press");
            }

            _interactAction.performed += OnClick;
        }

        private void OnEnable()
        {
            _interactAction?.Enable();
        }

        private void OnDisable()
        {
            _interactAction?.Disable();
        }

        private void Update()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                _lastPointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            else if (Mouse.current != null)
                _lastPointerPosition = Mouse.current.position.ReadValue();
        }

        private void OnDestroy()
        {
            if (_interactAction != null)
            {
                _interactAction.performed -= OnClick;
                _interactAction.Dispose();
            }
        }

        private void OnClick(InputAction.CallbackContext ctx)
        {
            if (Time.time - _lastInteractionTime < INTERACTION_COOLDOWN) return;
            _lastInteractionTime = Time.time;

            var hit = GetPointerHit();
            if (hit == null) return;

            var interactable = hit.Value.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.IsInteractive)
                interactable.Interact();
        }

        private RaycastHit? GetPointerHit()
        {
            if (_mainCamera == null) return null;

            Ray ray = _mainCamera.ScreenPointToRay(GetScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit hit, _maxRaycastDistance, _interactionMask))
                return hit;

            return null;
        }

        private Vector2 GetScreenPosition() => _lastPointerPosition;
    }
}