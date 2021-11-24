namespace BionicTraveler.Scripts.Items
{
    /// <summary>
    /// Describes keys linked to levels (scenes).
    /// </summary>
    public class KeyData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyData"/> class.
        /// </summary>
        /// <param name="color">The key color.</param>
        /// <param name="scenes">The list of scenes this key belongs to.</param>
        public KeyData(KeyColor color, string[] scenes)
        {
            this.Color = color;
            this.Scenes = scenes;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public KeyColor Color { get; }

        /// <summary>
        /// Gets the scenes.
        /// </summary>
        public string[] Scenes { get; }
    }
}
