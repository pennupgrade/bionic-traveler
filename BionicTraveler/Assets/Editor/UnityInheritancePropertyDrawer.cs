namespace BionicTraveler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Framework;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Property drawwer to render dialogue node names.
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityInheritanceAttribute))]
    public class UnityInheritancePropertyDrawer : PropertyDrawer
    {
        private float extraHeight;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;
            var attrib = this.attribute as UnityInheritanceAttribute;

            var obj = this.GetTargetObjectOfProperty(property);
            var inheritance = (IUnityInheritance)obj;

            var types = inheritance.DiscoverTypes().ToArray();

            var typeNames = types.Select(t => t.FullName).ToList();
            var displayTypeNames = types.Select(t => t.Name);
            EditorGUI.BeginChangeCheck();
            var currentIndex = typeNames.IndexOf(inheritance.TypeName);
            bool hasNoValue = currentIndex == -1;
            EditorGUI.BeginProperty(position, null, property);
            var popupPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            currentIndex = EditorGUI.Popup(popupPosition, "Type", currentIndex, displayTypeNames.ToArray());
            if (EditorGUI.EndChangeCheck() || hasNoValue)
            {
                Debug.Log(hasNoValue);

                // Ensure we always have a valid value selected.
                var newIndex = System.Math.Max(0, currentIndex);
                inheritance.TypeName = typeNames[newIndex];

                // Create or update type.
                inheritance.CreateType();
            }

            this.extraHeight = popupPosition.height + 2;
            var startPosition = position;
            EditorGUI.indentLevel++;

            foreach (var prop in this.GetChildProperties(property))
            {
                var propHeight = EditorGUI.GetPropertyHeight(prop);
                EditorGUI.PropertyField(
                    new Rect(startPosition.x, startPosition.y + this.extraHeight, startPosition.width, propHeight),
                    prop);
                this.extraHeight += propHeight + 2;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.CalculateHeight(property);
            return this.extraHeight + 10; // Some padding at the end.
        }

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>The internal object.</returns>
        public object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null)
            {
                return null;
            }

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", string.Empty).Replace("]", string.Empty));
                    obj = this.GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = this.GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private void CalculateHeight(SerializedProperty property)
        {
            // Popup.
            this.extraHeight = EditorGUIUtility.singleLineHeight + 2;

            // Child properties.
            foreach (var prop in this.GetChildProperties(property))
            {
                this.extraHeight += EditorGUI.GetPropertyHeight(prop) + 2;
            }
        }

        private IEnumerable<SerializedProperty> GetChildProperties(SerializedProperty property)
        {
            // Get the actual managed reference where our property is defined in.
            var obj = this.GetTargetObjectOfProperty(property);
            var inheritance = (IUnityInheritance)obj;

            // Only get properties that are under our currentProperty.instanceName to exclude all other
            // fields that do not belong to our type.
            string parentPath = property.propertyPath + "." + inheritance.InstanceName + ".";

            SerializedProperty underlyingType = property.FindPropertyRelative(inheritance.InstanceName).Copy();
            underlyingType.Reset();

            while (underlyingType.NextVisible(true))
            {
                if (underlyingType.propertyPath.StartsWith(parentPath))
                {
                    yield return underlyingType;
                }
            }
        }

        // Partial implementations from https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs.
        private object GetValue_Imp(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            var type = source.GetType();
            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                {
                    return f.GetValue(source);
                }

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                {
                    return p.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = this.GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext())
                {
                    return null;
                }
            }

            return enm.Current;
        }
    }
}
