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

        /// <summary>
        /// A quest has been completed.
        /// </summary>
        QuestCompleted,
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Very short classes.")]
    public class QuestEventEnemyKilled : QuestEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestEventEnemyKilled"/> class.
        /// </summary>
        /// <param name="enemyName">The name of the enemy.</param>
        public QuestEventEnemyKilled(string enemyName)
        {
            this.EnemyName = enemyName;
        }

        /// <inheritdoc/>
        public override QuestEventType Type => QuestEventType.EnemyKilled;

        /// <summary>
        /// Gets the name of the enemy that was killed.
        /// </summary>
        public string EnemyName { get; }
    }

    /// <summary>
    /// Event related to dialogue.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Very short classes.")]
    public class QuestEventSpokenTo : QuestEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestEventSpokenTo"/> class.
        /// </summary>
        /// <param name="npcName">The name of the NPC.</param>
        public QuestEventSpokenTo(string npcName)
        {
            this.NpcName = npcName;
        }

        /// <inheritdoc/>
        public override QuestEventType Type => QuestEventType.SpokenWith;

        /// <summary>
        /// Gets the name of the NPC that was spoken to.
        /// </summary>
        public string NpcName { get; }
    }

    /// <summary>
    /// Event fired when a quest is completed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Very short classes.")]
    public class QuestEventQuestCompleted : QuestEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestEventQuestCompleted"/> class.
        /// </summary>
        /// <param name="questName">The name of the quest.</param>
        public QuestEventQuestCompleted(string questName)
        {
            this.QuestName = questName;
        }

        /// <inheritdoc/>
        public override QuestEventType Type => QuestEventType.SpokenWith;

        /// <summary>
        /// Gets the name of the quest.
        /// </summary>
        public string QuestName { get; }
    }
}
