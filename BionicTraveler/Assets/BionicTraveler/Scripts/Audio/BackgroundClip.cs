namespace BionicTraveler.Scripts.Audio
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a background music clip and its relevant metadata.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudio", menuName = "Audio/Background Clip")]
    public class BackgroundClip : ScriptableObject
    {

        [SerializeField]
        [Tooltip("The audio clip file.")]
        private AudioClip clip;

        [SerializeField]
        [Tooltip("The timestamp in seconds when the clip should loop.")]
        private float loopPoint;

        [SerializeField]
        [Tooltip("The timestamp in seconds where the clip should loop to.")]
        private float loopStart;

        [SerializeField]
        [Tooltip("The scene which this background clip should play in.")]
        private List<string> scenes;

        /// <summary>
        /// Gets the audio clip.
        /// </summary>
        public AudioClip Clip => this.clip;

        /// <summary>
        /// Gets the loop point.
        /// </summary>
        public float LoopPoint => this.loopPoint;

        /// <summary>
        /// Gets the loop start.
        /// </summary>
        public float LoopStart => this.loopStart;

        /// <summary>
        /// Gets the scene.
        /// </summary>
        public List<string> Scenes => this.scenes;

    }
}
