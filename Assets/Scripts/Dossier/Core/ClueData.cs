using UnityEngine;

namespace Edgar.Dossier.Core
{
    [CreateAssetMenu(fileName = "Clue_", menuName = "Edgar/Dossier/Clue Data")]
    public class ClueData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for this clue (e.g., 'trashbin_note')")]
        public string clueId;
        
        [Tooltip("Display name shown in the dossier list")]
        public string title;

        [Header("Content")]
        [Tooltip("Full description shown in the detail view")]
        [TextArea(3, 10)]
        public string description;

        [Header("Visual")]
        [Tooltip("Thumbnail shown in the dossier list and detail view")]
        public Sprite sprite;

        [Header("Metadata")]
        [Tooltip("How this clue is categorized in the dossier")]
        public ClueCategory category;
        
        [Tooltip("Where this clue came from (e.g., 'Found in trashbin', 'Given by butler')")]
        public string source;

        [Header("Audio (Optional)")]
        [Tooltip("Sound played when this clue is added to the dossier")]
        public AudioClip discoverySound;
    }
}