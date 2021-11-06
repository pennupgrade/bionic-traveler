using UnityEngine;
using System;
using System.Collections;
using BionicTraveler.Scripts.Combat;

public abstract class Entity : MonoBehaviour
{
    private int MaxHealth = 100;
    private int Health;
    private bool Visible = false;
    [SerializeField]
    private float BaseSpeed = 1f;

    private bool invincible;

    /// <summary>
    /// Gets or sets a value indicating whether entity is invincible.
    /// </summary>
    internal bool IsInvincible { get => this.invincible; set => this.invincible = value; }

    /// <summary>
    /// Retrieve MaxHealth from Entity
    /// </summary>
    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    /// <summary>
    /// Retrieve Health from Entity
    /// </summary>
    public int GetHealth()
    {
        return Health;
    }

    /// <summary>
    /// Add some fixed value to the current Health value
    /// </summary>
    /// <param name = "amt">The amount of health to be added</param>
    public void AddHealth (float amt)
    {
        Health = (int)Math.Max((float)Health + amt, (float)MaxHealth);
    }

    /// <summary>
    /// Decrease the current Health value by some fixed amount
    /// </summary>
    /// <param name="amt">The amount of health lost</param>
    public void LoseHealth(float amt)
    {
        this.Health = (int)Math.Max(this.Health - amt, 0f);
        Debug.Log($"{this.gameObject.name}: My health is now {this.Health}");
    }

    /// <summary>
    /// OnHit functionality to be implemented in children of Entity.
    /// </summary>
    /// <param name="attack">The attack.</param>
    public virtual void OnHit(Attack attack)
    {
        if (this.IsInvincible)
        {
            return;
        }

        Debug.Log($"{this.gameObject.name} just got hit");
        var damage = attack.AttackData.GetBaseDamage();
        this.LoseHealth(damage);
    }

    /// <summary>
    /// Move the entity to some specified destination
    /// </summary>
    /// <param name="dest">The Transform of the destination to move to</param>
    /// <param name="smooth">Whether to use a SmoothStep function for the movement; default false</param>
    public void MoveTo (Vector3 dest, bool smooth = false)
    {
        StartCoroutine(Movement(dest, smooth));
    }
    
    private IEnumerator Movement(Vector3 target, bool smooth)
    {
        Transform pos = gameObject.transform;
        float duration = (pos.position - target).magnitude/BaseSpeed;
        float elapsed = 0f;
        float t = elapsed / duration;
        if (smooth) { t = t * t * (3f - 2f * t); }

        while (elapsed < duration)
        {
            pos.position = Vector3.Lerp(pos.position, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pos.position = target;
            
    }

    /// <summary>
    /// Destroy the EntityGameObject
    /// </summary>
    public void Destroy ()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Hides the entity in the current Scene
    /// </summary>
    public void MakeInvisible()
    {
        UnityEditor.SceneVisibilityManager.instance.Hide(gameObject, true);
    }

    /// <summary>
    /// Shows/Un-hides the entity in the current Scene
    /// </summary>
    public void MakeVisible()
    {
        UnityEditor.SceneVisibilityManager.instance.Show(gameObject, true);
    }

    /// <summary>
    /// Toggles the visibility state of the entity GameObject in the current Scene
    /// </summary>
    public void ToggleVisibility()
    {
        UnityEditor.SceneVisibilityManager.instance.ToggleVisibility(gameObject, true);
    }

    public bool IsVisible()
    {
        return Visible;
    }

    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;
        Visible = !UnityEditor.SceneVisibilityManager.instance.IsHidden(gameObject, true);
    }
}
