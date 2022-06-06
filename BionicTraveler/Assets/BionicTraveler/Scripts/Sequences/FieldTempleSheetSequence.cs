namespace BionicTraveler.Scripts.Sequences
{
    using BionicTraveler.Scripts.Interaction;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class FieldTempleSheetSequence : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject collisionObject = collision.gameObject;
            if (collisionObject.tag == "Player")
            {
                DialogueHost host = GameObject.Find("fieldTempleSheet").GetComponent<DialogueHost>();
                // sets name of Lyra to BFF
                host.StartDialogue(collisionObject, "fieldTempleSheet", "BFF", "mysterySheet");
            }
        }
    }
}
