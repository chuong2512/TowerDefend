using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MadBigMeshRenderer))]
	[RequireComponent(typeof(MadMaterialStore))]
	public class MadPanel : MadNode
	{
		public delegate void Event1<T>(T t);

		public enum RenderMode
		{
			Legacy,
			DepthBased
		}

		private static List<MadPanel> panels = new List<MadPanel>();

		public RenderMode renderMode;

		public bool halfPixelOffset = true;

		public bool hideInvisibleSprites;

		[NonSerialized]
		public bool ignoreInput;

		[NonSerialized]
		public HashSet<MadSprite> sprites = new HashSet<MadSprite>();

		private MadMaterialStore _materialStore;

		[HideInInspector]
		private MadSprite _focusedSprite;

		private int _focusedSpriteModCount;

		private Camera _currentCamera;

		public List<GameObject> unityUiIgnore = new List<GameObject>();

		private StandaloneInputModule uGUIStandaloneInputModule;

		private MethodInfo uGUIGetLastPointerMethod;

		private List<RaycastResult> uGUIRaycastResult;

		private object[] uGUIGetLastPointerMethodArgs;

		private HashSet<MadSprite> hoverSprites = new HashSet<MadSprite>();

		private bool haveTouch;

		private HashSet<MadSprite> touchDownSprites = new HashSet<MadSprite>();

		private static HashSet<MadSprite> EmptyMadSpriteHashSet = new HashSet<MadSprite>();

		private static List<MadSprite> EmptyMadSpriteList = new List<MadSprite>();

		public MadMaterialStore materialStore
		{
			get
			{
				if (_materialStore == null)
				{
					_materialStore = base.gameObject.AddComponent<MadMaterialStore>();
				}
				return _materialStore;
			}
			private set
			{
				_materialStore = value;
			}
		}

		public MadSprite focusedSprite
		{
			get
			{
				return _focusedSprite;
			}
			set
			{
				_focusedSprite = value;
				_focusedSpriteModCount++;
				if (this.onFocusChanged != null)
				{
					this.onFocusChanged(_focusedSprite);
				}
			}
		}

		public Camera currentCamera
		{
			get
			{
				if (_currentCamera == null || (_currentCamera.cullingMask & (1 << base.gameObject.layer)) == 0)
				{
					_currentCamera = null;
					Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
					foreach (Camera camera in array)
					{
						if ((camera.cullingMask & (1 << base.gameObject.layer)) != 0)
						{
							if (_currentCamera != null)
							{
								UnityEngine.Debug.Log("There are multiple cameras that are rendering the \"" + LayerMask.LayerToName(base.gameObject.layer) + "\" layer. Please adjust your culling masks and/or change layer of this Panel object.", this);
							}
							else
							{
								_currentCamera = camera;
							}
						}
					}
				}
				return _currentCamera;
			}
		}

		public event Event1<MadSprite> onFocusChanged;

		private void OnGUI()
		{
			if (MadTrial.isTrialVersion)
			{
				MadTrial.InfoLabel("This is an evaluation version of Mad Level Manager");
			}
		}

		public Vector3 WorldToPanel(Camera worldCamera, Vector3 worldPos)
		{
			Vector3 vector = worldCamera.WorldToScreenPoint(worldPos);
			vector = currentCamera.ScreenToWorldPoint(vector);
			vector.z = 0f;
			return vector;
		}

		private void OnEnable()
		{
			Unity5Check();
			panels.Add(this);
			materialStore = GetComponent<MadMaterialStore>();
			MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				MadGameObject.SafeDestroy(component);
			}
			MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				MadGameObject.SafeDestroy(component2);
			}
		}

		private void Start()
		{
			TryInitializeUnityUI();
		}

		private void TryInitializeUnityUI()
		{
			if (Application.isPlaying && EventSystem.current != null)
			{
				uGUIStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
				Type type = uGUIStandaloneInputModule.GetType();
				uGUIGetLastPointerMethod = type.GetMethod("GetLastPointerEventData", BindingFlags.Instance | BindingFlags.NonPublic);
				uGUIRaycastResult = new List<RaycastResult>();
				uGUIGetLastPointerMethodArgs = new object[1]
				{
					-1
				};
			}
		}

		private void Unity5Check()
		{
			if (!Application.isPlaying)
			{
			}
		}

		private void OnDisable()
		{
			panels.Remove(this);
		}

		private void Update()
		{
			if (halfPixelOffset)
			{
				MadRootNode madRootNode = FindParent<MadRootNode>();
				float pixelSize = madRootNode.pixelSize;
				float x = 0f;
				float y = 0f;
				if (Screen.height % 2 == 0)
				{
					y = pixelSize;
				}
				if (Screen.width % 2 == 0)
				{
					x = pixelSize;
				}
				MadTransform.SetLocalPosition(base.transform, new Vector3(x, y, 0f));
			}
			UpdateInput();
		}

		private void UpdateInput()
		{
			if (!ignoreInput)
			{
				UpdateTouchInput();
				if (Application.isEditor)
				{
					UpdateMouseInput();
				}
			}
		}

		private void UpdateTouchInput()
		{
			Touch[] touches = Input.touches;
			HashSet<MadSprite> hashSet = new HashSet<MadSprite>();
			bool ignorePointer = false;
			CheckIgnorePointer(ref ignorePointer);
			haveTouch = (UnityEngine.Input.touchCount > 0);
			if (touches.Length == 1)
			{
				Touch touch = touches[0];
				List<MadSprite> list = ignorePointer ? EmptyMadSpriteList : AllSpritesForScreenPoint(touch.position);
				for (int i = 0; i < list.Count; i++)
				{
					hashSet.Add(list[i]);
				}
				foreach (MadSprite item in hashSet)
				{
					if (touch.phase == TouchPhase.Began)
					{
						touchDownSprites.Add(item);
						item.onTouchEnter(item);
					}
					else if (touch.phase == TouchPhase.Ended && touchDownSprites.Contains(item))
					{
						item.onTap(item);
						item.TryFocus();
					}
					else if (IsDragging(item))
					{
						item.onTouchExit(item);
						touchDownSprites.Remove(item);
					}
				}
			}
			List<MadSprite> list2 = new List<MadSprite>();
			foreach (MadSprite touchDownSprite in touchDownSprites)
			{
				if (!hashSet.Contains(touchDownSprite))
				{
					list2.Add(touchDownSprite);
					touchDownSprite.onTouchExit(touchDownSprite);
				}
			}
			foreach (MadSprite item2 in list2)
			{
				hoverSprites.Remove(item2);
			}
		}

		private void UpdateMouseInput()
		{
			if (haveTouch)
			{
				return;
			}
			bool ignorePointer = false;
			CheckIgnorePointer(ref ignorePointer);
			HashSet<MadSprite> hashSet = ignorePointer ? EmptyMadSpriteHashSet : new HashSet<MadSprite>(AllSpritesForScreenPoint(UnityEngine.Input.mousePosition));
			foreach (MadSprite item in hashSet)
			{
				if (hoverSprites.Add(item))
				{
					item.onMouseEnter(item);
				}
				if (IsDragging(item))
				{
					touchDownSprites.Remove(item);
				}
			}
			if (hashSet.Count != hoverSprites.Count)
			{
				List<MadSprite> list = new List<MadSprite>();
				foreach (MadSprite hoverSprite in hoverSprites)
				{
					if (!hashSet.Contains(hoverSprite))
					{
						list.Add(hoverSprite);
						hoverSprite.onMouseExit(hoverSprite);
					}
				}
				foreach (MadSprite item2 in list)
				{
					hoverSprites.Remove(item2);
				}
			}
			if (!ignorePointer && Input.GetMouseButtonDown(0))
			{
				foreach (MadSprite hoverSprite2 in hoverSprites)
				{
					hoverSprite2.onMouseDown(hoverSprite2);
					touchDownSprites.Add(hoverSprite2);
				}
			}
			if (!ignorePointer && Input.GetMouseButtonUp(0))
			{
				int focusedSpriteModCount = _focusedSpriteModCount;
				foreach (MadSprite touchDownSprite in touchDownSprites)
				{
					if (hashSet.Contains(touchDownSprite))
					{
						touchDownSprite.onMouseUp(touchDownSprite);
						touchDownSprite.TryFocus();
					}
				}
				touchDownSprites.Clear();
				if (focusedSpriteModCount == _focusedSpriteModCount && focusedSprite != null)
				{
					focusedSprite.hasFocus = false;
				}
			}
		}

		private void CheckIgnorePointer(ref bool ignorePointer)
		{
			if (!(EventSystem.current != null))
			{
				return;
			}
			if (uGUIGetLastPointerMethod == null)
			{
				TryInitializeUnityUI();
			}
			if (uGUIGetLastPointerMethod == null)
			{
				return;
			}
			PointerEventData pointerEventData = (PointerEventData)uGUIGetLastPointerMethod.Invoke(uGUIStandaloneInputModule, uGUIGetLastPointerMethodArgs);
			if (pointerEventData == null)
			{
				return;
			}
			EventSystem.current.RaycastAll(pointerEventData, uGUIRaycastResult);
			for (int i = 0; i < uGUIRaycastResult.Count; i++)
			{
				GameObject gameObject = uGUIRaycastResult[i].gameObject;
				if (unityUiIgnore.Count == 0)
				{
					ignorePointer = true;
				}
				for (int j = 0; j < unityUiIgnore.Count; j++)
				{
					GameObject gameObject2 = unityUiIgnore[j];
					if (gameObject.transform != gameObject2.transform && !gameObject.transform.IsChildOf(gameObject2.transform))
					{
						ignorePointer = true;
						break;
					}
				}
			}
		}

		private bool IsDragging(MadSprite sprite)
		{
			MadDraggable madDraggable = sprite.FindParent<MadDraggable>();
			if (madDraggable != null && madDraggable.dragging)
			{
				return true;
			}
			return false;
		}

		private List<MadSprite> AllSpritesForScreenPoint(Vector2 point)
		{
			List<MadSprite> list = new List<MadSprite>();
			Ray ray = currentCamera.ScreenPointToRay(point);
			RaycastHit[] array = Physics.RaycastAll(ray, 4000f);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				Collider collider = raycastHit.collider;
				MadSprite component = collider.GetComponent<MadSprite>();
				if (component != null && component.panel == this)
				{
					list.Add(component);
				}
			}
			return list;
		}

		public static MadPanel FirstOrNull(Transform currentTransform)
		{
			if (currentTransform != null)
			{
				MadPanel madPanel = MadTransform.FindParent<MadPanel>(currentTransform);
				if (madPanel != null)
				{
					return madPanel;
				}
			}
			if (panels.Count > 0)
			{
				return panels[0];
			}
			return UnityEngine.Object.FindObjectOfType(typeof(MadPanel)) as MadPanel;
		}

		public static MadPanel UniqueOrNull()
		{
			if (panels.Count == 1)
			{
				return panels[0];
			}
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(MadPanel));
			if (array.Length == 1)
			{
				return array[0] as MadPanel;
			}
			return null;
		}

		public static MadPanel[] All()
		{
			return panels.ToArray();
		}
	}
}
