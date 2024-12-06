using UnityEditor;
using UnityEngine;

namespace CavrnusSdk.Experimental.Editor
{
    [CustomEditor(typeof(CavrnusPropertyObject<>), true)]
    public class CavrnusPropertyObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty propertyObjectType;
        private SerializedProperty propertyObjectJournalType;

        private SerializedProperty containerName;
        private SerializedProperty propertyName;
        private SerializedProperty defaultValue;

        private SerializedProperty isUserMetadata;
        private SerializedProperty allowRepeatTransientValues;

        private void OnEnable()
        {
            // General Property Object Settings
            propertyObjectType = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.PropertyObjectContainerType));
            propertyObjectJournalType = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.PropertyObjectJournalType));
            isUserMetadata = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.IsUserMetadata));

            // Container and Default Values
            containerName = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.ContainerName));
            propertyName = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.PropertyName));
            defaultValue = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.DefaultValue));

            // Transient Settings
            allowRepeatTransientValues = serializedObject.FindProperty(nameof(CavrnusPropertyObject<object>.AllowRepeatTransientValues));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Cavrnus Property Object Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Property Object Type
            EditorGUILayout.PropertyField(propertyObjectType, new GUIContent("Property Container Type"));

            if (propertyObjectType.enumValueFlag == 0) // User-specific settings
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("User-Specific Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(isUserMetadata, new GUIContent("Is User Metadata"));
            }
            else if (propertyObjectType.enumValueFlag == 1) // Space-specific settings
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Space-Specific Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(containerName, new GUIContent("Container Name"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Property Details
            EditorGUILayout.LabelField("Property Details", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(propertyName, new GUIContent("Property Name"));
            EditorGUILayout.PropertyField(defaultValue, new GUIContent("Default Value"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Journal Settings
            EditorGUILayout.LabelField("Journal Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(propertyObjectJournalType, new GUIContent("Journal Type"));

            if (propertyObjectJournalType.enumValueFlag == 1) // Transient-specific settings
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Transient Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(allowRepeatTransientValues, new GUIContent("Allow Repeat Transient Values"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
