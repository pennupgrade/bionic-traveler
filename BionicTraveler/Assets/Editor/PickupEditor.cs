namespace Editor
{
    using BionicTraveler.Scripts.World;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;

    /// <summary>
    /// Adds a custom editor for the <see cref="Pickup"/> type to allow it to automatically
    /// pull properties from its assigned <see cref="BionicTraveler.Scripts.Item.ItemData"/>.
    /// </summary>
    [CustomEditor(typeof(Pickup), true)]
    public class PickupEditor : Editor
    {
#if UNITY_EDITOR
        private bool justOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupEditor"/> class.
        /// </summary>
        public PickupEditor()
        {
            this.justOpened = true;
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var worldItem = this.target as Pickup;
            if (worldItem.ItemData == null)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(this.serializedObject.isEditingMultipleObjects);

            // If the button is down or we just opened the editor view and should sync the item with
            // its template, load all template settings again.
            if (GUILayout.Button("Update from template") || (this.justOpened && worldItem.KeepSyncedWithTemplate))
            {
                worldItem.InitializeFromTemplate();
            }

            EditorGUI.EndDisabledGroup();
        }
#endif
    }
}
