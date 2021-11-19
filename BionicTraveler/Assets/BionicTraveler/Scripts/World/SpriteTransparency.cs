namespace BionicTraveler.Scripts.World
{
    using BionicTraveler.Assets.Framework;
    using UnityEngine;

    /// <summary>
    /// This class is responsible for changing sprite alpha values when the player is behind it.
    /// </summary>
    public class SpriteTransparency : MonoBehaviour
    {
        private float targetAlpha;
        private float baseAlpha;
        private bool shouldAnimate;
        private GameTime startTime;
        private float animationTime;
        private bool isFadingOut;

        private void OnTriggerEnter2D(Collider2D collider2D) => this.StartAnimation(0.5f, 0.1f, true);

        private void OnTriggerExit2D(Collider2D collider2D) => this.StartAnimation(1.0f, 0.25f, false);

        private void StartAnimation(float targetAlpha, float animationTime, bool isFadingOut)
        {
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                this.targetAlpha = targetAlpha;
                this.animationTime = animationTime;
                this.isFadingOut = isFadingOut;
                this.shouldAnimate = true;
                this.startTime = GameTime.Now;
                this.baseAlpha = spriteRenderer.color.a;
            }
        }

        private void FixedUpdate()
        {
            if (this.shouldAnimate)
            {
                this.shouldAnimate = !this.startTime.HasTimeElapsed(this.animationTime);

                SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
                var newAlpha = 0.0f;
                if (this.isFadingOut)
                {
                    newAlpha = 1.0f - (this.targetAlpha * (this.startTime.Elapsed / this.animationTime));
                }
                else
                {
                    var gap = this.targetAlpha - this.baseAlpha;
                    newAlpha = this.baseAlpha + (gap * (this.startTime.Elapsed / this.animationTime));
                }

                newAlpha = Mathf.Clamp01(newAlpha);
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, newAlpha);
            }
        }
    }
}