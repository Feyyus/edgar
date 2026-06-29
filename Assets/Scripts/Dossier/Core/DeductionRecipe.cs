using System;
using UnityEngine;

namespace Edgar.Dossier.Core
{
    [Serializable]
    public class DeductionRecipe
    {
        [Tooltip("The clues that must be present in the dossier to unlock this deduction")]
        public ClueData[] requiredClues;

        [Tooltip("The deduction that will be unlocked when the required clues are present")]
        public ClueData deduction;
    }
}