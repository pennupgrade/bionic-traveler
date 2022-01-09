namespace BionicTraveler
{
    using BionicTraveler.Scripts.Quests;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    [CustomEditor(typeof(QuestStage), true)]
    public class QuestStageEditor : Editor
    {
        protected static string ABILITY_FACTORY_NAME = "objective";
        protected QuestStage questStateObject;
        protected SerializedObject serializedMage;
        protected SerializedProperty SerializedAbility;
        void OnEnable()
        {
            questStateObject = (QuestStage)target;
            serializedMage = new SerializedObject(questStateObject);
            SerializedAbility = serializedMage.FindProperty(ABILITY_FACTORY_NAME);
        }
        public override void OnInspectorGUI()
        {
            serializedMage.Update();
            //using excluding, instead of marking primary spell HideInInspector because it will mess up the serialized property
            DrawPropertiesExcluding(serializedMage, new string[] { ABILITY_FACTORY_NAME });
            DrawPrimarySpell();
            serializedMage.ApplyModifiedProperties();
        }

        protected void DrawPrimarySpell()
        {
            //display ability Type. TODO: Uppercase first letter.
            EditorGUILayout.LabelField(ABILITY_FACTORY_NAME, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(SerializedAbility.FindPropertyRelative("Type"));

            //display relevant ability properties based on ability type
            QuestObjectiveFactory abilityFactory = questStateObject.GetFactory();
            if (abilityFactory.Type == QuestObjectiveType.None)
            {
                return;
            }

            System.Type typeOfAbility = abilityFactory.GetClassType(abilityFactory.Type);
            SerializedProperty specificAbility = (SerializedAbility.FindPropertyRelative(typeOfAbility.Name.ToString())).Copy();
            string parentPath = specificAbility.propertyPath;
            while (specificAbility.NextVisible(true) && specificAbility.propertyPath.StartsWith(parentPath))
            {
                EditorGUILayout.PropertyField(specificAbility);
            }
        }
    }
}
