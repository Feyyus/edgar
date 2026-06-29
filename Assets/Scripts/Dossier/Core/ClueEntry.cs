using System;
using UnityEngine;

namespace Edgar.Dossier.Core
{
    [Serializable]
    public class ClueEntry
    {
        public ClueEntry(ClueData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            DiscoveredAt = Time.time;
            IsNew = true;
            PlayerNotes = string.Empty;
            IsPinned = false;
        }

        public ClueData Data { get; private set; }

        // Unix timestamp
        public float DiscoveredAt { get; private set; }

        // Set to false when the player views the clue in the dossier
        public bool IsNew { get; set; }

        /// <summary>
        /// Player's personal notes for this clue. Can be empty.
        /// </summary>
        public string PlayerNotes { get; set; }

        public bool IsPinned { get; set; }

        // ===== Convenience Properties =====
        public string Title => Data?.title;
        public string Description => Data?.description;
        public Sprite Sprite => Data?.sprite;
        public ClueCategory Category => Data?.category ?? ClueCategory.Evidence;
    }
}
