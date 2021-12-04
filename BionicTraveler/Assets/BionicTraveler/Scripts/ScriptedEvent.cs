using System.Collections;
using System.Collections.Generic;
using BionicTraveler.Scripts.Interaction;
using BionicTraveler.Scripts.World;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptedEvent class is the template class for a general scripted event/cutscene to be derived by specific scripted events/cutscenes
/// </summary>
public abstract class ScriptedEvent : MonoBehaviour
{
    // Cinemachine Virtual Cameras Director
    [SerializeField]
    private PlayableDirector CameraDirector;

    /// <summary>
    /// Plays the cutscenes coroutine
    /// </summary>
    public void PlaySequence()
    {
        CameraDirector.Play();
        this.StartCoroutine(this.GenerateEvent());
    }

    /// <summary>
    /// Generates a coroutine of events for the cutscene
    /// </summary>
    public abstract IEnumerator GenerateEvent();

    /// <summary>
    /// Calls an entities' MoveTo function
    /// </summary>
    /// <param name="e">The entity to move</param>
    /// <param name="dest">Postion the entity should move to arrives at
    /// the dest</param>
    /// <param name="moveSpeed"> Speed of entity movement</param>
    public Coroutine MoveTo(Entity e, Vector3 dest, float moveSpeed)
    {
        return e.MoveTo(dest, moveSpeed);
    }

    /// <summary>
    /// Initiates interaction with some interactable i
    /// E.g. NPC
    /// </summary>
    private void InteractWith(IInteractable i)
    {
        i.OnInteract(this.gameObject);
    }
}
