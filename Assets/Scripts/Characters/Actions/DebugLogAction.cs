using UnityEngine;
using Edgar.Characters.Core;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Example action that logs a message when the character is interacted with.
    /// Useful for testing and debugging character interactions.
    /// </summary>
    public class DebugLogAction : MonoBehaviour, ICharacterAction
    {
        [SerializeField] private string _customMessage = "";

        public void Execute(Character character)
        {
            if (character.Data == null)
            {
                Debug.LogError($"[DebugLogAction.Execute] Character data is null for {character.gameObject.name}");
                return;
            }

            if (!string.IsNullOrEmpty(_customMessage))
            {
                Debug.Log($"[{character.Data.displayName}]: {_customMessage}");
            }
            else
            {
                Debug.Log($"Interacted with character: {character.Data.displayName} (ID: {character.Data.characterId})");
            }
        }
    }

}