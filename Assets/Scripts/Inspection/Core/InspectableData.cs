using UnityEngine;

namespace Edgar.Inspection.Core
{
    [CreateAssetMenu(fileName = "Inspectable_", menuName = "Edgar/Inspection/Inspectable Data")]
    public class InspectableData : ScriptableObject
    {
        [Header("Identity")]
        public string title;
        [TextArea(2, 5)]
        public string description;

        [Header("Visual")]
        public GameObject inspectionPrefab;  // What to show in the inspection view
        public Vector3 inspectionScale = Vector3.one;
    }
}