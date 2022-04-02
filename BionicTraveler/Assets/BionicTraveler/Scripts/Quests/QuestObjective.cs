namespace BionicTraveler.Scripts.Quests
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Describes the type of objective.
    /// </summary>
    public enum QuestObjectiveType
    {
        /// <summary>
        /// No type.
        /// </summary>
        None,

        /// <summary>
        /// Obtain items.
        /// </summary>
        Item,

        /// <summary>
        /// Kill enemies.
        /// </summary>
        Kill,
    }

    /// <summary>
    /// Base class for objectives within quests that represent one concrete goal.
    /// </summary>
    [Serializable]
    public abstract class QuestObjective : ICloneable
    {
        /// <summary>
        /// Delegate for changes in a <see cref="QuestObjective"/>'s completeness state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newState">The new state.</param>
        public delegate void QuestObjectiveStateChangedEventHandler(QuestObjective sender, bool newState);

        /// <summary>
        /// Invoked when the completeness state changes.
        /// </summary>
        public event QuestObjectiveStateChangedEventHandler CompleteStateChanged;

        // Since this is a ScriptableObject, all state is serialized by default. We disable this for the
        // following properties so they get reset on each start.

        /// <summary>
        /// Gets a value indicating whether this objective is complete.
        /// </summary>
        [field: NonSerialized]
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Called when the objective becomes active. This allows to check for completion immediately.
        /// </summary>
        public virtual void Activate()
        {
        }

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEvent">The event.</param>
        public virtual void ProcessEvent(QuestEvent questEvent)
        {
        }

        /// <summary>
        /// Sets the objective as complete.
        /// </summary>
        public void SetAsComplete()
        {
            bool invoke = this.IsComplete != true;
            this.IsComplete = true;
            if (invoke)
            {
                this.CompleteStateChanged?.Invoke(this, true);
            }
        }

        /// <summary>
        /// Sets the objective as incomplete.
        /// </summary>
        public void SetAsIncomplete()
        {
            bool invoke = this.IsComplete != false;
            this.IsComplete = false;
            if (invoke)
            {
                this.CompleteStateChanged?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Clones the base properties from another <see cref="QuestObjective"/>.
        /// </summary>
        /// <param name="toClone">The object to clone.</param>
        public void CloneBase(QuestObjective toClone)
        {
            // No fields for now.
        }

        /// <summary>
        /// We implement cloning so that objectives can be easily duplicated or restored in the editor.
        /// </summary>
        /// <returns>A cloned instance.</returns>
        public abstract object Clone();
    }
}
