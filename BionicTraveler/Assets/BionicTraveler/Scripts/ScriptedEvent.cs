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
        this.StartCoroutine(this.GenerateEvent());
    }

    /// <summary>
    /// Generates a coroutine of events for the cutscene
    /// </summary>
    private IEnumerator GenerateEvent()
    {
        yield return null;
    }

    /// <summary>
    /// Calls an entities' MoveTo function
    /// </summary>
    /// <param name="e">The entity to move</param>
    /// <param name="dest">Postion the entity should move to</param>
    /// <param name="waitForArrival">Whether the scene should halt and wait till the entity
    /// arrives at the dest</param>
    /// <param name="threshold">A threshold float for how close the
    /// entity should get to the dest before snapping</param>
    private void MoveTo(Entity e, Vector3 dest, bool waitForArrival,
        float threshold)
    {
        e.MoveTo(dest);
        if (waitForArrival)
        {
            WaitForArrival(e, dest, threshold);
        }
    }

    /// <summary>
    ///  Halts the program until entity has gotten within threshold reach of
    ///  dest
    /// </summary>
    private void WaitForArrival(Entity e, Vector3 dest, float threshold)
    {
        while (true)
        {
            if (Mathf.Abs((e.gameObject.transform.position - dest).magnitude) <= threshold)
            {
                break;
            }
        }
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
