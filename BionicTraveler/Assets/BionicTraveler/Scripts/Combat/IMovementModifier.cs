namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;

    /// <summary>
    /// Describes a modifier for an entity's movement, such as slowing them down.
    /// </summary>
    public interface IMovementModifier
    {
        /// <summary>
        /// Gets the multiplier affecting the speed.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Multiplier for speed.</returns>
        float GetSpeedMultiplier(Entity entity);

        /// <summary>
        /// Gets whether this effect can stack with other effects of the same type.
        /// </summary>
        /// <returns>A boolean whether this effect can stack with other effects of the same type.</returns>
        bool CanStack();
    }
}