namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum QuestObjectiveType
    {
        None,
        Item,
        Kill,
    }

    /// <summary>
    /// Please document me.
    /// </summary>
    [Serializable]
    public class QuestObjective
    {
        public string BaseTypeString;

        public virtual bool IsComplete()
        {
            return true;
        }
    }
}
