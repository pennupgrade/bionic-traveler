using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BionicTraveler
{
    /// <summary>
    /// PrimitiveCutSceneTrigger
    /// Really collider simple cutscene trigger
    /// used for testing if KnightScriptedEvent works!
    /// </summary>
    public class PrimitiveCutSceneTrigger : MonoBehaviour
    {
        [SerializeField]
        private ScriptedEvent sEvent;
        private bool played = false;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player" && played == false)
            {
                played = true;
                Debug.Log("Collision occurring");
                sEvent.PlaySequence();
                Debug.Log("Sequence Complete");
            }
        }
    }
}
