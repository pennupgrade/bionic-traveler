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
                    this.animTask.Assign();
                    break;

                case FSMSubState.Remain:
                    if (this.animTask.Progress > 0.5f)
                    {
                        var playerObj = GameObject.FindGameObjectWithTag("Player");
                        var player = playerObj.GetComponent<PlayerEntity>();

                        player.ApplyForce(Vector2.down * 10);
                        sender.AdvanceTo(GuardState.Dialogue);

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
                    interactableComp.OnInteract(playerObj);
                    this.animTask = new TaskPlayAnimation(this.Owner, "GuardSpearUncross");
                    this.animTask.Assign();
                    break;

                case FSMSubState.Remain:
                    if (interactableComp.HasRun)
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
