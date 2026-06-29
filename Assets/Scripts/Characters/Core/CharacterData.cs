using UnityEngine;

namespace Edgar.Characters.Core
{
    /// <summary>
    /// ScriptableObject that stores character properties.
    /// Create via: Assets > Create > Characters > Character Data
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Edgar/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for this character")]
        public string characterId;

        [Tooltip("Display name shown to player")]
        public string displayName;

        [Header("Visual")]
        [Tooltip("Sprite to display for this character")]
        public Sprite sprite;

        [Header("Behavior")]
        [Tooltip("Can this character be interacted with?")]
        public bool isInteractive = true;
    }
}