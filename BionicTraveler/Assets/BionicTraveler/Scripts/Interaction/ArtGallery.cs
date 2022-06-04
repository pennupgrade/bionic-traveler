namespace BionicTraveler.Scripts.Interaction
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Main gallery interaction script.
    /// </summary>
    public class ArtGallery : MonoBehaviour
    {
        [SerializeField]
        private Menu galleryMenu;

        private void Start()
        {
            this.GetComponent<DialogueHost>().DialogueCompleted += this.ArtGallery_DialogueCompleted;
        }

        private void ArtGallery_DialogueCompleted(DialogueHost sender)
        {
            if (sender.VariableStorage.GetValue("$decision_leave").AsBool)
            {
                SceneManager.LoadScene("MainMenu");
            }

            if (sender.VariableStorage.GetValue("$decision_sound").AsBool)
            {
                Menus.WindowManager.Instance.ToggleMenu(this.galleryMenu);
            }
        }
    }
}
