namespace BionicTraveler.Scripts.Audio
{
    using UnityEngine;

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
        public float loopPoint; 

        /// <summary>
        /// Gets the audio clip.
        /// </summary>
        public AudioClip Clip => this.clip;

        /// <summary>
        /// Gets the loop point.
        /// </summary>
        public float LoopPoint => this.loopPoint;
    }
}
