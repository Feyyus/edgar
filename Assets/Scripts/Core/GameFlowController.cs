using UnityEngine;
using Edgar.Input;
using Edgar.Items.Core;
using Edgar.Items.Input;
using Edgar.UI;

namespace Edgar.Core
{
    public enum GameFlowState
    {
        Exploration,
        Dialogue,
        Inspection,
    }

    public class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        [SerializeField] private InteractionManager _interactionManager;
        [SerializeField] private NavigationUI _navigationUI;
        [SerializeField] private InspectionManager _inspectionManager;
        [SerializeField] private InspectionInputHandler _inspectionInputHandler;

        public GameFlowState CurrentState { get; private set; } = GameFlowState.Exploration;
        public bool CanInteract => CurrentState == GameFlowState.Exploration;
        public bool IsInspecting => CurrentState == GameFlowState.Inspection;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ApplyState(CurrentState);
        }

        public void EnterDialogue()
        {
            if (CurrentState == GameFlowState.Dialogue) return;
            CurrentState = GameFlowState.Dialogue;
            ApplyState(CurrentState);
        }

        public void ExitDialogue()
        {
            if (CurrentState != GameFlowState.Dialogue) return;
            CurrentState = GameFlowState.Exploration;
            ApplyState(CurrentState);
        }

        public void EnterInspection()
        {
            if (CurrentState == GameFlowState.Inspection) return;
            CurrentState = GameFlowState.Inspection;
            ApplyState(CurrentState);
        }

        public void ExitInspection()
        {
            if (CurrentState != GameFlowState.Inspection) return;
            CurrentState = GameFlowState.Exploration;
            ApplyState(CurrentState);
        }

        private void ApplyState(GameFlowState state)
        {
            bool interactionEnabled = state == GameFlowState.Exploration;
            bool navigationEnabled = state != GameFlowState.Dialogue && state != GameFlowState.Inspection;
            bool inspectionInputEnabled = state == GameFlowState.Inspection;

            if (_interactionManager != null)
                _interactionManager.enabled = interactionEnabled;

            if (_navigationUI != null)
                _navigationUI.enabled = navigationEnabled;

            if (_inspectionInputHandler != null)
                _inspectionInputHandler.enabled = inspectionInputEnabled;

            if (_inspectionManager != null && !inspectionInputEnabled)
                _inspectionManager.SetInputEnabled(false);
        }
    }
}
