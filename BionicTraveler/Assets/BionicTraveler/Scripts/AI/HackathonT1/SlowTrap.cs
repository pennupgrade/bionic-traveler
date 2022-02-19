namespace BionicTraveler.Scripts.AI.HackathonT1
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class SlowTrap : MonoBehaviour
    {
        // Start is called before the first frame update

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Debug.Log("Trigger Entered");
            // If collided with player, 
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Player Hit!");
                PlayerMovement player = collider.gameObject.GetComponent<PlayerMovement>();
                // TODO: player slowdown
                player.SlowDown();
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerMovement player = collider.gameObject.GetComponent<PlayerMovement>();
                player.RestoreSpeed();

                // Remove self once player has run over it once.
                Destroy(this.gameObject);
            }
        }
    }
}
