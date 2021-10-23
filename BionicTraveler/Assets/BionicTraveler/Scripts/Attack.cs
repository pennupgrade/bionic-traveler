namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Interface for attack, containing few defined properties and functions,
    /// other specific fields implemented in implementations.
    /// </summary>
    public interface IAttack<T>
    {
        // properties
        float range { get; set; }
        float cooldown { get; set; }
        float freezeDuration { get; set; }

        // functions
        void executeAttack(T attackType);
        bool didHit(T attackType);
    }
}
