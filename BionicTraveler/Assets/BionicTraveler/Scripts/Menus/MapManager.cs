namespace BionicTraveler.Scripts
{
    using UnityEngine;

    /// <summary>
    /// class that opens map UI for current level.
    /// currently just the wild world.
    /// </summary>
    public class MapManager : Menu
    {
        [SerializeField]
        [Tooltip("The player indicator game object")]
        private GameObject playerIndicator;

        [SerializeField]
        [Tooltip("An offset vector from player starting position to (0,0)")]
        private Vector3 offset;

        [SerializeField]
        [Tooltip("A scaling factor that describes the ratio of units in the gameworld to pixels on the map image.")]
        private float scale;
        
        
        private GameObject playerObject;
        private RectTransform indicatorTransform;

        /// <summary>
        /// Gets an instance of the MapMenu class -> used to make sure that there is the only one instance of MapMenu.
        /// </summary>
        public static MapManager Instance { get; private set; }

        /// <summary>
        /// Called when the program is first started
        /// Gets indicator transform and player game object.
        /// </summary>
        public override void Start() 
        {
            base.Start();
            this.indicatorTransform = playerIndicator.GetComponent<RectTransform>();
            this.playerObject = GameObject.FindGameObjectsWithTag("Player")[0];
        }

        /// <summary>
        /// Called every frame.
        /// Maps the position of the the player to the indicator based on offset and scale. 
        /// </summary>
        private void Update() 
        {
            this.indicatorTransform.anchoredPosition = (playerObject.transform.position - offset) * scale;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
