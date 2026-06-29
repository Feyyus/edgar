using UnityEngine;
using Edgar.Dossier.Core;
using Edgar.Input;
using Edgar.Inspection.Input;
using Edgar.UI;
using Edgar.Characters.Core;
using Edgar.Core;

namespace Edgar.Inspection.Core
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
        [SerializeField] private InspectionViewportController _viewportController;
        [SerializeField] private InspectionCopySpawner _copySpawner;

        private InspectableObject _currentItem;
        private GameObject _inspectionCopy;

        public bool IsInspecting => _currentItem != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetInputEnabled(false);
        }

        public void SetInputEnabled(bool enabled)
        {
            if (_inputHandler != null)
                _inputHandler.enabled = enabled;
        }

        public void OpenInspection(InspectableObject item)
        {
            if (IsInspecting) return;

            _currentItem = item;
            if (_viewportController != null)
                _viewportController.ResetView();

            _inspectionCopy = _copySpawner != null ? _copySpawner.SpawnCopy(item) : null;

            if (_copySpawner != null && _copySpawner.OriginalBehavior == InspectionOriginalBehavior.Hide)
                item.gameObject.SetActive(false);

            if (GameFlowController.Instance != null)
                GameFlowController.Instance.EnterInspection();

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

            if (_copySpawner != null && _copySpawner.OriginalBehavior == InspectionOriginalBehavior.Hide)
                _currentItem.gameObject.SetActive(true);

            Destroy(_inspectionCopy);
            _inspectionCopy = null;

            if (_inputHandler != null) _inputHandler.enabled = false;
            if (GameFlowController.Instance != null)
                GameFlowController.Instance.ExitInspection();

            _inspectionUI.Hide();
            _currentItem = null;
            if (_viewportController != null)
                _viewportController.ResetView();
        }

        public void ApplyRotation(Vector2 delta)
        {
            if (_inspectionCopy == null) return;
            if (_viewportController != null)
                _viewportController.ApplyRotation(delta);
        }

        public void ApplyZoom(float delta)
        {
            if (_inspectionCopy == null) return;
            if (_viewportController != null)
                _viewportController.ApplyZoom(delta);
        }
    }

    public enum InspectionOriginalBehavior
    {
        Hide,
        Highlight,
        Nothing
    }
}
