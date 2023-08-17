using UnityEditor;
using UnityEngine;

namespace EVRC.Core.Editor
{
    [CustomEditor(typeof(EDStatusFlagsEvent))]
    public class StatusFlagsEventEditor : UnityEditor.Editor
    {
        private EDStatusFlags statusFlags;
        private EDStatusFlags2 statusFlags2;

        private void OnEnable()
        {
            statusFlags = EDStatusFlags.InMainShip;
            statusFlags2 = EDStatusFlags2.OnFoot;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status Flags:");
            statusFlags = (EDStatusFlags)EditorGUILayout.EnumFlagsField(statusFlags);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status Flags2:");
            statusFlags2 = (EDStatusFlags2)EditorGUILayout.EnumFlagsField(statusFlags2);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Raise"))
            {
                ((EDStatusFlagsEvent)target).Raise(statusFlags, statusFlags2);
                Debug.Log($"Raising EdStatusAndGuiEvent with: {statusFlags}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(EDGuiFocusEvent))]
    public class YourScriptableObjectEditor : UnityEditor.Editor
    {
        private EDGuiFocus guiFocus;

        private void OnEnable()
        {
            guiFocus = EDGuiFocus.NoFocus;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ED Gui Focus");
            guiFocus = (EDGuiFocus)EditorGUILayout.EnumPopup(guiFocus);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Raise"))
            {
                ((EDGuiFocusEvent)target).Raise(guiFocus);
                Debug.Log($"Raising EdStatusAndGuiEvent with: {guiFocus}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
