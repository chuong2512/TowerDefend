using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	public abstract class MadAnim : MonoBehaviour
	{
		public enum WrapMode
		{
			Once,
			Loop,
			PingPong,
			ClampForever
		}

		public string animationName = "New Animation";

		public MadiTween.EaseType easing = MadiTween.EaseType.easeOutCubic;

		public bool useAnimationCurve;

		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

		public float duration = 1f;

		public float delay;

		public float offset;

		public WrapMode wrapMode;

		public bool queue;

		public bool playOnAwake;

		public bool destroyObjectOnFinish;

		public bool sendMessageOnFinish;

		public GameObject messageReceiver;

		public string messageName;

		public bool playAnimationOnFinish;

		public string playAnimationOnFinishName;

		public bool playAnimationOnFinishFromTheBeginning;

		public bool ignoreTimeScale;

		private float delayTime;

		private float playTime;

		private bool firstFrame = true;

		private bool startAnimInvoked;

		private bool hasOrigin;

		private string animationQueue;

		private float lastTime;

		public bool isPlaying
		{
			get;
			private set;
		}

		public bool isDelaying
		{
			get;
			private set;
		}

		protected float deltaTime
		{
			get
			{
				if (!ignoreTimeScale)
				{
					return Time.deltaTime;
				}
				if (Mathf.Approximately(lastTime, 0f))
				{
					return 0f;
				}
				return Time.realtimeSinceStartup - lastTime;
			}
		}

		public void Play()
		{
			if (!isPlaying)
			{
				if (delayTime >= delay)
				{
					TryStartPlaying();
				}
				else
				{
					isDelaying = true;
				}
				firstFrame = true;
			}
		}

		public void PlayNow()
		{
			if (!isPlaying)
			{
				isDelaying = false;
				firstFrame = true;
				TryStartPlaying();
			}
		}

		public void Stop()
		{
			isPlaying = false;
		}

		public void Reset()
		{
			delayTime = 0f;
			playTime = offset * duration;
			startAnimInvoked = false;
		}

		protected MadiTween.EasingFunction GetEasingFunction()
		{
			if (useAnimationCurve)
			{
				return EasingFromCurve;
			}
			return MadiTween.GetEasingFunction(easing);
		}

		private float EasingFromCurve(float start, float end, float value)
		{
			float num = end - start;
			return start + animationCurve.Evaluate(value) * num;
		}

		protected abstract void Anim(float progress);

		protected virtual void Start()
		{
			if (!hasOrigin)
			{
				UpdateOrigin();
			}
			if (playOnAwake)
			{
				Play();
			}
		}

		private void TryStartPlaying()
		{
			MadAnim madAnim = OtherAnimationPlaying();
			if (madAnim != null)
			{
				if (!queue)
				{
					madAnim.Stop();
					isPlaying = true;
					return;
				}
				if (!string.IsNullOrEmpty(animationQueue))
				{
					UnityEngine.Debug.LogWarning("Animation queue cannot contain more than one animation. Please review your animations density.");
				}
				madAnim.animationQueue = animationName;
				isPlaying = false;
			}
			else
			{
				isPlaying = true;
			}
		}

		private void Update()
		{
			if (firstFrame)
			{
				firstFrame = false;
			}
			else if (isDelaying)
			{
				delayTime += deltaTime;
				if (delayTime >= delay)
				{
					isDelaying = false;
					playTime += delayTime - delay;
					TryStartPlaying();
				}
			}
			else if (isPlaying)
			{
				playTime += deltaTime;
			}
			if (isPlaying)
			{
				if (!startAnimInvoked)
				{
					StartAnim();
					startAnimInvoked = true;
				}
				switch (wrapMode)
				{
				case WrapMode.Once:
					AnimWrapOnce(playTime);
					break;
				case WrapMode.Loop:
					AnimLoop(playTime);
					break;
				case WrapMode.PingPong:
					AnimPingPong(playTime);
					break;
				case WrapMode.ClampForever:
					AnimClampForever(playTime);
					break;
				default:
					UnityEngine.Debug.LogError("Unknown wrap mode: " + wrapMode);
					break;
				}
			}
			lastTime = Time.realtimeSinceStartup;
		}

		private MadAnim OtherAnimationPlaying()
		{
			return PlayingAnimation(base.gameObject, GetType());
		}

		private void AnimWrapOnce(float animTime)
		{
			if (animTime < duration)
			{
				float progress = animTime / duration;
				Anim(progress);
			}
			else if (animTime > duration)
			{
				Finish();
			}
		}

		private void AnimLoop(float animTime)
		{
			animTime %= duration;
			float progress = animTime / duration;
			Anim(progress);
		}

		private void AnimPingPong(float animTime)
		{
			animTime %= duration * 2f;
			float num = animTime / duration;
			if (num > 1f)
			{
				num = 2f - num;
			}
			Anim(num);
		}

		private void AnimClampForever(float animTime)
		{
			float progress = Mathf.Clamp(animTime / duration, 0f, 1f);
			Anim(progress);
		}

		protected abstract void StartAnim();

		public virtual void UpdateOrigin()
		{
			hasOrigin = true;
		}

		private void Finish()
		{
			Anim(1f);
			if (sendMessageOnFinish)
			{
				GameObject gameObject = base.gameObject;
				if (messageReceiver != null)
				{
					gameObject = messageReceiver;
				}
				gameObject.SendMessage(messageName);
			}
			if (playAnimationOnFinish && !string.IsNullOrEmpty(playAnimationOnFinishName))
			{
				PlayAnimation(base.gameObject, playAnimationOnFinishName, playAnimationOnFinishFromTheBeginning);
			}
			if (destroyObjectOnFinish)
			{
				MadGameObject.SafeDestroy(base.gameObject);
			}
			isPlaying = false;
			if (!string.IsNullOrEmpty(animationQueue))
			{
				PlayAnimationNow(base.gameObject, animationQueue);
			}
		}

		public static int PlayAnimation(GameObject gameObject, string animationName, bool fromTheBeginning = false)
		{
			List<MadAnim> list = FindAnimations(gameObject, animationName);
			for (int i = 0; i < list.Count; i++)
			{
				MadAnim madAnim = list[i];
				if (fromTheBeginning)
				{
					madAnim.Reset();
				}
				madAnim.Play();
			}
			return list.Count;
		}

		public static int PlayAnimationNow(GameObject gameObject, string animationName, bool fromTheBeginning = false)
		{
			List<MadAnim> list = FindAnimations(gameObject, animationName);
			for (int i = 0; i < list.Count; i++)
			{
				MadAnim madAnim = list[i];
				if (fromTheBeginning)
				{
					madAnim.Reset();
				}
				madAnim.PlayNow();
			}
			return list.Count;
		}

		public static int StopAnimation(GameObject gameObject, string animationName)
		{
			List<MadAnim> list = FindAnimations(gameObject, animationName);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Stop();
			}
			return list.Count;
		}

		public static List<MadAnim> FindAnimations(GameObject gameObject, string name)
		{
			MadAnim[] components = gameObject.GetComponents<MadAnim>();
			IEnumerable<MadAnim> source = from anim in components
				where anim.animationName == name
				select anim;
			return source.ToList();
		}

		public static List<MadAnim> AllAnimations(GameObject gameObject)
		{
			return gameObject.GetComponents<MadAnim>().ToList();
		}

		public static MadAnim PlayingAnimation(GameObject gameObject, Type type)
		{
			Component[] components = gameObject.GetComponents(type);
			for (int i = 0; i < components.Length; i++)
			{
				MadAnim madAnim = components[i] as MadAnim;
				if (madAnim.isPlaying)
				{
					return madAnim;
				}
			}
			return null;
		}

		public static T PlayingAnimation<T>(GameObject gameObject) where T : MadAnim
		{
			T[] components = gameObject.GetComponents<T>();
			for (int i = 0; i < components.Length; i++)
			{
				T result = components[i];
				if (result.isPlaying)
				{
					return result;
				}
			}
			return (T)null;
		}
	}
}
