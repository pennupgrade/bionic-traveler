namespace Editor
{
    using System.Linq;
    using Framework;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;

    /// <summary>
    /// Property drawwer to render tag names.
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityTagSelectorAttribute))]
    public class UnityTagsPropertyDrawer : PropertyDrawer
    {
#if UNITY_EDITOR
        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nodes = UnityEditorInternal.InternalEditorUtility.tags.ToList();
            EditorGUI.BeginChangeCheck();
            var currentIndex = nodes.IndexOf(property.stringValue);
            bool hasNoValue = currentIndex == -1;
            currentIndex = EditorGUI.Popup(position, "Tag", currentIndex, nodes.ToArray());
            if (EditorGUI.EndChangeCheck() || hasNoValue)
            {
                // Ensure we always have a valid value selected.
                property.stringValue = nodes[System.Math.Max(0, currentIndex)];
            }
        }
#endif
    }
}
