namespace Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using BionicTraveler.Scripts.Interaction;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Property drawwer to render dialogue node names.
    /// </summary>
    [CustomPropertyDrawer(typeof(DialogueNodeSelectorAttribute))]
    public class YarnProgramPropertyDrawer : PropertyDrawer
    {
        private static Regex arrayElementRegex = new Regex(@"\[(.*?)\]");

        private bool showField;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;
            var attrib = this.attribute as DialogueNodeSelectorAttribute;

            // We only support value look for DialogueData directly and for DialogueHost.
            YarnProgram value = null;
            if (targetObject.GetType() == typeof(DialogueData))
            {
                var field = typeof(DialogueData).GetField(
                attrib.Dialogue,
                BindingFlags.Instance | BindingFlags.NonPublic);
                value = field.GetValue(targetObject) as YarnProgram;
            }
            else if (targetObject.GetType() == typeof(DialogueHost))
            {
                // If we are within a DialogueHost, try to identify what array element we are.
                var path = property.propertyPath.Replace("." + property.name, string.Empty);
                var indexMatch = arrayElementRegex.Match(path);
                if (indexMatch != null)
                {
                    var val = indexMatch.Value.Replace("[", string.Empty).Replace("]", string.Empty);
                    if (int.TryParse(val, out int itemIndex))
                    {
                        // We now know which array element we are.
                        // Get the list of dialogues field.
                        var listField = targetObject.GetType().GetField(
                            "dialogues",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        var listValue = listField.GetValue(targetObject);
                        if (listValue is List<DialogueData> dialogueList)
                        {
                            value = dialogueList[itemIndex].Dialogue;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("DialogueNodeSelectorAttribute can only be used on DialogueData or DialogueHost");
                return;
            }

            // Get the actual dialogue interactable via name from the attribute.
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
