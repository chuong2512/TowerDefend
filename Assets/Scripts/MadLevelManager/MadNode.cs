using UnityEngine;

namespace MadLevelManager
{
	public class MadNode : MonoBehaviour
	{
		[HideInInspector]
		public bool managed;

		public static bool Instantiating
		{
			get;
			private set;
		}

		private MadRootNode Root
		{
			get
			{
				if (this is MadRootNode)
				{
					return (MadRootNode)this;
				}
				Transform transform = base.transform;
				do
				{
					transform = transform.parent;
					MadRootNode component = transform.GetComponent<MadRootNode>();
					if (component != null)
					{
						return component;
					}
				}
				while (transform.parent != null);
				UnityEngine.Debug.LogError("Missing root node?!");
				return null;
			}
		}

		public T CreateChild<T>(string name) where T : MadNode
		{
			GameObject gameObject = null;
			try
			{
				Instantiating = true;
				gameObject = new GameObject(name);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = Vector3.zero;
			}
			finally
			{
				Instantiating = false;
			}
			return gameObject.AddComponent<T>();
		}

		public T CreateChild<T>(string name, T template) where T : MadNode
		{
			return (T)CreateChild(name, template.gameObject);
		}

		public MadNode CreateChild(string name, GameObject template)
		{
			GameObject gameObject = null;
			try
			{
				Instantiating = true;
				gameObject = UnityEngine.Object.Instantiate(template);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.name = name;
			}
			finally
			{
				Instantiating = false;
			}
			MadNode component = gameObject.GetComponent<MadNode>();
			if (component == null)
			{
				UnityEngine.Debug.LogError("Template doesn't have Node component");
				return null;
			}
			return component;
		}

		public T FindParent<T>() where T : Component
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				T component = parent.GetComponent<T>();
				if ((Object)component != (Object)null)
				{
					return component;
				}
				parent = parent.parent;
			}
			return (T)null;
		}
	}
}
