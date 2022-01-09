namespace BionicTraveler
{
    using BionicTraveler.Scripts.Quests;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
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

            var inheritanceField = targetObject.GetType().GetField(property.name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var inheritance = (UnityInheritance)inheritanceField.GetValue(targetObject);

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
            string parentPath = property.name + "." + inheritance.InstanceName + ".";
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
