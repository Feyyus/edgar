using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Edgar.Dossier.Core;

namespace Edgar.Dossier.UI
{
    public class DossierUI : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private GameObject _emptyState;

        [Header("Left Panel — List")]
        [SerializeField]
        [Tooltip("Container for the list of clues")]
        private Transform _listContainer;
        [SerializeField]
        [Tooltip("Prefab for a single clue entry in the list")]
        private GameObject _entryPrefab;

        [Header("Right Panel — Detail")]
        [SerializeField] private GameObject _detailContainer;
        [SerializeField] private Image _photo;
        [SerializeField] private Sprite _defaultPhoto;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _source;
        [SerializeField] private TMP_InputField _notes;
        [SerializeField] private GameObject _newBadge;

        // State
        private List<ClueEntry> _entries = new();
        private ClueEntry _selectedEntry;
        private List<ClueEntryUI> _entryUIs = new();
        private InputAction _toggleAction;

        // ===== Unity Lifecycle =====

        private void Awake()
        {
            _toggleAction = InputSystem.actions?.FindAction("Player/ToggleDossier");
        }

        private void Start()
        {
            gameObject.SetActive(false);
            _detailContainer.SetActive(false);
            _emptyState.SetActive(false);

            DossierService.OnClueAdded += OnClueAdded;
            DossierService.OnClueUpdated += OnClueUpdated;
        }

        private void OnEnable()
        {
            if (_toggleAction != null)
            {
                _toggleAction.performed += OnToggleRequested;
                _toggleAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (_toggleAction != null)
            {
                _toggleAction.performed -= OnToggleRequested;
                _toggleAction.Disable();
            }
        }

        private void OnDestroy()
        {
            DossierService.OnClueAdded -= OnClueAdded;
            DossierService.OnClueUpdated -= OnClueUpdated;
        }

        // ===== Public API =====

        public void Toggle()
        {
            if (gameObject.activeSelf) Close();
            else Open();
        }

        private void OnToggleRequested(InputAction.CallbackContext context)
        {
            Toggle();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        // ===== Core Logic =====

        private void Refresh()
        {
            // Get all entries from service, newest first
            _entries = DossierService.GetAllEntries()
                .OrderByDescending(e => e.DiscoveredAt)
                .ToList();

            RebuildList();

            if (_selectedEntry != null)
            {
                // If the previously selected entry still exists, keep it selected
                if (_entries.Contains(_selectedEntry))
                {
                    SelectEntry(_selectedEntry);
                    return;
                }
            }

            // Auto-select first entry if available
            if (_entries.Count > 0)
            {
                SelectEntry(_entries[0]);
            }
            else
            {
                ShowEmpty();
            }
        }

        private void RebuildList()
        {
            // Clear existing entries
            foreach (var ui in _entryUIs)
            {
                if (ui != null) Destroy(ui.gameObject);
            }
            _entryUIs.Clear();

            // Instantiate new entries
            foreach (var entry in _entries)
            {
                var go = Instantiate(_entryPrefab, _listContainer);
                var ui = go.GetComponent<ClueEntryUI>();
                ui.Bind(entry, SelectEntry);
                _entryUIs.Add(ui);
            }
        }

        private void SelectEntry(ClueEntry entry)
        {
            _selectedEntry = entry;
            ShowDetail(entry);

            // Mark as read
            DossierService.MarkAsRead(entry);

            // Update list item visuals
            foreach (var ui in _entryUIs)
            {
                ui.Refresh();
                ui.SetSelected(ui.Entry == entry);
            }
        }

        private void ShowDetail(ClueEntry entry)
        {
            _detailContainer.SetActive(true);
            _emptyState.SetActive(false);

            _photo.sprite = entry.Data.sprite != null ? entry.Data.sprite : _defaultPhoto;
            _title.text = entry.Data.title;
            _description.text = entry.Data.description;
            _source.text = string.IsNullOrEmpty(entry.Data.source)
                ? ""
                : $"Source: {entry.Data.source}";
            _newBadge.SetActive(entry.IsNew);

            // Player notes (editable)
            _notes.onValueChanged.RemoveAllListeners();
            _notes.text = entry.PlayerNotes;
            _notes.onValueChanged.AddListener(val =>
            {
                DossierService.SetPlayerNote(entry, val);
            });
        }

        private void ShowEmpty()
        {
            _detailContainer.SetActive(false);
            _emptyState.SetActive(true);
            _selectedEntry = null;
        }

        // ===== Event Handlers =====

        private void OnClueAdded(ClueEntry entry)
        {
            if (gameObject.activeSelf)
            {
                Refresh();
            }
        }

        private void OnClueUpdated(ClueEntry entry)
        {
            if (!gameObject.activeSelf) return;

            // If this entry is currently selected, update detail view
            if (_selectedEntry == entry)
            {
                ShowDetail(entry);
            }

            // Update list item if it exists
            foreach (var ui in _entryUIs)
            {
                if (ui.Entry == entry)
                {
                    ui.Refresh();
                    break;
                }
            }
        }
    }
}
