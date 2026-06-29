using UnityEngine;
using Edgar.Characters.Actions;
using Edgar.Core;

namespace Edgar.Characters.Core
{
    /// <summary>
    /// Main character component that manages character data and interactions.
    /// Coordinates between visual representation, data, and actions.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Character : MonoBehaviour, IInteractable
    {
        [SerializeField] private CharacterData _data;

        private SpriteRenderer _spriteRenderer;
        private ICharacterAction[] _actions;

        public CharacterData Data => _data;
        public bool IsInteractive => _data != null && _data.isInteractive;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _actions = GetComponents<ICharacterAction>();

            if (_data != null)
            {
                ApplyCharacterData();
            }
            else
            {
                Debug.LogWarning($"[Character.Start] Character data is null for {gameObject.name}");
            }
        }

        private void ApplyCharacterData()
        {
            if (_spriteRenderer != null && _data.sprite != null)
            {
                _spriteRenderer.sprite = _data.sprite;
            }

            gameObject.name = $"Character_{_data.characterId}";
        }

        public void Interact()
        {
            if (!IsInteractive)
            {
                return;
            }

            if (_actions == null || _actions.Length == 0)
            {
                Debug.LogWarning($"[Character.Interact] No actions found on character {gameObject.name}");
                _actions = GetComponents<ICharacterAction>();
            }

            foreach (var action in _actions)
            {
                if (action != null)
                {
                    action.Execute(this);
                }
                else
                {
                    Debug.LogWarning($"[Character.Interact] Null action found in actions array");
                }
            }
        }

        public void SetCharacterData(CharacterData newData)
        {
            _data = newData;
            if (_spriteRenderer != null)
            {
                ApplyCharacterData();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_data != null && _data.sprite != null)
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = _data.sprite;
                }
            }
        }
#endif
    }
}