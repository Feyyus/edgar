using UnityEngine;
using Edgar.Dossier.Core;
using Edgar.Items.Core;

namespace Edgar.Dossier.Triggers
{
    public class InspectStartTrigger : MonoBehaviour
    {
        [SerializeField] private ClueData _clue;
        [SerializeField] private bool _logOnce = true;

        private InspectableObject _inspectable;
        private bool _hasLogged;

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
            if (_logOnce && _hasLogged) return;
            if (_clue == null) return;

            DossierService.AddClue(_clue);
            _hasLogged = true;
        }
    }
}