namespace BionicTraveler.Scripts.Quests
{
    /// <summary>
    /// The quest event type.
    /// </summary>
    public enum QuestEventType
    {
        /// <summary>
        /// An enemy was killed.
        /// </summary>
        EnemyKilled,

        /// <summary>
        /// The player's inventory has changed.
        /// </summary>
        InventoryChanged,

        /// <summary>
        /// The player's inventory has changed.
        /// </summary>
        SpokenWith,
    }

    /// <summary>
    /// Describes a quest related event that happened in the game.
    /// </summary>
    public abstract class QuestEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestEvent"/> class.
        /// </summary>
        public QuestEvent()
        {
        }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        public abstract QuestEventType Type { get; }
    }

    /// <summary>
    /// Event related to inventory changes.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Very short classes.")]
    public class QuestEventInventory : QuestEvent
    {
        /// <inheritdoc/>
        public override QuestEventType Type => QuestEventType.InventoryChanged;
    }

    /// <summary>
    /// Event related to killing an enemy changes.
    /// </summary>
    public class QuestEventEnemyKilled : QuestEvent
    {
        /// <inheritdoc/>
        /// 
        public string targetKilled;
        public override QuestEventType Type => QuestEventType.EnemyKilled;
    }

    /// <summary>
    /// Event related to inventory changes.
    /// </summary>
    public class QuestEventSpokenTo : QuestEvent
    {
        /// <inheritdoc/>
        /// 
        public string personSpokenTo;
        public override QuestEventType Type => QuestEventType.SpokenWith;
    }
}
