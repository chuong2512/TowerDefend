using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelGridLayout : MadLevelAbstractLayout
	{
		private class Page
		{
			public readonly Transform transform;

			public MadSprite[] sprites;

			public MadLevelIcon[] icons;

			public int dragStopIndex
			{
				get;
				private set;
			}

			public Page(int dragStopIndex, Transform transform)
			{
				this.transform = transform;
				this.dragStopIndex = dragStopIndex;
				sprites = transform.GetComponentsInChildren<MadSprite>();
				icons = transform.GetComponentsInChildren<MadLevelIcon>();
			}

			public bool ContainsIcon(MadLevelIcon icon)
			{
				return Array.IndexOf(icons, icon) != -1;
			}
		}

		public enum SetupMethod
		{
			Generate,
			Manual
		}

		public enum HorizontalAlign
		{
			Left,
			Center,
			Right
		}

		public enum VerticalAlign
		{
			Top,
			Middle,
			Bottom
		}

		public enum PagingMethod
		{
			HorizontalSimple,
			VerticalSimple,
			HorizontalZoom,
			VerticalZoom
		}

		public SetupMethod setupMethod;

		public MadSprite rightSlideSprite;

		public MadSprite leftSlideSprite;

		public Vector2 iconScale = Vector2.one;

		public Vector2 iconOffset;

		public Vector2 rightSlideScale = Vector2.one;

		public Vector2 rightSlideOffset;

		public Vector2 leftSlideScale = Vector2.one;

		public Vector2 leftSlideOffset;

		public int gridWidth = 3;

		public int gridHeight = 3;

		public bool limitLevelsPerPage;

		public int levelsPerPage;

		public int pixelsWidth = 720;

		public int pixelsHeight = 578;

		public HorizontalAlign horizontalAlign = HorizontalAlign.Center;

		public VerticalAlign verticalAlign;

		public bool pagesOffsetFromResolution = true;

		public float pagesOffsetManual = 1000f;

		public PagingMethod pagingMethod;

		public bool pagingInvert;

		public float pagesOffsetPercent = 2f;

		public float pagesZoomScale = 0.5f;

		public bool ignoreInputForIconsOnDifferentPages;

		[NonSerialized]
		[HideInInspector]
		public bool hidePrevSlideIcon;

		[NonSerialized]
		[HideInInspector]
		public bool hideNextSlideIcon;

		[NonSerialized]
		[HideInInspector]
		public bool dirty;

		[NonSerialized]
		[HideInInspector]
		public bool deepClean;

		private int hash;

		private MadDragStopDraggable draggable;

		private MadSprite slideLeft;

		private MadSprite slideRight;

		private List<Page> pages = new List<Page>();

		private int pageCurrentIndex;

		public bool hideManagedObjects = true;

		[SerializeField]
		private Vector2 _iconScale = Vector2.one;

		private bool canSwitchPagesInThisFrame = true;

		private float pagesXOffset
		{
			get
			{
				if (pagesOffsetFromResolution)
				{
					MadRootNode madRootNode = MadTransform.FindParent<MadRootNode>(base.transform);
					Vector3 position = madRootNode.ScreenGlobal(1f, 1f);
					position = base.transform.InverseTransformPoint(position);
					return position.x * 2f;
				}
				return pagesOffsetManual;
			}
		}

		private float pagesYOffset
		{
			get
			{
				if (pagesOffsetFromResolution)
				{
					MadRootNode madRootNode = MadTransform.FindParent<MadRootNode>(base.transform);
					Vector3 position = madRootNode.ScreenGlobal(1f, 1f);
					position = base.transform.InverseTransformPoint(position);
					return position.y * 2f;
				}
				return pagesOffsetManual;
			}
		}

		private Page currentPage => pages[pageCurrentIndex];

		private bool generate => setupMethod == SetupMethod.Generate;

		public override MadLevelIcon GetIcon(string levelName)
		{
			MadLevelIcon madLevelIcon = MadTransform.FindChild(base.transform, (MadLevelIcon i) => i.level.name == levelName);
			if (madLevelIcon != null)
			{
				return madLevelIcon;
			}
			return null;
		}

		public override MadLevelIcon FindClosestIcon(Vector3 position)
		{
			MadLevelIcon result = null;
			float num = float.MaxValue;
			List<MadLevelIcon> list = MadTransform.FindChildren<MadLevelIcon>(base.transform);
			foreach (MadLevelIcon item in list)
			{
				float num2 = Vector3.Distance(position, item.transform.position);
				if (num2 < num)
				{
					result = item;
					num = num2;
				}
			}
			return result;
		}

		public override void LookAtIcon(MadLevelIcon icon, bool animate = false)
		{
			int newIndex = PageIndexForLevel(icon.level.name);
			SwitchPage(newIndex, !animate);
		}

		public void LookAtIconAnimate(MadLevelIcon icon)
		{
			int newIndex = PageIndexForLevel(icon.level.name);
			SwitchPage(newIndex, now: false);
		}

		public override MadLevelIcon GetCurrentIcon()
		{
			if (gridWidth * gridHeight == 1)
			{
				return pages[pageCurrentIndex].icons[0];
			}
			return base.GetCurrentIcon();
		}

		public override void Activate(MadLevelIcon icon)
		{
			if (currentPage.ContainsIcon(icon))
			{
				base.Activate(icon);
				return;
			}
			MadLevelIcon activeIcon = GetActiveIcon();
			if (activeIcon != null && icon != activeIcon)
			{
				DeactivateActiveIcon();
			}
			Page page = FindPageWithIcon(icon);
			if (page != null)
			{
				int dragStopIndex = page.dragStopIndex;
				if (currentPage.dragStopIndex < dragStopIndex)
				{
					GoToNextPage();
				}
				else
				{
					GoToPrevPage();
				}
			}
		}

		private Page FindPageWithIcon(MadLevelIcon icon)
		{
			for (int i = 0; i < pages.Count; i++)
			{
				Page page = pages[i];
				if (page.ContainsIcon(icon))
				{
					return page;
				}
			}
			return null;
		}

		private void OnValidate()
		{
			gridWidth = Mathf.Max(1, gridWidth);
			gridHeight = Mathf.Max(1, gridHeight);
			pixelsWidth = Mathf.Max(1, pixelsWidth);
			pixelsHeight = Mathf.Max(1, pixelsHeight);
			if (!limitLevelsPerPage)
			{
				levelsPerPage = gridWidth * gridHeight;
			}
			else
			{
				levelsPerPage = Mathf.Clamp(levelsPerPage, 1, gridWidth * gridHeight);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Upgrade();
		}

		private void Upgrade()
		{
			if (_iconScale != Vector2.one)
			{
				iconScale = _iconScale;
				_iconScale = Vector2.one;
			}
		}

		protected override void Start()
		{
			UpdateLayout(forceDelete: false);
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
			try
			{
				MadTransform.registerUndo = false;
				UpdateLayout(forceDelete: false);
			}
			finally
			{
				MadTransform.registerUndo = true;
			}
			SlideIconsUpdate();
			UpdatePagingZoom();
			if (!Application.isPlaying && !limitLevelsPerPage)
			{
				levelsPerPage = gridWidth * gridHeight;
			}
			if (Application.isPlaying)
			{
				UpdateIgnoreInput();
			}
			canSwitchPagesInThisFrame = true;
		}

		private void UpdateIgnoreInput()
		{
			if (!ignoreInputForIconsOnDifferentPages)
			{
				return;
			}
			for (int i = 0; i < pages.Count; i++)
			{
				Page page = pages[i];
				for (int j = 0; j < page.icons.Length; j++)
				{
					MadLevelIcon madLevelIcon = page.icons[j];
					madLevelIcon.GetComponent<Collider>().enabled = (page == currentPage);
				}
			}
		}

		private void UpdatePagingZoom()
		{
			if (pagingMethod == PagingMethod.HorizontalZoom || pagingMethod == PagingMethod.VerticalZoom)
			{
				float progress = draggable.GetProgress();
				for (int i = 0; i < pages.Count; i++)
				{
					Page page = pages[i];
					float num = Mathf.Abs(progress - (float)i);
					float num2 = pagesZoomScale * num;
					page.transform.localScale = new Vector3(Mathf.Clamp01(1f - num2), Mathf.Clamp01(1f - num2), Mathf.Clamp01(1f - num2));
					Transform transform = page.transform;
					Vector3 localPosition = page.transform.localPosition;
					float x = localPosition.x;
					Vector3 localPosition2 = page.transform.localPosition;
					transform.localPosition = new Vector3(x, localPosition2.y, 300f * num);
				}
			}
		}

		private void UpdateLayout(bool forceDelete)
		{
			if (IsDirty())
			{
				bool forceDelete2 = deepClean || (!Application.isPlaying && (forceDelete || generate));
				CleanUp(forceDelete2);
				Build(forceDelete2);
				MakeClean();
				base.currentConfiguration.callbackChanged = delegate
				{
					if (this != null)
					{
						CleanUp(generate);
						Build(generate);
						MakeClean();
					}
				};
			}
		}

		private void SlideIconsUpdate()
		{
			if (!(slideLeft == null) && !(slideRight == null))
			{
				SlideSetActive(slideLeft, ShoudShowPrevSlideIcon());
				SlideSetActive(slideRight, ShouldShowNextSlideIcon());
				if (draggable.dragging)
				{
					SlideIconsHide();
				}
			}
		}

		private bool ShouldShowNextSlideIcon()
		{
			return HasNextPage() && !hideNextSlideIcon;
		}

		private bool ShoudShowPrevSlideIcon()
		{
			return HasPrevPage() && !hidePrevSlideIcon;
		}

		private void SlideIconsHide()
		{
			SlideSetActive(slideLeft, act: false);
			SlideSetActive(slideRight, act: false);
		}

		private static void SlideSetActive(MadSprite icon, bool act)
		{
			if (!MadGameObject.IsActive(icon.gameObject))
			{
				MadGameObject.SetActive(icon.gameObject, active: true);
			}
			bool visible = icon.visible;
			if (visible != act)
			{
				icon.visible = act;
			}
		}

		private bool HasNextPage()
		{
			return pageCurrentIndex + 1 < pages.Count;
		}

		private bool HasPrevPage()
		{
			return pageCurrentIndex > 0;
		}

		private void GoToNextPage()
		{
			if (canSwitchPagesInThisFrame)
			{
				SwitchPage(pageCurrentIndex + 1, now: false);
				canSwitchPagesInThisFrame = false;
			}
		}

		private void GoToPrevPage()
		{
			if (canSwitchPagesInThisFrame)
			{
				SwitchPage(pageCurrentIndex - 1, now: false);
				canSwitchPagesInThisFrame = false;
			}
		}

		private void SwitchPage(int newIndex, bool now)
		{
			MadDebug.Assert(newIndex >= 0 && newIndex < pages.Count, "There's no page with index " + newIndex);
			pageCurrentIndex = newIndex;
			draggable.MoveTo(newIndex, now);
		}

		private int PageIndexForLevel(string levelName)
		{
			int num = base.currentConfiguration.FindLevelIndex(MadLevel.Type.Level, configurationGroup, levelName);
			int num2 = gridWidth * gridHeight;
			return num / num2;
		}

		public int GetCurrentPageNumber()
		{
			return pageCurrentIndex + 1;
		}

		private bool IsDirty()
		{
			int num = ComputeHash();
			if (dirty)
			{
				hash = num;
				return true;
			}
			if (base.currentConfiguration == null || iconTemplate == null)
			{
				hash = num;
				return false;
			}
			if (hash != num)
			{
				hash = num;
				return true;
			}
			return false;
		}

		private int ComputeHash()
		{
			int currentHash = 37;
			currentHash = MadHashCode.Add(currentHash, base.currentConfiguration);
			currentHash = MadHashCode.Add(currentHash, configurationGroup);
			currentHash = MadHashCode.Add(currentHash, hideManagedObjects);
			currentHash = MadHashCode.Add(currentHash, limitLevelsPerPage);
			currentHash = MadHashCode.Add(currentHash, levelsPerPage);
			currentHash = MadHashCode.Add(currentHash, (int)setupMethod);
			currentHash = MadHashCode.Add(currentHash, iconTemplate);
			currentHash = MadHashCode.AddList(currentHash, iconTemplates);
			currentHash = MadHashCode.Add(currentHash, (int)iconTemplateQuantity);
			currentHash = MadHashCode.Add(currentHash, iconScale);
			currentHash = MadHashCode.Add(currentHash, iconOffset);
			currentHash = MadHashCode.Add(currentHash, leftSlideSprite);
			currentHash = MadHashCode.Add(currentHash, leftSlideScale);
			currentHash = MadHashCode.Add(currentHash, leftSlideOffset);
			currentHash = MadHashCode.Add(currentHash, rightSlideSprite);
			currentHash = MadHashCode.Add(currentHash, rightSlideScale);
			currentHash = MadHashCode.Add(currentHash, rightSlideOffset);
			currentHash = MadHashCode.Add(currentHash, gridWidth);
			currentHash = MadHashCode.Add(currentHash, gridHeight);
			currentHash = MadHashCode.Add(currentHash, (int)horizontalAlign);
			currentHash = MadHashCode.Add(currentHash, (int)verticalAlign);
			currentHash = MadHashCode.Add(currentHash, pixelsWidth);
			currentHash = MadHashCode.Add(currentHash, pixelsHeight);
			currentHash = MadHashCode.Add(currentHash, pagesOffsetManual);
			currentHash = MadHashCode.Add(currentHash, pagesOffsetFromResolution);
			currentHash = MadHashCode.Add(currentHash, (int)enumerationType);
			currentHash = MadHashCode.Add(currentHash, enumerationOffset);
			currentHash = MadHashCode.Add(currentHash, (int)pagingMethod);
			currentHash = MadHashCode.Add(currentHash, pagingInvert);
			return MadHashCode.Add(currentHash, pagesOffsetPercent);
		}

		private void MakeClean()
		{
			dirty = false;
			deepClean = false;
		}

		private void CleanUp(bool forceDelete)
		{
			int num = base.currentConfiguration.LevelCount(MadLevel.Type.Level, configurationGroup);
			List<MadLevelIcon> list = MadTransform.FindChildren(base.transform, (MadLevelIcon icon) => icon.hasLevelConfiguration);
			if (forceDelete)
			{
				foreach (MadLevelIcon item in list)
				{
					UnityEngine.Object.DestroyImmediate(item.gameObject);
				}
				List<Transform> list2 = MadTransform.FindChildren(base.transform, (Transform t) => t.name.StartsWith("Page "));
				foreach (Transform item2 in list2)
				{
					UnityEngine.Object.DestroyImmediate(item2.gameObject);
				}
			}
			else
			{
				IOrderedEnumerable<MadLevelIcon> orderedEnumerable = from c in list
					orderby c.levelIndex
					select c;
				foreach (MadLevelIcon item3 in orderedEnumerable)
				{
					if (item3.levelIndex >= num)
					{
						MadGameObject.SetActive(item3.gameObject, active: false);
					}
				}
			}
			ClearSlide("SlideLeftAnchor");
			ClearSlide("SlideRightAnchor");
		}

		private void Build(bool forceDelete)
		{
			UpdateMultipleIcons();
			draggable = MadTransform.GetOrCreateChild<MadDragStopDraggable>(base.transform, "Draggable");
			draggable.dragStopCallback = OnDragStopCallback;
			float num = -pixelsWidth / 2;
			float num2 = pixelsHeight / 2;
			float num3 = pixelsWidth / (gridWidth + 1);
			float num4 = -pixelsHeight / (gridHeight + 1);
			MadLevelIcon madLevelIcon = null;
			int num5 = base.currentConfiguration.LevelCount(MadLevel.Type.Level, configurationGroup);
			int levelIndex = 0;
			int pageIndex = 0;
			while (levelIndex < num5)
			{
				Transform transform = MadTransform.FindChild(draggable.transform, (Transform t) => t.name == "Page " + (pageIndex + 1));
				bool flag = transform == null;
				if (flag)
				{
					transform = MadTransform.CreateChild<Transform>(draggable.transform, "Page " + (pageIndex + 1));
					transform.hideFlags = ((generate && hideManagedObjects) ? HideFlags.HideInHierarchy : HideFlags.None);
				}
				Transform transform2 = MadTransform.FindChild(draggable.transform, (Transform t) => t.name == "Anchor " + (pageIndex + 1));
				if (transform2 == null)
				{
					transform2 = MadTransform.CreateChild<Transform>(draggable.transform, "Anchor " + (pageIndex + 1));
					transform2.gameObject.AddComponent<MadFollow>();
				}
				MadFollow component = transform2.GetComponent<MadFollow>();
				component.followTransform = transform;
				if (flag || generate)
				{
					switch (pagingMethod)
					{
					case PagingMethod.HorizontalSimple:
						transform.localPosition = ComputePageOffsetHoriz(pageIndex);
						break;
					case PagingMethod.VerticalSimple:
						transform.localPosition = ComputePageOffsetVert(pageIndex);
						break;
					case PagingMethod.HorizontalZoom:
						transform.localPosition = ComputePageOffsetHoriz(pageIndex);
						break;
					case PagingMethod.VerticalZoom:
						transform.localPosition = ComputePageOffsetVert(pageIndex);
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				int num6 = 0;
				bool flag2 = false;
				for (int i = 1; i <= gridHeight; i++)
				{
					if (levelIndex >= num5)
					{
						break;
					}
					if (flag2)
					{
						break;
					}
					int num7 = 1;
					while (num7 <= gridWidth && levelIndex < num5)
					{
						if (limitLevelsPerPage && generate && num6 == levelsPerPage)
						{
							flag2 = true;
							break;
						}
						MadLevelIcon madLevelIcon2 = null;
						if (!forceDelete)
						{
							madLevelIcon2 = MadTransform.FindChild(draggable.transform, (MadLevelIcon ic) => ic.levelIndex == levelIndex, 0);
							if (madLevelIcon2 != null)
							{
								madLevelIcon2.transform.parent = transform;
							}
							else
							{
								madLevelIcon2 = MadTransform.FindChild(transform.transform, (MadLevelIcon ic) => ic.levelIndex == levelIndex, 0);
							}
						}
						MadLevelConfiguration.Level level = base.currentConfiguration.GetLevel(MadLevel.Type.Level, configurationGroup, levelIndex);
						bool flag3 = madLevelIcon2 == null;
						if (flag3)
						{
							MadLevelIcon madLevelIcon3 = (iconTemplateQuantity != 0) ? iconTemplates[levelIndex] : iconTemplate;
							if (madLevelIcon3 == null)
							{
								madLevelIcon3 = iconTemplate;
							}
							madLevelIcon2 = CreateIcon(transform.transform, level.name, madLevelIcon3);
						}
						else
						{
							madLevelIcon2.name = level.name;
						}
						madLevelIcon2.gameObject.hideFlags = ((generate && hideManagedObjects) ? HideFlags.HideInHierarchy : HideFlags.None);
						madLevelIcon2.levelGroup = configurationGroup;
						madLevelIcon2.levelIndex = levelIndex;
						madLevelIcon2.configuration = base.currentConfiguration;
						madLevelIcon2.hasLevelConfiguration = true;
						if (!MadGameObject.IsActive(madLevelIcon2.gameObject))
						{
							MadGameObject.SetActive(madLevelIcon2.gameObject, active: true);
						}
						else
						{
							MadGameObject.SetActive(madLevelIcon2.gameObject, active: false);
							MadGameObject.SetActive(madLevelIcon2.gameObject, active: true);
						}
						if (generate || flag3)
						{
							madLevelIcon2.pivotPoint = MadSprite.PivotPoint.Center;
							if (!generate)
							{
								madLevelIcon2.transform.localPosition = new Vector3(num + num3 * (float)num7 + iconOffset.x, num2 + num4 * (float)i + iconOffset.y, 0f);
							}
							else
							{
								madLevelIcon2.transform.localPosition = IconGeneratedPosition(levelIndex, num5, num7 - 1, i - 1);
							}
							madLevelIcon2.transform.localScale = new Vector3(iconScale.x, iconScale.y, 1f);
							if (madLevelIcon2.levelNumber != null)
							{
								madLevelIcon2.levelNumber.text = GetEnumerationValue(levelIndex);
							}
						}
						if (madLevelIcon != null && flag3)
						{
							madLevelIcon.unlockOnComplete.Add(madLevelIcon2);
						}
						if (!Application.isPlaying || !MadLevelProfile.IsLockedSet(level.name))
						{
							madLevelIcon2.locked = madLevelIcon2.level.lockedByDefault;
						}
						madLevelIcon = madLevelIcon2;
						num7++;
						levelIndex++;
						num6++;
					}
				}
				pageIndex++;
			}
			BuildSlideIcons();
			BuildDragging();
			MadSprite[] componentsInChildren = GetComponentsInChildren<MadSprite>();
			MadSprite[] array = componentsInChildren;
			foreach (MadSprite madSprite in array)
			{
				madSprite.editorSelectable = !generate;
			}
		}

		private Vector3 ComputePageOffsetHoriz(int pageIndex)
		{
			if (!pagesOffsetFromResolution)
			{
				return new Vector3(pagesXOffset * (float)pageIndex * (float)((!pagingInvert) ? 1 : (-1)), 0f, 0f);
			}
			return new Vector3(pagesXOffset * (pagesOffsetPercent / 2f) * (float)pageIndex * (float)((!pagingInvert) ? 1 : (-1)), 0f, 0f);
		}

		private Vector3 ComputePageOffsetVert(int pageIndex)
		{
			if (!pagesOffsetFromResolution)
			{
				return new Vector3(0f, (0f - pagesYOffset) * (float)pageIndex * (float)((!pagingInvert) ? 1 : (-1)), 0f);
			}
			return new Vector3(0f, (0f - pagesYOffset) * (pagesOffsetPercent / 2f) * (float)pageIndex * (float)((!pagingInvert) ? 1 : (-1)), 0f);
		}

		private void OnDragStopCallback(int index)
		{
			if (Application.isPlaying)
			{
				pageCurrentIndex = index;
			}
		}

		private Vector3 IconGeneratedPosition(int levelIndex, int levelCount, int xIndex, int yIndex)
		{
			float num = -pixelsWidth / 2;
			float num2 = pixelsWidth / 2;
			float num3 = pixelsHeight / 2;
			float num4 = -pixelsHeight / 2;
			float num5 = pixelsWidth / (gridWidth + 1);
			float num6 = -pixelsHeight / (gridHeight + 1);
			int num7 = gridWidth * gridHeight;
			int num8 = levelIndex / num7;
			int num9 = Mathf.Min(levelCount - num8 * num7, num7);
			if (limitLevelsPerPage && generate)
			{
				num9 = levelsPerPage;
			}
			int num10 = (int)Mathf.Ceil((float)num9 / (float)gridWidth);
			int num11 = (yIndex >= num9 / gridWidth) ? (num9 % gridWidth) : gridWidth;
			float num12 = num5 * (float)xIndex + iconOffset.x;
			float num13 = num6 * (float)yIndex + iconOffset.y;
			float num14 = num5 * (float)(num11 - 1);
			float num15 = num6 * (float)(num10 - 1);
			switch (horizontalAlign)
			{
			case HorizontalAlign.Left:
				num12 = num + num5 + num12;
				break;
			case HorizontalAlign.Center:
				num12 -= num14 / 2f;
				break;
			case HorizontalAlign.Right:
				num12 = num2 - num5 - (num14 - num12);
				break;
			}
			switch (verticalAlign)
			{
			case VerticalAlign.Top:
				num13 = num3 + num6 + num13;
				break;
			case VerticalAlign.Middle:
				num13 -= num15 / 2f;
				break;
			case VerticalAlign.Bottom:
				num13 = num4 - num6 - (num15 - num13);
				break;
			}
			return new Vector3(num12, num13, 0f);
		}

		private void BuildDragging()
		{
			List<Transform> list = MadTransform.FindChildren(draggable.transform, (Transform t) => t.name.StartsWith("Page"), 0);
			list.Sort(delegate(Transform a, Transform b)
			{
				int num = int.Parse(a.name.Split(' ')[1]);
				int value = int.Parse(b.name.Split(' ')[1]);
				return num.CompareTo(value);
			});
			draggable.ClearDragStops();
			pages.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				MadDragStopDraggable madDragStopDraggable = draggable;
				Vector3 localPosition = list[i].localPosition;
				float x = localPosition.x;
				Vector3 localPosition2 = list[i].localPosition;
				int dragStopIndex = madDragStopDraggable.AddDragStop(x, localPosition2.y);
				Page item = new Page(dragStopIndex, list[i].transform);
				pages.Add(item);
			}
			switch (pagingMethod)
			{
			case PagingMethod.HorizontalSimple:
				draggable.direction = MadDragStopDraggable.Direction.Horizontal;
				break;
			case PagingMethod.VerticalSimple:
				draggable.direction = MadDragStopDraggable.Direction.Vertical;
				break;
			case PagingMethod.HorizontalZoom:
				draggable.direction = MadDragStopDraggable.Direction.Horizontal;
				break;
			case PagingMethod.VerticalZoom:
				draggable.direction = MadDragStopDraggable.Direction.Vertical;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			draggable.directionInvert = pagingInvert;
		}

		private void BuildSlideIcons()
		{
			if (!(leftSlideSprite == null) && !(rightSlideSprite == null))
			{
				slideLeft = BuildSlide(leftSlideSprite, "SlideLeftAnchor", left: true);
				slideRight = BuildSlide(rightSlideSprite, "SlideRightAnchor", left: false);
				slideLeft.transform.localScale = new Vector3(leftSlideScale.x, leftSlideScale.y, 1f);
				slideRight.transform.localScale = new Vector3(rightSlideScale.x, rightSlideScale.y, 1f);
				slideLeft.transform.localPosition += (Vector3)leftSlideOffset;
				slideRight.transform.localPosition += (Vector3)rightSlideOffset;
				MadSprite.Action b = delegate
				{
					if (HasPrevPage())
					{
						GoToPrevPage();
					}
				};
				MadSprite madSprite = slideLeft;
				madSprite.onTap = (MadSprite.Action)Delegate.Combine(madSprite.onTap, b);
				MadSprite madSprite2 = slideLeft;
				madSprite2.onMouseUp = (MadSprite.Action)Delegate.Combine(madSprite2.onMouseUp, b);
				MadSprite.Action b2 = delegate
				{
					if (HasNextPage())
					{
						GoToNextPage();
					}
				};
				MadSprite madSprite3 = slideRight;
				madSprite3.onTap = (MadSprite.Action)Delegate.Combine(madSprite3.onTap, b2);
				MadSprite madSprite4 = slideRight;
				madSprite4.onMouseUp = (MadSprite.Action)Delegate.Combine(madSprite4.onMouseUp, b2);
			}
		}

		private void ClearSlide(string anchorName)
		{
			MadAnchor madAnchor = MadTransform.FindChildWithName<MadAnchor>(base.transform, anchorName);
			if (madAnchor != null)
			{
				UnityEngine.Object.DestroyImmediate(madAnchor.gameObject);
			}
		}

		private MadSprite BuildSlide(MadSprite template, string anchorName, bool left)
		{
			MadAnchor madAnchor = MadTransform.CreateChild<MadAnchor>(base.transform, anchorName);
			if (hideManagedObjects)
			{
				madAnchor.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			madAnchor.position = ((!left) ? MadAnchor.Position.Right : MadAnchor.Position.Left);
			madAnchor.Update();
			GameObject gameObject = MadTransform.CreateChild(madAnchor.transform, "Offset");
			gameObject.transform.localPosition = new Vector3((!left) ? (-template.texture.width / 2) : (template.texture.width / 2), 0f, 0f);
			MadSprite madSprite = MadTransform.CreateChild(gameObject.transform, "slide", template);
			madSprite.transform.localScale = Vector3.one;
			madSprite.transform.localPosition = Vector3.zero;
			madSprite.guiDepth = 50;
			return madSprite;
		}
	}
}
