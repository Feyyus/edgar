using Edgar.Characters.Core;

namespace Edgar.Characters.Actions
{
    /// <summary>
    /// Interface for all character actions.
    /// Implement this to create custom behaviors when a character is interacted with.
    /// </summary>
    public interface ICharacterAction
    {
        /// <summary>
        /// Execute this action on the given character.
        /// </summary>
        /// <param name="character">The character being interacted with</param>
        void Execute(Character character);
    }
}