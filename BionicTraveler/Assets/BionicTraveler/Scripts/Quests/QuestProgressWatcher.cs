namespace BionicTraveler.Scripts.Quests
{
    using BionicTraveler.Scripts.Interaction;
    using UnityEngine;

    /// <summary>
    /// Global watcher class for the quest system. Due to lack of scripting/glue for events when
    /// quest or objectives get finished, we hardcode the logic here for now.
    /// </summary>
    internal class QuestProgressWatcher : MonoBehaviour
    {
        [SerializeField]
        private DialogueInteractable bearDialogue;

        [SerializeField]
        private string questName;

        private void Start()
        {
            var questManager = GameObject.FindObjectOfType<Quests.QuestManager>();
            questManager.OnQuestFinished += this.OnQuestFinished;
        }

        private void OnQuestFinished(Quest quest)
        {
            if (quest.Title == questName)
            {
                this.bearDialogue.OnInteract(this.GetPlayer());
            }
        }

        private GameObject GetPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return player;
        }
    }
}