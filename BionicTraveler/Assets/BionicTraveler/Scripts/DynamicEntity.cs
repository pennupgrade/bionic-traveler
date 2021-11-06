using System.Collections;
using BionicTraveler.Scripts.Items;
using UnityEngine;

/// <summary>
/// Class for all entities that can move themselves.
/// </summary>
public class DynamicEntity : Entity
{
    private Vector3 direction;
    private Vector3 velocity;
    private bool stunned;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicEntity"/> class.
    /// </summary>
    public DynamicEntity()
    {
        this.Inventory = new Inventory(this);
    }

    /// <summary>
    /// Gets the inventory.
    /// </summary>
    public Inventory Inventory { get; }

    /// <summary>
    /// Gets or sets direction for SpriteRenderer/FSM.
    /// </summary>
    internal Vector3 Direction { get => this.direction; set => this.direction = value; }

    /// <summary>
    /// Gets or sets velocity.
    /// </summary>
    internal Vector3 Velocity { get => this.velocity; set => this.velocity = value; }

    /// <summary>
    /// Gets or sets a value indicating whether entity is stunned.
    /// </summary>
    internal bool Stunned { get => this.stunned; set => this.stunned = value; }


    /// <summary>
    /// Function for moving a dynamic entity to a target position.
    /// </summary>
    /// <param name="target">Target world position to move to.</param>
    public void MoveTo(Vector3 target)
    {
        Vector3 pos = this.gameObject.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(target.y - pos.y, target.x - pos.x);
        if (angle > 315 || angle < 45)
        {
            this.Direction = Vector3.right;
        }
        else if (angle < 135)
        {
            this.Direction = Vector3.up;
        }
        else if (angle < 225)
        {
            this.Direction = Vector3.left;
        }
        else
        {
            this.Direction = Vector3.down;
        }

        base.MoveTo(target);
    }

    /// <summary>
    /// Makes DEntity invincible for specified number of milliseconds
    /// </summary>
    /// <param name="ms">Number of milliseconds to remain invincible</param>
    public void IFrame(int ms)
    {
        this.StartCoroutine(this.IFrameHandler(ms));
    }

    /// <summary>
    /// Stagger/Stun the entity for the specified number of milliseconds
    /// </summary>
    /// <param name="ms">The number of milliseconds to stun the entity</param>
    public void Stagger(int ms)
    {
        this.StartCoroutine(this.StaggerHandler(ms));
    }

    private IEnumerator IFrameHandler(int ms)
    {
        this.IsInvincible = true;
        yield return new WaitForSeconds(ms / 1000f);
        this.IsInvincible = false;
    }

    private IEnumerator StaggerHandler(int ms)
    {
        this.Stunned = true;
        yield return new WaitForSeconds(ms / 1000f);
        this.Stunned = false;
    }

}
