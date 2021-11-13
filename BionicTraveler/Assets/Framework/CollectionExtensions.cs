namespace Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains helper functions for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets a random item from the list.
        /// </summary>
        /// <typeparam name="T">The list type.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The random item.</returns>
        public static T GetRandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.IndexOutOfRangeException("List is empty.");
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
