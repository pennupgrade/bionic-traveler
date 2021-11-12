namespace BionicTraveler.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Main camera controller, makes the camera follow the player.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // Start is called before the first frame update
        private Transform target;
        [SerializeField] private float smoothSpeed;

        private void Start()
        {
            this.target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        // Update is called once per frame
        private void Update()
        {
            this.transform.position = new Vector3(this.target.position.x, this.target.position.y, this.transform.position.z);
        }
    }
}
