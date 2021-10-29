using UnityEngine;
using System;


public abstract class Entity : MonoBehaviour
{
    private int MaxHealth = 100;
    private int Health;
    private GameObject ent;
    private Boolean visible = false;


    
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
    /// <param name = "add">The amount of health to be added</param>
    public void AddHealth (float add)
    {
        Health = (int)Math.Max((float)Health + add, (float)MaxHealth);
    }

    /// <summary>
    /// Decrease the current Health value by some fixed amount
    /// </summary>
    /// <param name="loss">The amount of health lost</param>
    public void LoseHealth (float loss)
    {
        Health = (int)Math.Min((float)Health - loss, 0f);
    }

    /// <summary>
    /// OnHit functionality to be implemented in children of Entity
    /// </summary>
    public abstract void OnHit();

    /// <summary>
    /// Move the entity to some specified destination
    /// </summary>
    /// <param name="dest">The Transform of the destination to move to</param>
    public void MoveTo (Vector3 dest)
    {
        Transform pos = ent.transform;
        pos.LookAt(dest);
        Vector3 dist = dest - pos.position;
        while ((dest - pos.position).magnitude > (pos.forward * Time.deltaTime).magnitude)
        {
            pos.Translate(pos.forward * Time.deltaTime);
            dist = dest - pos.position;
        }
        pos.


    }

    /// <summary>
    /// Destroy the EntityGameObject
    /// </summary>
    public void Destroy ()
    {
        Destroy(ent);
    }

    /// <summary>
    /// Hides the entity in the current Scene
    /// </summary>
    public void MakeInvisible()
    {
        UnityEditor.SceneVisibilityManager.instance.Hide(ent, true);
    }

    /// <summary>
    /// Shows/Un-hides the entity in the current Scene
    /// </summary>
    public void MakeVisible()
    {
        UnityEditor.SceneVisibilityManager.instance.Show(ent, true);
    }

    /// <summary>
    /// Toggles the visibility state of the entity GameObject in the current Scene
    /// </summary>
    public void ToggleVisibility()
    {
        UnityEditor.SceneVisibilityManager.instance.ToggleVisibility(ent, true);
    }

    public Boolean IsVisible()
    {
        return visible;
    }


    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;
        ent = gameObject;
        visible = !UnityEditor.SceneVisibilityManager.instance.IsHidden(ent, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
