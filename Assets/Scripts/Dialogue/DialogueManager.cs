using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Singleton that owns the Yarn Spinner DialogueRunner.
/// Disables interaction and navigation input while a conversation is active.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private NavigationUI navigationUI;

    public bool IsDialogueActive => dialogueRunner != null && dialogueRunner.IsDialogueRunning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
            dialogueRunner.onDialogueStart.AddListener(() => Debug.Log($"[Dialogue] started, presenters: {string.Join(", ", dialogueRunner.DialoguePresenters)}"));
        }
    }

    void OnDestroy()
    {
        if (dialogueRunner != null)
            dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
    }

    public void StartDialogue(string nodeName)
    {
        if (IsDialogueActive) return;
        if (dialogueRunner == null) { Debug.LogWarning("[Dialogue] dialogueRunner is null"); return; }

        Debug.Log($"[Dialogue] calling StartDialogue({nodeName}), project={dialogueRunner.YarnProject?.name ?? "NULL"}");

        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = false;
        if (navigationUI != null) navigationUI.enabled = false;

        dialogueRunner.StartDialogue(nodeName);
    }

    private void OnDialogueComplete()
    {
        Debug.Log("[Dialogue] complete");
        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = true;
        if (navigationUI != null) navigationUI.enabled = true;
    }
}
