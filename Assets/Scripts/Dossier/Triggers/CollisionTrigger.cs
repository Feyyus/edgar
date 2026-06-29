using UnityEngine;
using Edgar.Dossier.Core;

namespace Edgar.Dossier.Triggers
{
    public class CollisionTrigger : MonoBehaviour
    {
        [SerializeField] private ClueData _clue;
        [SerializeField] private string _tagToCheck = "Player";
        [SerializeField] private bool _logOnce = true;
        [SerializeField] private float _delay = 0f;
        
        private bool _hasLogged;


        private void OnTriggerEnter(Collider other)
        {
            if (_logOnce && _hasLogged) return;
            if (!other.CompareTag(_tagToCheck)) return;
            
            if (_delay > 0f) Invoke(nameof(LogClue), _delay);
            else LogClue();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_logOnce && _hasLogged) return;
            if (!collision.gameObject.CompareTag(_tagToCheck)) return;
            
            LogClue();
        }

        private void LogClue()
        {
            DossierService.AddClue(_clue);
            _hasLogged = true;
        }
    }
}