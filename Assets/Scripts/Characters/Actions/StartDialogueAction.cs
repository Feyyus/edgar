using UnityEngine;
using Edgar.Characters.Core;
using Edgar.Dialogue;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Triggers a Yarn Spinner conversation when the character is clicked.
    /// Set yarnNode to the title of the .yarn node to play.
    /// </summary>
    public class StartDialogueAction : MonoBehaviour, ICharacterAction
    {
        [SerializeField] private string _yarnNode;


        public void Execute(Character character)
        {
            if (string.IsNullOrEmpty(_yarnNode)) { Debug.LogWarning("StartDialogueAction: yarnNode is empty"); return; }
            if (DialogueManager.Instance == null) { Debug.LogWarning("StartDialogueAction: no DialogueManager in scene"); return; }
            Debug.Log($"StartDialogueAction: starting node '{_yarnNode}'");
            DialogueManager.Instance.StartDialogue(_yarnNode);
        }
    }
}