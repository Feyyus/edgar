using System;
using UnityEngine;
using Edgar.Core;

namespace Edgar.Inspection.Core
{
    public class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private InspectableData _inspectableData;
        [SerializeField] private bool _isInteractive = true;

        public event Action OnInspectionStarted;
        public event Action OnInspectionEnded;

        public InspectableData InspectableData => _inspectableData;
        public GameObject InspectionPrefab => _inspectableData?.inspectionPrefab;
        public Vector3 InspectionScale => _inspectableData?.inspectionScale ?? Vector3.one;
        public string Title => _inspectableData?.title ?? gameObject.name;
        public string Description => _inspectableData?.description ?? "";
        public bool IsInteractive => _isInteractive;

        private bool _isBeingInspected;

        public void Interact()
        {
            if (!IsInteractive || _isBeingInspected) return;
            BeginInspection();
        }

        public void BeginInspection()
        {
            if (!IsInteractive || _isBeingInspected) return;

            OnInspectionStarted?.Invoke();
            InspectionManager.Instance.OpenInspection(this);
        }

        public void EndInspection()
        {
            _isBeingInspected = false;
            OnInspectionEnded?.Invoke();
        }

        public void OnCopySpawned()
        {
            _isBeingInspected = true;
        }
    }
}