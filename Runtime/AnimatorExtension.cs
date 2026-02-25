
namespace UnityEngine
{
	public static class AnimatorExtension
	{
		public static void ClearAnimationEvent( this AnimationClip clip)
		{
			if( clip != null)
			{
				clip.events = System.Array.Empty<AnimationEvent>();
			}
		}
		public static void AddAnimationEvent( this AnimationClip clip, string methodName, float t=-1)
		{
			if( clip != null)
			{
				if( t < 0.0f)
				{
					t = clip.length;
				}
				clip.AddEvent( new AnimationEvent
				{
					functionName = methodName,
					time = t
				});
			}
		}
	}
}