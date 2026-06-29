# EDGAR — AI Development Guide

## Project Overview

Detective game with dossier system, inspectable objects, and clue logging.

- **Mobile-first** (horizontal orientation)
- **Unity 6000.3.9f1** · **URP** · **Cinemachine** · **Input System**

## Architecture Patterns

- **Event-Driven** — Systems communicate via events, not direct references
- **Static Service** — `DossierService` is static, not MonoBehaviour
- **Component-Based** — Small, focused components, not monolithic managers
- **Observer Pattern** — UI subscribes to service events

## Namespaces

For a script in the directory Assets/Scripts/Core/ use the namespace "Edgar.Core".

## Input System

Don't use hardcoded keys. Use Unity's new input system instead. If it's something new, suggest adding new entries to the .inputactions file.

## Naming Conventions

| Type              | Convention                      | Example                        |
| ----------------- | ------------------------------- | ------------------------------ |
| Private fields    | `_camelCase`                    | `_clueData`, `_isInteractive`  |
| Public fields     | `CamelCase`                     | `ClueId`, `Title`              |
| Properties        | `PascalCase`                    | `AllEntries`, `TotalCount`     |
| Methods           | `PascalCase`                    | `AddClue()`, `MarkAsRead()`    |
| Events            | `OnPascalCase`                  | `OnClueAdded`, `OnClueUpdated` |
| Enums             | `PascalCase`                    | `ClueCategory.Evidence`        |
| Interfaces        | `IPascalCase`                   | `IInteractable`                |
| ScriptableObjects | `PascalCase` + `Data` suffix    | `ClueData`, `CharacterData`    |
| Components        | `PascalCase` + `Trigger` suffix | `InspectStartTrigger`          |

## Code Style

- **No XML comments** unless explaining WHY (not WHAT)
- **Self-documenting** code is preferred over comments
- **Minimal regions** — avoid `#region` noise
- **DRY** — extract repeated logic into methods
- **Single Responsibility** — one class, one purpose
- **Static services** for global state, **MonoBehaviours** for scene objects

## Core Systems

### Dossier System

```csharp
// Data (ScriptableObject — read-only, lives in Assets)
ClueData : ScriptableObject {
    string clueId, title, description;
    Sprite sprite;
    ClueCategory category;
    string source;
    AudioClip discoverySound;
}

// Runtime (mutable, created when logged)
ClueEntry {
    ClueData Data;           // Reference to asset
    bool IsNew;              // Unread flag
    string PlayerNotes;      // Editable by player
    bool IsPinned;           // Player toggled
    float DiscoveredAt;      // Timestamp
}

// Service (static, global)
DossierService {
    static IReadOnlyList<ClueEntry> AllEntries;
    static event Action<ClueEntry> OnClueAdded;
    static event Action<ClueEntry> OnClueUpdated;
    static void AddClue(ClueData);
    static void MarkAsRead(ClueEntry);
    static void SetPlayerNote(ClueEntry, string);
    static void TogglePinned(ClueEntry);
    static bool HasClue(ClueData);
    static IEnumerable<ClueEntry> GetByCategory(ClueCategory);
    static IEnumerable<ClueEntry> Search(string); // Not implemented
    static void Clear(); // Testing only
}
```

### Interaction System

```csharp
// Interface
IInteractable {
    bool IsInteractive { get; }
    void Interact();
}

// Pure inspection — NO clue logic
InspectableObject : MonoBehaviour, IInteractable {
    [SerializeField] GameObject _inspectionPrefab;
    [SerializeField] Vector3 _inspectionScale = Vector3.one;
    [SerializeField] bool _isInteractive = true;
    event Action OnInspectionStarted;
    event Action OnInspectionEnded;
    void BeginInspection();
    void EndInspection();
}

// Triggers — attach to any GameObject to log clues
InspectStartTrigger : MonoBehaviour { /* calls DossierService.AddClue() on inspection start */ }
RotationThresholdTrigger : MonoBehaviour { /* calls AddClue() when rotation > threshold */ }
CollisionTrigger : MonoBehaviour { /* calls AddClue() on trigger/collision enter */ }
DialogueEndTrigger : MonoBehaviour { /* calls AddClue() when dialogue completes */ }
ClueLogger : MonoBehaviour { /* convenience wrapper — public LogClue() method */ }
```

### Inspection System

```csharp
// Manager (singleton, scene-based)
InspectionManager : MonoBehaviour {
    static InspectionManager Instance;
    void OpenInspection(InspectableObject);
    void CloseInspection();
    void ApplyRotation(Vector2 delta);
    void ApplyZoom(float delta);
}

// Input (disabled by default, enabled during inspection)
InspectionInputHandler : MonoBehaviour {
    // Handles drag rotation, pinch zoom, scroll wheel
    // Uses InputSystem actions: Inspection/Rotate, Inspection/Zoom, Inspection/ExitInspect
}

// UI (shows ClueData during inspection)
InspectionUI : MonoBehaviour {
    void Show(ClueData data);
    void Hide();
    void OnCloseButtonClicked(); // Called by button in Inspector
}
```

### UI Pattern — List + Detail

```csharp
// Main panel
DossierUI : MonoBehaviour {
    [SerializeField] Transform _listContainer;
    [SerializeField] GameObject _entryPrefab;
    // Right panel: title, description, source, notes input, new badge
    void Refresh();        // Rebuilds list, shows first entry
    void SelectEntry(ClueEntry); // Shows detail, marks as read
}

// List item
ClueEntryUI : MonoBehaviour {
    void Bind(ClueEntry entry, Action<ClueEntry> onClick);
    void Refresh();        // Updates visuals from entry
    void SetSelected(bool);
    ClueEntry Entry { get; }
}
```

## Data Flow

```
┌─────────────────────────────────────────────────────────────────────────┐
│ 1. Player clicks object                                                │
│    → InteractionManager.OnClick()                                      │
│    → IInteractable.Interact()                                          │
│    → InspectableObject.BeginInspection()                               │
├─────────────────────────────────────────────────────────────────────────┤
│ 2. Inspection starts                                                   │
│    → OnInspectionStarted event fires                                   │
│    → InspectStartTrigger.OnInspectionStarted()                         │
│    → DossierService.AddClue(clueData)                                  │
├─────────────────────────────────────────────────────────────────────────┤
│ 3. Clue is added                                                       │
│    → ClueEntry created                                                 │
│    → OnClueAdded event fires                                           │
│    → DossierUI.Refresh()                                               │
│    → New entry appears in list                                         │
├─────────────────────────────────────────────────────────────────────────┤
│ 4. Player rotates object                                               │
│    → InspectionInputHandler.HandleRotation()                           │
│    → InspectionManager.ApplyRotation()                                 │
│    → RotationThresholdTrigger.OnRotationDelta()                        │
│    → DossierService.AddClue(clueData) // if threshold reached          │
└─────────────────────────────────────────────────────────────────────────┘
```

## UI Layouts

### Dossier (Case File)

```
Left: Scrollable list of ALL clues (order: newest first)
Right: Detail view (thumbnail, title, description, source, player notes)
No tabs. No pagination. Visual icons distinguish categories.
```

### Inspection View

```
Full-screen close-up of item.
Drag to rotate. Pinch/scroll to zoom.
Close button (bottom-right corner — mobile-friendly).
```

## Mobile-First Rules

- **No hover states** — mobile has no cursor
- **Tap-based interaction** — `InteractionManager` handles both mouse and touch
- **Close button** - UI should be closable not only with a keyboard
- **Canvas Scaler** with `Reference Resolution: 1920x1080`
- **UIToolkit** not required — use Canvas + TextMeshPro

## Key Decisions

1. **No physical inventory** — items go directly to dossier log
2. **ClueData is read-only** — never modify at runtime
3. **Flat list in dossier** — no tabs (add later if needed)
4. **Event-driven UI updates** — no polling or direct references
5. **InspectableObject has NO clue logic** — pure inspection

## Common Mistakes to Avoid

1. ❌ **Modifying ScriptableObjects at runtime** — use `ClueEntry` instead
2. ❌ **Tight coupling** — use events, not `FindObjectOfType`
3. ❌ **Monolithic components** — split into small, focused pieces
4. ❌ **Enums for trigger types** — use dedicated components
5. ❌ **Comments explaining WHAT** — code should be self-documenting
6. ❌ **`public` fields without good reason** — use `[SerializeField] private`
7. ❌ **Mixing inspection and clue logic** — keep them separate

## Folder Placement Rules

| What                         | Where                                      |
| ---------------------------- | ------------------------------------------ |
| New clue type                | `Data/Clues/[Category]/`                   |
| New trigger                  | `Scripts/Dossier/Triggers/`                |
| New UI component for dossier | `Scripts/Dossier/UI/`                      |
| New inspectable prefab       | `Prefabs/Items/`                           |
| New character                | `Prefabs/Characters/` + `Data/Characters/` |
| New camera point             | `Prefabs/Camera/`                          |
| Generic helper               | `Scripts/Utils/`                           |
| Shared UI helper             | `Scripts/UI/`                              |
