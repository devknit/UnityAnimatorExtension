
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor( typeof( AnimatorSingleClip))]
	sealed class AnimatorSingleClipEditor : Editor
	{
		void OnEnable()
		{
			m_ClipProperty = serializedObject.FindProperty( "m_Clip");
			m_PlayOnAwakeProperty = serializedObject.FindProperty( "m_PlayOnAwake");
			m_SpeedMultiplierProperty = serializedObject.FindProperty( "m_SpeedMultiplier");
			m_OnDoneProperty = serializedObject.FindProperty( "m_OnDone");
		}
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.PropertyField( m_ClipProperty);
			EditorGUILayout.PropertyField( m_PlayOnAwakeProperty);
			EditorGUILayout.PropertyField( m_SpeedMultiplierProperty);
			EditorGUILayout.PropertyField( m_OnDoneProperty);
			serializedObject.ApplyModifiedProperties();
        }
		SerializedProperty m_ClipProperty;
		SerializedProperty m_PlayOnAwakeProperty;
		SerializedProperty m_SpeedMultiplierProperty;
		SerializedProperty m_OnDoneProperty;
    }
}