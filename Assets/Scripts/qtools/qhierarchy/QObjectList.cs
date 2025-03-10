using System.Collections.Generic;
using UnityEngine;

namespace qtools.qhierarchy
{
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class QObjectList : MonoBehaviour, ISerializationCallbackReceiver
	{
		public static List<QObjectList> instances = new List<QObjectList>();

		public List<GameObject> lockedObjects = new List<GameObject>();

		public List<GameObject> editModeVisibileObjects = new List<GameObject>();

		public List<GameObject> editModeInvisibleObjects = new List<GameObject>();

		public List<GameObject> wireframeHiddenObjects = new List<GameObject>();

		public Dictionary<GameObject, Color> gameObjectColor = new Dictionary<GameObject, Color>();

		public List<GameObject> gameObjectColorKeys = new List<GameObject>();

		public List<Color> gameObjectColorValues = new List<Color>();

		public void Awake()
		{
			checkIntegrity();
			foreach (GameObject editModeVisibileObject in editModeVisibileObjects)
			{
				editModeVisibileObject.SetActive(!Application.isPlaying);
			}
			foreach (GameObject editModeInvisibleObject in editModeInvisibleObjects)
			{
				editModeInvisibleObject.SetActive(Application.isPlaying);
			}
			if (!Application.isEditor && Application.isPlaying)
			{
				instances.Remove(this);
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			instances.RemoveAll((QObjectList item) => item == null);
			if (!instances.Contains(this))
			{
				instances.Add(this);
			}
		}

		public void OnEnable()
		{
			if (!instances.Contains(this))
			{
				instances.Add(this);
			}
		}

		public void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				checkIntegrity();
				foreach (GameObject editModeVisibileObject in editModeVisibileObjects)
				{
					editModeVisibileObject.SetActive(value: false);
				}
				foreach (GameObject editModeInvisibleObject in editModeInvisibleObjects)
				{
					editModeInvisibleObject.SetActive(value: true);
				}
				foreach (GameObject lockedObject in lockedObjects)
				{
					lockedObject.hideFlags &= ~HideFlags.NotEditable;
				}
				instances.Remove(this);
			}
		}

		public void merge(QObjectList anotherInstance)
		{
			for (int num = anotherInstance.lockedObjects.Count - 1; num >= 0; num--)
			{
				if (!lockedObjects.Contains(anotherInstance.lockedObjects[num]))
				{
					lockedObjects.Add(anotherInstance.lockedObjects[num]);
				}
			}
			for (int num2 = anotherInstance.editModeVisibileObjects.Count - 1; num2 >= 0; num2--)
			{
				if (!editModeVisibileObjects.Contains(anotherInstance.editModeVisibileObjects[num2]))
				{
					editModeVisibileObjects.Add(anotherInstance.editModeVisibileObjects[num2]);
				}
			}
			for (int num3 = anotherInstance.editModeInvisibleObjects.Count - 1; num3 >= 0; num3--)
			{
				if (!editModeInvisibleObjects.Contains(anotherInstance.editModeInvisibleObjects[num3]))
				{
					editModeInvisibleObjects.Add(anotherInstance.editModeInvisibleObjects[num3]);
				}
			}
			for (int num4 = anotherInstance.wireframeHiddenObjects.Count - 1; num4 >= 0; num4--)
			{
				if (!wireframeHiddenObjects.Contains(anotherInstance.wireframeHiddenObjects[num4]))
				{
					wireframeHiddenObjects.Add(anotherInstance.wireframeHiddenObjects[num4]);
				}
			}
			for (int num5 = anotherInstance.gameObjectColorKeys.Count - 1; num5 >= 0; num5--)
			{
				if (!gameObjectColorKeys.Contains(anotherInstance.gameObjectColorKeys[num5]))
				{
					gameObjectColorKeys.Add(anotherInstance.gameObjectColorKeys[num5]);
					gameObjectColorValues.Add(anotherInstance.gameObjectColorValues[num5]);
					gameObjectColor.Add(anotherInstance.gameObjectColorKeys[num5], anotherInstance.gameObjectColorValues[num5]);
				}
			}
		}

		public void checkIntegrity()
		{
			lockedObjects.RemoveAll((GameObject item) => item == null);
			editModeVisibileObjects.RemoveAll((GameObject item) => item == null);
			editModeInvisibleObjects.RemoveAll((GameObject item) => item == null);
			wireframeHiddenObjects.RemoveAll((GameObject item) => item == null);
			for (int num = gameObjectColorKeys.Count - 1; num >= 0; num--)
			{
				if (gameObjectColorKeys[num] == null)
				{
					gameObjectColorKeys.RemoveAt(num);
					gameObjectColorValues.RemoveAt(num);
				}
			}
			OnAfterDeserialize();
		}

		public void OnBeforeSerialize()
		{
			gameObjectColorKeys.Clear();
			gameObjectColorValues.Clear();
			foreach (KeyValuePair<GameObject, Color> item in gameObjectColor)
			{
				gameObjectColorKeys.Add(item.Key);
				gameObjectColorValues.Add(item.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			gameObjectColor.Clear();
			for (int i = 0; i < gameObjectColorKeys.Count; i++)
			{
				gameObjectColor.Add(gameObjectColorKeys[i], gameObjectColorValues[i]);
			}
		}
	}
}
