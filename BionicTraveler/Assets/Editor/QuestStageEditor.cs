namespace BionicTraveler
{
    using BionicTraveler.Scripts.Quests;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Property drawwer to render dialogue node names.
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityInheritanceAttribute))]
    public class UnityInheritancePropertyDrawer : PropertyDrawer
    {
        private float extraHeight;

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;
            var attrib = this.attribute as UnityInheritanceAttribute;

            var obj = GetTargetObjectOfProperty(property);
            var inheritance = (UnityInheritance)obj;

            var types = inheritance.DiscoverTypes().ToArray();

            var typeNames = types.Select(t => t.FullName).ToList();
            var displayTypeNames = types.Select(t => t.Name);
            EditorGUI.BeginChangeCheck();
            var currentIndex = typeNames.IndexOf(inheritance.Type);
            bool hasNoValue = currentIndex == -1;
            EditorGUI.BeginProperty(position, null, property);
            var popupPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            currentIndex = EditorGUI.Popup(popupPosition, "Type", currentIndex, displayTypeNames.ToArray());
            if (EditorGUI.EndChangeCheck() || hasNoValue)
            {
                // Ensure we always have a valid value selected.
                var newIndex = System.Math.Max(0, currentIndex);
                inheritance.Type = typeNames[newIndex];
            }

            // Draw properties of type.
            var instance = inheritance.CreateType(inheritance.Type);

            SerializedProperty underlyingType = property.FindPropertyRelative(inheritance.InstanceName).Copy();
            underlyingType.Reset();

            // Only draw properties that are under our currentProperty.instanceName to exclude all other
            // fields that do not belong to our type.
            string parentPath = property.propertyPath + "." + inheritance.InstanceName + ".";
            this.extraHeight = popupPosition.height + 2;
            var startPosition = position;
            EditorGUI.indentLevel++;
            while (underlyingType.NextVisible(true))
            {
                if (underlyingType.propertyPath.StartsWith(parentPath))
                {
                    var propHeight = EditorGUI.GetPropertyHeight(underlyingType);
                    EditorGUI.PropertyField(new Rect(startPosition.x, startPosition.y + extraHeight, startPosition.width, propHeight),
                        underlyingType);
                    this.extraHeight += propHeight + 2;
                }
            }

            EditorGUI.indentLevel++;
            EditorGUI.EndProperty();
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return this.extraHeight + 10; // Some padding at the end.
        }
    }

    /// <summary>
    /// Please document me.
    /// </summary>
    //[CustomEditor(typeof(QuestStage), true)]
    //public class QuestStageEditor : Editor
    //{
    //    protected static string ABILITY_FACTORY_NAME = "objective";
    //    protected QuestStage questStateObject;
    //    protected SerializedObject serializedMage;
    //    protected SerializedProperty SerializedAbility;
    //    void OnEnable()
    //    {
    //        questStateObject = (QuestStage)target;
    //        serializedMage = new SerializedObject(questStateObject);
    //        SerializedAbility = serializedMage.FindProperty(ABILITY_FACTORY_NAME);
    //    }
    //    public override void OnInspectorGUI()
    //    {
    //        serializedMage.Update();
    //        //using excluding, instead of marking primary spell HideInInspector because it will mess up the serialized property
    //        DrawPropertiesExcluding(serializedMage, new string[] { ABILITY_FACTORY_NAME });
    //        DrawPrimarySpell();
    //        serializedMage.ApplyModifiedProperties();
    //    }

    //    protected void DrawPrimarySpell()
    //    {
    //        //display ability Type. TODO: Uppercase first letter.
    //        EditorGUILayout.LabelField(ABILITY_FACTORY_NAME, EditorStyles.boldLabel);
    //        EditorGUILayout.PropertyField(SerializedAbility.FindPropertyRelative("Type"));

    //        //display relevant ability properties based on ability type
    //        QuestObjectiveFactory abilityFactory = questStateObject.GetFactory();
    //        if (abilityFactory.Type == QuestObjectiveType.None)
    //        {
    //            return;
    //        }

    //        System.Type typeOfAbility = abilityFactory.GetClassType(abilityFactory.Type);
    //        SerializedProperty specificAbility = (SerializedAbility.FindPropertyRelative(typeOfAbility.Name.ToString())).Copy();
    //        string parentPath = specificAbility.propertyPath;
    //        while (specificAbility.NextVisible(true) && specificAbility.propertyPath.StartsWith(parentPath))
    //        {
    //            EditorGUILayout.PropertyField(specificAbility);
    //        }
    //    }
    //}
}
