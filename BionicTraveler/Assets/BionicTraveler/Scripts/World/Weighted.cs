namespace BionicTraveler.Scripts.World
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Class to hold some object along with a weight
    /// <![CDATA[ List<Weighted<T>> ]]> is an unity-serializable alternative
    /// for <![CDATA[ Dictionary<T, int> ]]>.
    /// </summary>
    /// <typeparam name="T">type of value stored</typeparam>
    [Serializable]
    public struct Weighted<T>
    {
        /// <summary>
        /// value being stored.
        /// </summary>
        [SerializeField]
        private T value;

        /// <summary>
        /// weight for the value.
        /// </summary>
        [SerializeField]
        private int weight;

        /// <summary>
        /// Gets the value
        /// </summary>
        public T Value => this.value;

        /// <summary>
        /// Gets the weight for the value
        /// </summary>
        public int Weight => this.weight;

        /// <summary>
        /// overriding ToString for better logging
        /// </summary>
        /// <returns>string.</returns>
        public override string ToString()
        {
            return "{v:" + this.Value + ",w:" + this.Weight + "}";
        }
    }

}