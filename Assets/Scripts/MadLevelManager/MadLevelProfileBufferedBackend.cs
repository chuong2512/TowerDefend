using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public abstract class MadLevelProfileBufferedBackend : IMadLevelProfileBackend
	{
		private Dictionary<string, string> profileValues = new Dictionary<string, string>();

		private bool started;

		private float _maxTimePause = 16f;

		protected MadLevelProfileWatcher profileWatcher;

		public float maxTimePause
		{
			get
			{
				return _maxTimePause;
			}
			set
			{
				_maxTimePause = value;
			}
		}

		protected MadLevelProfileBufferedBackend()
		{
			if (Application.isPlaying)
			{
				profileWatcher = MadTransform.GetOrCreateChild<MadLevelProfileWatcher>(null, "_MLM_ProfileWatcher");
				profileWatcher.Watch(this);
			}
		}

		public IEnumerator Run()
		{
			started = true;
			while (true)
			{
				yield return new WaitForSeconds(maxTimePause);
				if (profileValues.Count > 0)
				{
					Flush();
				}
			}
		}

		public abstract void Start();

		public abstract string LoadProfile(string profileName);

		public void SaveProfile(string profileName, string value)
		{
			profileValues[profileName] = value;
			if (!started)
			{
				Flush();
			}
		}

		public void Flush()
		{
			foreach (string key in profileValues.Keys)
			{
				Flush(key, profileValues[key]);
			}
			profileValues.Clear();
		}

		public abstract bool CanWorkInEditMode();

		protected abstract void Flush(string profileName, string value);
	}
}
