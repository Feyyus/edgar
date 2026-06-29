using UnityEngine;
using Yarn.Unity;
using Edgar.Input;
using Edgar.Dossier.Triggers;
using Edgar.UI;
using Edgar.Core;

namespace Edgar.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private NavigationUI _navigationUI;
        [SerializeField] private GameObject _dialoguePanel;

        private string _lastNodeName;

        public bool IsDialogueActive => _dialogueRunner != null && _dialogueRunner.IsDialogueRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (_dialoguePanel != null) _dialoguePanel.SetActive(false);

            if (_dialogueRunner != null)
            {
                _dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
                _dialogueRunner.onNodeStart.AddListener(OnNodeStart);
            }
        }

        private void OnDestroy()
        {
            if (_dialogueRunner != null)
            {
                _dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
                _dialogueRunner.onNodeStart.RemoveListener(OnNodeStart);
            }
        }

        public void StartDialogue(string nodeName)
        {
            if (IsDialogueActive) return;
            if (_dialogueRunner == null) return;

            _lastNodeName = nodeName;

            if (_dialoguePanel != null) _dialoguePanel.SetActive(true);

            if (GameFlowController.Instance != null)
                GameFlowController.Instance.EnterDialogue();

            _dialogueRunner.StartDialogue(nodeName);
        }

        private void OnNodeStart(string nodeName)
        {
            _lastNodeName = nodeName;
        }

        private void OnDialogueComplete()
        {
            if (_dialoguePanel != null) _dialoguePanel.SetActive(false);

            if (GameFlowController.Instance != null)
                GameFlowController.Instance.ExitDialogue();

            // Notify all DialogueEndTrigger components
            var triggers = FindObjectsByType<DialogueEndTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
                trigger.OnDialogueComplete(_lastNodeName);
            }
        }
    }
}