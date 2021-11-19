namespace BionicTraveler
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class is responsible for changing sprite alpha values when the player is behind it.
    /// </summary>
    public class SpriteTransparency : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D collider2D)
        {
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
}
