namespace BionicTraveler.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Utility function to set proper canvas size when the game starts so the canvas isn't huge in the editor.
    /// </summary>
    public class ResizeCanvas : MonoBehaviour
    {
        private Canvas canvas;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.canvas = this.GetComponent<Canvas>();
            this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
}
