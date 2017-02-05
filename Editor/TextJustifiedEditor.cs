using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Labels.
    /// </summary>

    [CustomEditor(typeof(TextJustified), true)]
    [CanEditMultipleObjects]
    public class TextJustifiedEditor : GraphicEditor
    {
        Text text = null;
        SerializedProperty m_Text = null;
        SerializedProperty m_FontData = null;
        SerializedProperty m_Justified = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_Justified = serializedObject.FindProperty("m_Justified");
        }

        public override void OnInspectorGUI()
        {
            if (m_Text == null)
                m_Text = serializedObject.FindProperty("m_Text");
            if (m_FontData == null)
                m_FontData = serializedObject.FindProperty("m_FontData");
            if (m_Justified == null)
                m_Justified = serializedObject.FindProperty("m_Justified");

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            EditorGUILayout.PropertyField(m_Justified);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}