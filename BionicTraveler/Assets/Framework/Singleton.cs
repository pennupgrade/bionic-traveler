namespace BionicTraveler.Assets.Framework
{
    /// <summary>
    /// An object that only exists once globally.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class Singleton<T>
        where T : Singleton<T>, new()
    {
        private static T instance;

        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }
    }
}