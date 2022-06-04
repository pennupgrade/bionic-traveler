namespace BionicTraveler.Scripts.AI
{
    using System.Linq;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.Quests;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Main behavior logic for the familiar. Includes approach, fight and retreat.
    /// </summary>
    public class FamiliarBehavior : EntityBehavior
    {
        [SerializeField]
        private GameObject walkPoint;

        [SerializeField]
        private GameObject walkBackPoint;

        private TaskGoToPoint walkTask;
        private TaskCombat combatTask;
        private bool hasFought;

        /// <summary>
        /// Describes the states of the familiar.
        /// </summary>
        public enum FamiliarState
        {
            Idle,
            Dialogue,
            Walk,
            Fight,
            WalkBack,
        }

        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<FamiliarState>();
            fsm.SetDefaultState(FamiliarState.Idle);
            fsm.RegisterCallback(FamiliarState.Idle, this.IdleState);
            fsm.RegisterCallback(FamiliarState.Dialogue, this.DialogueState);
            fsm.RegisterCallback(FamiliarState.Walk, this.WalkState);
            fsm.RegisterCallback(FamiliarState.Fight, this.FightState);
            fsm.RegisterCallback(FamiliarState.WalkBack, this.WalkBackState);
            return fsm;
        }

        private void IdleState(FSM<FamiliarState> sender, FamiliarState currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    break;

                case FSMSubState.Remain:
                    if (!this.hasFought && this.IsPlayerClose())
                    {
                        sender.AdvanceTo(FamiliarState.Dialogue);
                    }

                    break;
            }
        }

        private void DialogueState(FSM<FamiliarState> sender, FamiliarState currentState, FSMSubState subState)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            var interactableComp = this.GetComponent<DialogueInteractable>();
            switch (subState)
            {
                case FSMSubState.Enter:
                    interactableComp.OnInteract(playerObj);
                    break;

                case FSMSubState.Remain:
                    if (interactableComp.HasRun)
                    {
                        var speakWithQuest = new Quests.QuestEventSpokenTo("Familiar");
                        QuestManager.Instance.ProcessEvent(speakWithQuest);

                        sender.AdvanceTo(FamiliarState.Walk);
                    }

                    break;
            }
        }

        private void WalkState(FSM<FamiliarState> sender, FamiliarState currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.walkTask = new TaskGoToPoint(this.Owner, this.walkPoint.transform.position);
                    this.walkTask.Assign();
                    break;

                case FSMSubState.Remain:
                    if (this.walkTask.HasEnded)
                    {
                        sender.AdvanceTo(FamiliarState.Fight);
                    }

                    break;
            }
        }

        private void FightState(FSM<FamiliarState> sender, FamiliarState currentState, FSMSubState subState)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            this.Owner.Damaged += this.Owner_Damaged;
            switch (subState)
            {
                case FSMSubState.Enter:
                    // Enable combat behavior.
                    this.GetComponent<CasterEntityBehavior>().enabled = true;
                    break;

                case FSMSubState.Remain:
                    if (this.hasFought)
                    {
                        // Remove guards.
                        var guards = GameObject.FindObjectsOfType<GuardBehavior>();
                        foreach (var guard in guards)
                        {
                            Destroy(guard.gameObject);
                        }

                        sender.AdvanceTo(FamiliarState.WalkBack);
                    }

                    break;

                case FSMSubState.Leave:

                    break;
            }
        }

        private void Owner_Damaged(Entity sender, Entity attacker, bool fatal)
        {
            if (fatal)
            {
                QuestManager.Instance.ProcessEvent(new Quests.QuestEventEnemyKilled(this.name));

                // Restore health and stop fighting.
                sender.SetHealth(100);
                this.GetComponent<CasterEntityBehavior>().enabled = false;
                this.hasFought = true;
                this.Owner.IsInvincible = true;
            }
        }

        private void WalkBackState(FSM<FamiliarState> sender, FamiliarState currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.walkTask = new TaskGoToPoint(this.Owner, this.walkBackPoint.transform.position);
                    this.walkTask.Assign();
                    break;

                case FSMSubState.Remain:
                    if (this.walkTask.HasEnded)
                    {
                        sender.AdvanceTo(FamiliarState.Idle);
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
    }
}
