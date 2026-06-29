namespace Edgar.Core
{
    /// <summary>
    /// Contract for any scene object the player can tap to interact with.
    /// Implement this on any component that should respond to player input.
    /// </summary>
    public interface IInteractable
    {
        bool IsInteractive { get; }
        void Interact();
    }
}