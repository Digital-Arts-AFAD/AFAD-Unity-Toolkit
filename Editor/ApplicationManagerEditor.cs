using UnityEngine;
using UnityEditor;

namespace DigitalArtsAFAD
{
    [CustomEditor(typeof(ApplicationManager))]
    [CanEditMultipleObjects]
    public class ApplicationManagerEditor : Editor
    {
        bool showMainMenu = false;
        bool showModals = false;
        bool showSceneLoading = false;

        SerializedProperty[] properties;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("m_Script")
                );
            }

            showMainMenu = EditorGUILayout.Foldout(showMainMenu, "UI");
            if (showMainMenu)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenu"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fader"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cursor"));
            }
            showModals = EditorGUILayout.Foldout(showModals, "Modals");
            if (showModals)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stopTimeOnModal"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onModalShow"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onModalHide"));
            }
            showSceneLoading = EditorGUILayout.Foldout(showSceneLoading, "Scene loading");
            if (showSceneLoading)
            {
                SerializedProperty sceneMode = serializedObject.FindProperty("sceneMode");
                EditorGUILayout.PropertyField(sceneMode);
                if (sceneMode.enumValueIndex == (int)SceneMode.ADDITIVE)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultScene"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("disableObjectsDuringLoading"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("enableObjectsDuringLoading"), true);
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onLoadingStarted"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onLoadingFinished"));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}