using NUnit.Framework;
using Edgar.Dossier.Core;
using UnityEngine;

namespace Edgar.Tests
{
    public class DossierServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            DossierService.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            DossierService.Clear();
        }

        [Test]
        public void AddClue_AddsEntryAndMarksItAsNew()
        {
            var clue = ScriptableObject.CreateInstance<ClueData>();
            clue.clueId = "test_clue";
            clue.title = "Test Clue";
            clue.description = "A test clue";
            clue.category = ClueCategory.Evidence;

            DossierService.AddClue(clue);

            Assert.AreEqual(1, DossierService.TotalClues);
            var entry = DossierService.GetEntry(clue);
            Assert.IsNotNull(entry);
            Assert.IsTrue(entry.IsNew);

            Object.DestroyImmediate(clue);
        }

        [Test]
        public void MarkAsRead_UpdatesEntryState()
        {
            var clue = ScriptableObject.CreateInstance<ClueData>();
            clue.clueId = "test_clue_read";
            clue.title = "Read Me";
            clue.description = "A clue to mark as read";
            clue.category = ClueCategory.Evidence;

            DossierService.AddClue(clue);
            var entry = DossierService.GetEntry(clue);

            DossierService.MarkAsRead(entry);

            Assert.IsFalse(entry.IsNew);
            Object.DestroyImmediate(clue);
        }
    }
}
