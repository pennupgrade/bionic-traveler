namespace BionicTraveler.Scripts.Interaction
{
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Logic to walk towards BFF for intro sequence.
    /// </summary>
    public class IntroSequenceBff : MonoBehaviour
    {
        [SerializeField]
        private GameObject bff;

        private void StartLogic()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerEntity = player.GetComponent<PlayerEntity>();
            playerEntity.DisableInput();
            playerEntity.EnableScriptedSequenceMovement();

            var taskGoToPoint = new TaskGoToPoint(playerEntity, this.bff.transform.position + (Vector3.down * 0.5f));
            taskGoToPoint.ForceWalking = true;
            taskGoToPoint.Ended += this.TaskGoToPoint_Ended;
            taskGoToPoint.Assign();
        }

        private void TaskGoToPoint_Ended(EntityTask task, bool successful)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerEntity = player.GetComponent<PlayerEntity>();
            playerEntity.DisableScriptedSequenceMovement();

            DialogueStates.Instance.SetCustomState("Intro_FinishedWalkingToBff");
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                this.StartLogic();
            }
        }
    }
}
