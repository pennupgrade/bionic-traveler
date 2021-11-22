namespace BionicTraveler.Assets.Framework
{
    /// <summary>
    /// Describes a time instant in the game.
    /// </summary>
    public sealed class GameTime
    {
        private bool isDefault;

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

        /// <summary>
        /// Gets the current game time.
        /// </summary>
        public static GameTime Now => new GameTime();

        /// <summary>
        /// Gets the default time which represents the moment the game has started (0 seconds).
        /// All calls to <see cref="HasTimeElapsed(float)"/> will return true.
        /// If you need to compare against game startup without having calls evaluate to true,
        /// use <see cref="Now"/> at startup instead.
        /// </summary>
        public static GameTime Default => new GameTime(true);

        /// <summary>
        /// Gets the time this <see cref="GameTime"/> represents.
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public float Elapsed => UnityEngine.Time.time - this.Time;

        /// <summary>
        /// Returns whether the specified time has elapsed.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds.</param>
        /// <returns>Whether the time has elapsed.</returns>
        public bool HasTimeElapsed(float timeInSeconds)
        {
            // Special behavior for our default case.
            if (this.isDefault)
            {
                return true;
            }

            return UnityEngine.Time.time - this.Time > timeInSeconds;
        }

        /// <summary>
        /// Returns whether the specified time has elapsed. Resets the internal timer afterwards.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds.</param>
        /// <returns>Whether the time has elapsed.</returns>
        public bool HasTimeElapsedReset(float timeInSeconds)
        {

            // Special behavior for our default case.
            if (this.isDefault)
            {
                this.isDefault = false;
                this.Time = UnityEngine.Time.time;
                return true;
            }

            var hasElapsed = UnityEngine.Time.time - this.Time > timeInSeconds;
            if (hasElapsed)
            {
                this.Time = UnityEngine.Time.time;
            }

            return hasElapsed;
        }
    }
}