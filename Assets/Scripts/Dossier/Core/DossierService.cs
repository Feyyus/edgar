using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Edgar.Dossier.Core
{
    public static class DossierService
    {
        private static readonly List<ClueEntry> _entries = new();
        
        public static IReadOnlyList<ClueEntry> AllEntries => _entries;
        public static int TotalClues => _entries.Count;
        public static int NewClues => _entries.Count(e => e.IsNew);
        public static IReadOnlyList<ClueEntry> GetAllEntries() => _entries;

        public static event Action<ClueEntry> OnClueAdded;
        public static event Action<ClueEntry> OnClueUpdated;

        public static void AddClue(ClueData data)
        {
            if (data == null)
            {
                Debug.LogError("[DossierService] Attempted to add null ClueData");
                return;
            }

            // Prevent duplicates (by asset reference)
            if (_entries.Any(e => e.Data == data))
            {
                Debug.Log($"[DossierService] Clue '{data.title}' already in dossier");
                return;
            }

            var entry = new ClueEntry(data);
            _entries.Add(entry);

            if (data.discoverySound != null)
            {
                // TODO: Play discovery sound (requires AudioManager or similar)
                // AudioManager.PlayOneShot(data.discoverySound);
            }
            
            Debug.Log($"[DossierService] Added clue: {data.title}");
            OnClueAdded?.Invoke(entry);
        }

        public static void MarkAsRead(ClueEntry entry)
        {
            if (entry == null || !entry.IsNew) return;

            entry.IsNew = false;
            OnClueUpdated?.Invoke(entry);
        }

        public static void SetPlayerNote(ClueEntry entry, string note)
        {
            if (entry == null) return;

            entry.PlayerNotes = note ?? string.Empty;
            OnClueUpdated?.Invoke(entry);
        }

        public static void TogglePinned(ClueEntry entry)
        {
            if (entry == null) return;
            entry.IsPinned = !entry.IsPinned;
            OnClueUpdated?.Invoke(entry);
        }

        public static bool HasClue(ClueData data) => data != null && _entries.Any(e => e.Data == data);
        public static bool HasClue(string clueId) => !string.IsNullOrEmpty(clueId) && _entries.Any(e => e.Data.clueId == clueId);
        public static ClueEntry GetEntry(ClueData data) => data != null ? _entries.FirstOrDefault(e => e.Data == data) : null;
        public static ClueEntry GetEntry(string clueId) => !string.IsNullOrEmpty(clueId) ? _entries.FirstOrDefault(e => e.Data.clueId == clueId) : null;
        
        public static IEnumerable<ClueEntry> GetByCategory(ClueCategory category) => _entries.Where(e => e.Data.category == category);
        public static IEnumerable<ClueEntry> GetNew() => _entries.Where(e => e.IsNew);
        public static IEnumerable<ClueEntry> GetPinned() => _entries.Where(e => e.IsPinned);
        
        public static void Clear()
        {
            _entries.Clear();
        }
    }
}