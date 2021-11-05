using System.Collections;
using System.Collections.Generic;
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
    private void PlaySequence()
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
    /// Calls an entities' moveTo function
    ///
    /// Input:
    /// Takes in an entity, destination vec3, bool waitForArrival that
    /// determines whether the scene should halt and wait till the entity
    /// arrives at the dest, and a threshold float for how close the
    /// entity should get to the dest before stopping in the case
    /// </summary>
    private void MoveTo(Entity e, Vector3 dest, bool waitForArrival,
        float threshold)
    {
        e.moveTo(t);
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
            if (Mathf.Abs(e.pos - t) <= threshold)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Initiates interaction with some interactable i
    /// E.g. NPC
    /// </summary>
    private void InteractWith(Interactable i)
    {
        i.onInteract();
    }
}
