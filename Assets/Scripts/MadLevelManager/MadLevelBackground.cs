using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelBackground : MonoBehaviour
	{
		public MadDraggable draggable;

		public int startDepth = -20;

		public bool ignoreYMovement;

		public bool ignoreXMovement;

		[NonSerialized]
		public List<MadLevelBackgroundLayer> layers = new List<MadLevelBackgroundLayer>();

		public Vector2 UserPosition
		{
			get
			{
				if (draggable == null)
				{
					return Vector2.zero;
				}
				Vector3 localPosition = draggable.transform.localPosition;
				float x = ignoreXMovement ? 0f : localPosition.x;
				float y = ignoreYMovement ? 0f : localPosition.y;
				return new Vector2(x, y);
			}
		}

		private void OnEnable()
		{
			UpdateLayers();
		}

		private void Update()
		{
			if (Application.isEditor)
			{
				UpdateLayers();
			}
			UpdateDepth();
		}

		private void UpdateLayers()
		{
			layers.Clear();
			layers.AddRange(MadTransform.FindChildren<MadLevelBackgroundLayer>(base.transform));
			layers = (from o in layers
				orderby o.name
				select o).ToList();
			foreach (MadLevelBackgroundLayer layer in layers)
			{
				MadSprite component = layer.GetComponent<MadSprite>();
				component.hideFlags = HideFlags.HideInInspector;
			}
		}

		public void UpdateDepth()
		{
			int num = startDepth;
			foreach (MadLevelBackgroundLayer layer in layers)
			{
				MadSprite component = layer.GetComponent<MadSprite>();
				component.guiDepth = num++;
				layer.Update();
			}
		}

		public void RemoveLayer(MadLevelBackgroundLayer layer)
		{
			MadGameObject.SafeDestroy(layer.gameObject);
			layers.Remove(layer);
		}

		public int IndexOf(MadLevelBackgroundLayer layer)
		{
			return layers.IndexOf(layer);
		}
	}
}
