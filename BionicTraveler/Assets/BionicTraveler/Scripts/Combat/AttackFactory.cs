﻿namespace BionicTraveler.Scripts.Combat
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Simple factory to create new attacks. Not really a factory pattern since Unity does
    /// not allow constructors on behaviors, but something like it.
    /// </summary>
    public class AttackFactory
    {
        /// <summary>
        /// Creates a new <see cref="Attack"/> based on the specified data and attaches it to <paramref name="gameObject"/>.
        /// </summary>
        /// <param name="gameObject">The game object receiving the new component.</param>
        /// <param name="data">The attack data.</param>
        /// <returns>The new attack component.</returns>
        public static Attack CreateAttack(GameObject gameObject, AttackData data)
        {
            if (gameObject is null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var attack = CreateAttack(gameObject, data.Type);
            attack.SetData(data);
            return attack;
        }

        private static Attack CreateAttack(GameObject gameObject, AttackType type)
        {
            return type switch
            {
                AttackType.Melee => gameObject.AddComponent<MeleeAttack>(),
                AttackType.RangedProjectile => gameObject.AddComponent< ProjectileAttack>(),
                _ => throw new InvalidOperationException($"Invalid type {type}"),
            };
        }
    }
}