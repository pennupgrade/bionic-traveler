namespace Editor
{
    using System.Linq;
    using System.Reflection;
    using BionicTraveler.Scripts.Interaction;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Property drawwer to render dialogue node names.
    /// </summary>
    [CustomPropertyDrawer(typeof(DialogueNodeSelectorAttribute))]
    public class YarnProgramPropertyDrawer : PropertyDrawer
    {
        private bool showField;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;
            var attrib = this.attribute as DialogueNodeSelectorAttribute;

            // Get the actual dialogue interactable via name from the attribute.
            var field = typeof(DialogueInteractable).GetField(
                attrib.Dialogue,
                BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(targetObject) as YarnProgram;
            if (value != null)
            {
                var nodes = value.GetProgram().Nodes.Keys.ToList();
                EditorGUI.BeginChangeCheck();
                var currentIndex = nodes.IndexOf(property.stringValue);
                bool hasNoValue = currentIndex == -1;
                currentIndex = EditorGUI.Popup(position, "Start Node", currentIndex, nodes.ToArray());
                if (EditorGUI.EndChangeCheck() || hasNoValue)
                {
                    // Ensure we always have a valid value selected.
                    property.stringValue = nodes[System.Math.Max(0, currentIndex)];
                }

                this.showField = true;
            }
            else
            {
                this.showField = false;
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return this.showField ? EditorGUI.GetPropertyHeight(property) : -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
