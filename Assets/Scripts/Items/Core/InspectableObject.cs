using System;
using UnityEngine;
using Edgar.Core;
using Edgar.Items.Core;

namespace Edgar.Items.Core
{
    public class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _inspectionPrefab;
        [SerializeField] private Vector3 _inspectionScale = Vector3.one;
        [SerializeField] private bool _isInteractive = true;

        public event Action OnInspectionStarted;
        public event Action OnInspectionEnded;

        public GameObject InspectionPrefab => _inspectionPrefab;
        public Vector3 InspectionScale => _inspectionScale;
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