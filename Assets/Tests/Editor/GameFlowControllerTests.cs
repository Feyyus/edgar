using NUnit.Framework;
using Edgar.Core;
using UnityEngine;

namespace Edgar.Tests
{
    public class GameFlowControllerTests
    {
        [Test]
        public void EnterDialogue_SetsStateToDialogue()
        {
            var gameObject = new GameObject("GameFlowControllerTest");
            var controller = gameObject.AddComponent<GameFlowController>();

            controller.EnterDialogue();

            Assert.AreEqual(GameFlowState.Dialogue, controller.CurrentState);
            Assert.IsFalse(controller.CanInteract);

            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void EnterInspection_ThenExitInspection_ReturnsToExploration()
        {
            var gameObject = new GameObject("GameFlowControllerTest");
            var controller = gameObject.AddComponent<GameFlowController>();

            controller.EnterInspection();
            Assert.AreEqual(GameFlowState.Inspection, controller.CurrentState);
            Assert.IsTrue(controller.IsInspecting);

            controller.ExitInspection();
            Assert.AreEqual(GameFlowState.Exploration, controller.CurrentState);
            Assert.IsTrue(controller.CanInteract);

            Object.DestroyImmediate(gameObject);
        }
    }
}
