namespace BionicTraveler.Scripts.AI
{
    using System.Linq;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class GuardBehavior : EntityBehavior
    {
        private TaskPlayAnimation animTask;
        private TaskExecuteSequence guardAnimSequence;
        private bool hasStartedDialogue;
        private bool shouldPushPlayer;

        public enum GuardState
        {
            Idle,
            Confronting,
            Dialogue,
        }

        public override IFSM CreateFSM()
        {
            var fsm = new FSM<GuardState>();
            fsm.SetDefaultState(GuardState.Idle);
            fsm.RegisterCallback(GuardState.Confronting, this.ConfrontingState);
            fsm.RegisterCallback(GuardState.Idle, this.IdleState);
            fsm.RegisterCallback(GuardState.Dialogue, this.DialogueState);
            return fsm;
        }

        private void IdleState(FSM<GuardState> sender, GuardState currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.animTask = new TaskPlayAnimation(this.Owner, "Idle");
                    this.animTask.Assign();
                    break;

                case FSMSubState.Remain:
                    if (this.IsPlayerClose())
                    {
                        sender.AdvanceTo(GuardState.Confronting);
                    }
                    break;

                case FSMSubState.Leave:

                    break;
            }
        }

        private void ConfrontingState(FSM<GuardState> sender, GuardState currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Player Detected - confonting now");
                    this.animTask = new TaskPlayAnimation(this.Owner, "GuardSpearCross");
                    var taskSequence = new TaskSequence(
                        this.animTask,
                        new TaskPlayAnimation(this.Owner, "GuardSpearUncross"));
                    this.guardAnimSequence = new TaskExecuteSequence(this.Owner, taskSequence);
                    this.guardAnimSequence.Assign();

                    this.hasStartedDialogue = false;
                    this.shouldPushPlayer = false;
                    break;

                case FSMSubState.Remain:
                    var playerObj = GameObject.FindGameObjectWithTag("Player");
                    var player = playerObj.GetComponent<PlayerEntity>();
                    if (this.animTask.Progress > 0.5f)
                    {
                        this.shouldPushPlayer = true;
                    }

                    if (this.shouldPushPlayer)
                    {
                        // Try to start the dialogue if we have not started it yet and player has control,
                        // i.e. is not busy in another dialogue.
                        if (!this.hasStartedDialogue && player.HasControl)
                        {
                            var interactableComp = this.GetComponent<DialogueInteractable>();
                            interactableComp.OnInteract(playerObj);
                            this.hasStartedDialogue = true;
                        }

                        // Move them until player is out of radius.
                        if (this.IsPlayerClose())
                        {
                            player.ApplyForce(Vector2.down * 10);
                        }
                        else
                        {
                            sender.AdvanceTo(GuardState.Dialogue);
                        }
                    }
                    break;

                case FSMSubState.Leave:

                    break;
            }
        }

        private void DialogueState(FSM<GuardState> sender, GuardState currentState, FSMSubState subState)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            var interactableComp = this.GetComponent<DialogueInteractable>();
            switch (subState)
            {
                case FSMSubState.Enter:
                    break;

                case FSMSubState.Remain:
                    // If our dialogue has been completed or it was never started, go back to idle.
                    if (interactableComp.HasRun || !this.hasStartedDialogue)
                    {
                        sender.AdvanceTo(GuardState.Idle);
                    }
                    break;

                case FSMSubState.Leave:

                    break;
            }
        }

        private bool IsPlayerClose()
        {
            if (this.EntityScanner != null)
            {
                var nearbyTargets = this.EntityScanner.GetAllDynamicInRange();
                var target = nearbyTargets.FirstOrDefault(target => target.IsPlayer);
                if (target != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }
}
