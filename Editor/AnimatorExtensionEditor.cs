
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityEditor
{
	public static class AnimatorExtensionEditor
	{
		sealed class FontAssetBuildProcessor : IPreprocessBuildWithReport
		{
			public void OnPreprocessBuild( BuildReport report)
			{
				Clean( true);
			}
			public int callbackOrder
			{
				get{ return 0; }
			}
		}
		sealed class FontAssetSaveProcessor : AssetModificationProcessor
		{
			static string[] OnWillSaveAssets( string[] paths)
			{
				Clean( false);
				return paths;
			}
		}
		[MenuItem( "Window/Animation/Clear Animation Event")]
		static void Clean()
		{
			Clean( true);
		}
		static void Clean( bool save)
		{
			string[] assetGUIDs = AssetDatabase.FindAssets( "t:AnimationClip");
			if( (assetGUIDs?.Length ?? 0) > 0)
			{
				foreach( string assetGUID in assetGUIDs)
				{
					string assetPath = AssetDatabase.GUIDToAssetPath( assetGUID);
					AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>( assetPath);
					if( clip != null)
					{
						AnimationUtility.SetAnimationEvents( clip, System.Array.Empty<AnimationEvent>());
						EditorUtility.SetDirty( clip);
					}
				}
				if( save != false)
				{
					AssetDatabase.SaveAssets();
				}
			}
		}
	}
}