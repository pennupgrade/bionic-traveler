namespace BionicTraveler.Assets.Framework
{
    public sealed class GameTime
    {
        private bool isDefault;

        public static GameTime Now => new GameTime();

        /// <summary>
        /// Gets the default time which represents the moment the game has started (0 seconds).
        /// All calls to <see cref="HasTimeElapsed(float)"/> will return true.
        /// If you need to compare against game startup without having calls evaluate to true,
        /// use <see cref="Now"/> at startup instead.
        /// </summary>
        public static GameTime Default => new GameTime(true);

        private GameTime()
        {
            this.Time = UnityEngine.Time.time;
        }

        private GameTime(bool isDefault)
        {
            this.isDefault = isDefault;
            if (this.isDefault)
            {
                this.Time = 0;
            }
            else
            {
                this.Time = UnityEngine.Time.time;
            }
        }

        public float Time { get; }

        public bool HasTimeElapsed(float timeInSeconds)
        {
            // Special behavior for our default case.
            if (this.isDefault)
            {
                return true;
            }

            return UnityEngine.Time.time - this.Time > timeInSeconds;
        }
    }
}