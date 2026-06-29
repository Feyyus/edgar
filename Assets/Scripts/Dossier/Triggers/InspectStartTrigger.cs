using UnityEngine;
using Edgar.Dossier.Core;
using Edgar.Inspection.Core;

namespace Edgar.Dossier.Triggers
{
    public class InspectStartTrigger : MonoBehaviour
    {
        [SerializeField] private ClueData _clue;

        private InspectableObject _inspectable;

        private void Awake()
        {
            _inspectable = GetComponent<InspectableObject>();
            if (_inspectable != null)
            {
                _inspectable.OnInspectionStarted += LogClue;
            }
        }

        private void OnDestroy()
        {
            if (_inspectable != null)
            {
                _inspectable.OnInspectionStarted -= LogClue;
            }
        }

        public void LogClue()
        {
            if (_clue == null) return;

            DossierService.AddClue(_clue);
        }
    }
}