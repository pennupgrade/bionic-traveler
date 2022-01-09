namespace BionicTraveler.Scripts.Quests
{
    using System;

    [System.Serializable]
    public class QuestObjectiveFactory
    {
        public QuestObjectiveType Type = QuestObjectiveType.None;

        //make sure the variable names here match the type, needed for reflection.
        public QuestObjectiveItem QuestObjectiveItem = new QuestObjectiveItem();
        public QuestObjectiveKill QuestObjectiveKill = new QuestObjectiveKill();

        public QuestObjective CreateObjective()
        {
            return GetAbilityFromType(Type);
        }

        public System.Type GetClassType(QuestObjectiveType abilityType)
        {
            return GetAbilityFromType(abilityType).GetType();
        }

        private QuestObjective GetAbilityFromType(QuestObjectiveType type)
        {
            switch (type)
            {
                case QuestObjectiveType.Item:
                    return QuestObjectiveItem;
                case QuestObjectiveType.Kill:
                    return QuestObjectiveKill;
                default:
                    return null;
            }
        }
    }
}