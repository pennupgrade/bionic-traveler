namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// KnightScriptedEvent
    /// A derived class of ScriptedEvent, currently deals with a single Dynamic
    /// Entity, Knight, however this can be modified to however many Dynamic
    /// Entities as well as Interactables if we want Dynamic Entities
    /// Interacting with Interactables.
    /// </summary>
    public class KnightScriptedEvent : ScriptedEvent
    {
        [SerializeField]
        private BionicTraveler.Scripts.World.DynamicEntity knight;

        /// <summary>
        /// GenerateEvent override
        /// </summary>
        /// <returns>IEnumerator (Coroutine) </returns>
        public override IEnumerator GenerateEvent()
        {
            yield return this.MoveTo(this.knight, new Vector3(-750.37f, -17.94f, 0.0f), 0.4f);
            yield return this.MoveTo(this.knight, new Vector3(-750.37f, -16.31f, 0.0f), 0.4f);
            yield return this.MoveTo(this.knight, new Vector3(-751.95f, -16.31f, 0.0f), 0.4f);
            yield return this.MoveTo(this.knight, new Vector3(-757.37f, -16.31f, 0.0f), 0.4f);

            float speed = 3.0f;
            for (int i = 0; i < 9; i++)
            {
                yield return this.MoveTo(this.knight, new Vector3(-757.22f, -19.86f, 0.0f), speed);
                speed *= 2;
                yield return this.MoveTo(this.knight, new Vector3(-750.57f, -19.86f, 0.0f), speed);
                speed *= 2;
                yield return this.MoveTo(this.knight, new Vector3(-750.57f, -15.82f, 0.0f), speed);
                speed *= 2;
                yield return this.MoveTo(this.knight, new Vector3(-757.17f, -15.82f, 0.0f), speed);
                speed *= 2;
            }
            yield return this.MoveTo(this.knight, new Vector3(-757.22f, -19.86f, 0.0f), 0.05f);
        }
    }
}
