
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections;
using System.Collections.Generic;

namespace Knit.Animator.Simple
{
	[RequireComponent( typeof( UnityEngine.Animator))]
	public sealed class SingleAnimator : MonoBehaviour, IAnimationClipSource
	{
		public event UnityAction<SingleAnimator> OnDone
		{
			add{ m_OnDone.AddListener( value); }
			remove{ m_OnDone.RemoveListener( value); }
		}
		public void Play()
		{
			if( m_Created == false)
			{
				CreateGraph();
			}
			if( m_Graph.IsValid() != false && m_Playing == null)
			{
				m_Playable.SetTime( 0);
				m_Playing = OnMonitor();
				m_Graph.Play();
				
				if( m_Clip.isLooping != false && enabled != false && gameObject.activeInHierarchy != false)
				{
					m_Coroutine = StartCoroutine( m_Playing);
				}
			}
		}
		public void Play( AnimationClip animationClip)
		{
			if( (animationClip?.legacy ?? true) == false && m_Clip != animationClip)
			{
				Stop();
				DisposeGraph();
				m_Clip = animationClip;
				CreateGraph();
				Play();
			}
		}
		public void Stop()
		{
			if( m_Playing != null)
			{
				if( m_Coroutine != null)
				{
					StopCoroutine( m_Coroutine);
					m_Coroutine = null;
				}
				m_Playing = null;
				m_Graph.Stop();
			}
		}
		public bool IsPlaying()
		{
			return m_Playing != null;
		}
		void CreateGraph()
		{
			if( (m_Clip?.legacy ?? true) == false)
			{
				if( m_Clip.isLooping == false && m_Clip.wrapMode == WrapMode.Default)
				{
					m_Clip.wrapMode = WrapMode.Once;
				}
				m_Graph = PlayableGraph.Create();
				m_Playable = AnimationClipPlayable.Create( m_Graph, m_Clip);
				
				if( m_Clip.isLooping == false)
				{
					m_Playable.SetDuration( m_Clip.length);
				}
				m_Playable.SetSpeed( m_SpeedMultiplier);
				m_Playable.SetApplyFootIK( false);
				m_Playable.SetApplyPlayableIK( false);
				AnimationPlayableOutput.Create( m_Graph, "AnimationClip", 
					GetComponent<UnityEngine.Animator>()).SetSourcePlayable( m_Playable);
			}
		#if UNITY_EDITOR
			m_CacheClip = m_Clip;
		#endif
			m_Created = true;
		}
		void DisposeGraph()
		{
			if( m_Created != false)
			{
				if( m_Graph.IsValid() != false)
				{
					m_Graph.Destroy();
				}
				if( m_Playable.IsValid() != false)
				{
					m_Playable.Destroy();
				}
				m_Created = false;
			}
		}
		IEnumerator OnMonitor()
		{
			double duration = m_Playable.GetDuration();
			
			while( m_Playable.GetTime() < duration)
			{
				yield return null;
			}
			m_OnDone.Invoke( this);
			Stop();
		}
		void Awake()
		{
			CreateGraph();
		}
		void OnEnable()
		{
			if( m_Playing == null && m_PlayOnAwake != PlayOnAwake.None)
			{
				Play();
			}
			if( m_Playing != null)
			{
				if( m_PlayOnAwake == PlayOnAwake.Restart)
				{
					m_Playable.SetTime( 0);
				}
				if( m_Clip.isLooping != false)
				{
					m_Coroutine = StartCoroutine( m_Playing);
				}
				m_Graph.Play();
			}
		}
		void OnDisable()
		{
			if( m_Playing != null)
			{
				if( m_Coroutine != null)
				{
					StopCoroutine( m_Coroutine);
					m_Coroutine = null;
				}
				m_Graph.Stop();
			}
		}
		void OnDestroy()
		{
			DisposeGraph();
		}
		public void GetAnimationClips( List<AnimationClip> results)
		{
			if( m_Clip != null)
			{
				results.Add( m_Clip);
			}
		}
	#if UNITY_EDITOR
		void OnValidate()
		{
			if( m_CacheClip != m_Clip)
			{
				bool isPlaying = IsPlaying();
				
				DisposeGraph();
				CreateGraph();
				
				if( isPlaying != false)
				{
					Play();
				}
				m_CacheClip = m_Clip;
			}
			if( m_Playable.IsValid() != false)
			{
				m_Playable.SetSpeed( m_SpeedMultiplier);
			}
		}
		AnimationClip m_CacheClip;
	#endif
		enum PlayOnAwake
		{
			None,
			Resume,
			Restart,
		}
		[System.Serializable]
		sealed class DoneEvent : UnityEvent<SingleAnimator>{}
		[SerializeField]
		AnimationClip m_Clip;
		[SerializeField]
		PlayOnAwake m_PlayOnAwake = PlayOnAwake.Restart;
		[SerializeField]
		float m_SpeedMultiplier = 1.0f;
		[SerializeField]
		DoneEvent m_OnDone = new();
		[System.NonSerialized]
		PlayableGraph m_Graph;
		[System.NonSerialized]
		AnimationClipPlayable m_Playable;
		[System.NonSerialized]
		bool m_Created;
		[System.NonSerialized]
		IEnumerator m_Playing;
		[System.NonSerialized]
		Coroutine m_Coroutine;
	}
}