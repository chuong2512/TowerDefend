using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadAnchor : MadNode
	{
		public enum Mode
		{
			ScreenAnchor,
			ObjectAnchor
		}

		public enum Position
		{
			Left,
			Top,
			Right,
			Bottom,
			TopLeft,
			TopRight,
			BottomRight,
			BottomLeft,
			Center
		}

		public Mode mode;

		public Position position;

		public GameObject anchorObject;

		public Camera anchorCamera;

		private MadRootNode _root;

		private MadPanel _panel;

		private MadRootNode root
		{
			get
			{
				if (_root == null)
				{
					_root = MadTransform.FindParent<MadRootNode>(base.transform);
				}
				return _root;
			}
		}

		private MadPanel panel
		{
			get
			{
				if (_panel == null)
				{
					_panel = MadTransform.FindParent<MadPanel>(base.transform);
					if (_panel == null)
					{
						UnityEngine.Debug.LogError("Anchor can be set only under the panel", this);
					}
				}
				return _panel;
			}
		}

		private void Start()
		{
		}

		public void Update()
		{
			switch (mode)
			{
			case Mode.ScreenAnchor:
				UpdateScreenAnchor();
				break;
			case Mode.ObjectAnchor:
				UpdateObjectAnchor();
				break;
			default:
				MadDebug.Assert(condition: false, "Unknown mode: " + mode);
				break;
			}
		}

		private void UpdateScreenAnchor()
		{
			Vector3 vector = FromPosition(position);
			MadTransform.SetPosition(base.transform, vector);
		}

		private Vector3 FromPosition(Position position)
		{
			float x = 0f;
			float y = 0f;
			switch (position)
			{
			case Position.Left:
				x = 0f;
				y = 0.5f;
				break;
			case Position.Top:
				y = 1f;
				x = 0.5f;
				break;
			case Position.Right:
				x = 1f;
				y = 0.5f;
				break;
			case Position.Bottom:
				y = 0f;
				x = 0.5f;
				break;
			case Position.TopLeft:
				x = 0f;
				y = 1f;
				break;
			case Position.TopRight:
				x = 1f;
				y = 1f;
				break;
			case Position.BottomRight:
				x = 1f;
				y = 0f;
				break;
			case Position.BottomLeft:
				x = 0f;
				y = 0f;
				break;
			case Position.Center:
				x = 0.5f;
				y = 0.5f;
				break;
			default:
				MadDebug.Assert(condition: false, "Unknown option: " + position);
				break;
			}
			return root.ScreenGlobal(x, y);
		}

		private void UpdateObjectAnchor()
		{
			if (anchorObject == null)
			{
				return;
			}
			Camera main = anchorCamera;
			if (main == null)
			{
				if (Application.isPlaying)
				{
					MadDebug.LogOnce("Anchor camera not set. Using main camera.", this);
				}
				main = Camera.main;
				if (main == null)
				{
					UnityEngine.Debug.LogWarning("There's no camera tagged as MainCamera on this scene. Please make sure that there is one or assign a custom camera to this anchor object.", this);
					return;
				}
			}
			Vector3 vector = panel.WorldToPanel(main, anchorObject.transform.position);
			MadTransform.SetPosition(base.transform, vector);
		}
	}
}
