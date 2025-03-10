using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelFreeLayout : MadLevelAbstractLayout
	{
		private Vector2 offset = new Vector2(128f, 0f);

		public Texture2D backgroundTexture;

		private MadFreeDraggable _draggable;

		[HideInInspector]
		public bool dirty;

		private int lastHash;

		public MadFreeDraggable draggable
		{
			get
			{
				if (_draggable == null)
				{
					_draggable = MadTransform.GetOrCreateChild<MadFreeDraggable>(base.transform, "Draggable");
				}
				return _draggable;
			}
		}

		public override MadLevelIcon GetIcon(string levelName)
		{
			MadDebug.Assert(!string.IsNullOrEmpty(levelName), "null or empty level name");
			return MadTransform.FindChild(draggable.transform, (MadLevelIcon icon) => MadGameObject.IsActive(icon.gameObject) && icon.level.name == levelName, 0);
		}

		public override MadLevelIcon FindClosestIcon(Vector3 position)
		{
			List<MadLevelIcon> list = MadTransform.FindChildren(draggable.transform, (MadLevelIcon ic) => true, 0);
			float num = float.PositiveInfinity;
			MadLevelIcon result = null;
			foreach (MadLevelIcon item in list)
			{
				float num2 = Vector3.Distance(item.transform.position, position);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
			return result;
		}

		public override void LookAtIcon(MadLevelIcon icon, bool animate = false)
		{
			LookAtIcon(icon, MadiTween.EaseType.easeInQuad, animate ? 1 : 0);
		}

		public void LookAtIcon(MadLevelIcon icon, MadiTween.EaseType easeType, float time)
		{
			draggable.MoveToLocal(icon.transform.localPosition, easeType, time);
		}

		public void LookAtLevel(string levelName, MadiTween.EaseType easeType, float time)
		{
			MadLevelIcon icon = GetIcon(levelName);
			if (icon != null)
			{
				LookAtIcon(icon, easeType, time);
			}
			else
			{
				UnityEngine.Debug.LogError("No icon found for level '" + levelName + "'");
			}
		}

		public void ReplaceIcons(GameObject newIcon)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Debug.LogError("This method can be called only from the editor");
				return;
			}
			MadUndo.LegacyRegisterSceneUndo("Replaced Icons");
			List<MadLevelIcon> source = MadTransform.FindChildren<MadLevelIcon>(draggable.transform);
			IEnumerable<MadLevelIcon> enumerable = from i in source
				where MadGameObject.IsActive(i.gameObject)
				select i;
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			List<MadLevelIcon> list = new List<MadLevelIcon>();
			foreach (MadLevelIcon item2 in enumerable)
			{
				Vector3 position = item2.transform.position;
				Quaternion rotation = item2.transform.rotation;
				Vector3 localScale = item2.transform.localScale;
				string name = item2.name;
				int guiDepth = item2.guiDepth;
				int levelIndex = item2.levelIndex;
				MadLevelConfiguration configuration = item2.configuration;
				List<int> list2 = new List<int>();
				foreach (MadLevelIcon item3 in item2.unlockOnComplete)
				{
					list2.Add(item3.levelIndex);
				}
				dictionary[item2.levelIndex] = list2;
				MadUndo.DestroyObjectImmediate(item2.gameObject);
				MadLevelIcon madLevelIcon = (iconTemplateQuantity != 0) ? iconTemplates[levelIndex] : iconTemplate;
				if (madLevelIcon == null)
				{
					madLevelIcon = iconTemplate;
				}
				MadLevelIcon madLevelIcon2 = CreateIcon(draggable.transform, name, madLevelIcon);
				madLevelIcon2.transform.position = position;
				madLevelIcon2.transform.rotation = rotation;
				madLevelIcon2.transform.localScale = localScale;
				madLevelIcon2.guiDepth = guiDepth;
				madLevelIcon2.levelIndex = levelIndex;
				madLevelIcon2.configuration = configuration;
				madLevelIcon2.hasLevelConfiguration = true;
				list.Add(madLevelIcon2);
				List<MadSprite> list3 = MadTransform.FindChildren<MadSprite>(madLevelIcon2.transform);
				foreach (MadSprite item4 in list3)
				{
					item4.guiDepth += guiDepth;
				}
				MadUndo.RegisterCreatedObjectUndo(madLevelIcon2.gameObject, "Replaced Icons");
			}
			source = MadTransform.FindChildren<MadLevelIcon>(draggable.transform);
			foreach (MadLevelIcon item5 in list)
			{
				List<int> list4 = dictionary[item5.levelIndex];
				foreach (int unlockLevelIndex in list4)
				{
					IEnumerable<MadLevelIcon> source2 = from i in source
						where i.levelIndex == unlockLevelIndex
						select i;
					MadLevelIcon item = source2.First();
					item5.unlockOnComplete.Add(item);
				}
			}
			Build();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			base.currentConfiguration.callbackChanged = delegate
			{
				if (this != null)
				{
					Build();
				}
			};
			if (IsDirty())
			{
				Build();
			}
		}

		protected override void Update()
		{
			base.Update();
			if (IsDirty())
			{
				Build();
			}
		}

		private bool IsDirty()
		{
			if (dirty)
			{
				dirty = false;
				return true;
			}
			if (base.currentConfiguration == null || iconTemplate == null)
			{
				return false;
			}
			int currentHash = 37;
			currentHash = MadHashCode.Add(currentHash, base.currentConfiguration);
			currentHash = MadHashCode.Add(currentHash, configurationGroup);
			currentHash = MadHashCode.Add(currentHash, iconTemplate);
			currentHash = MadHashCode.AddList(currentHash, iconTemplates);
			currentHash = MadHashCode.Add(currentHash, (int)iconTemplateQuantity);
			currentHash = MadHashCode.Add(currentHash, backgroundTexture);
			currentHash = MadHashCode.Add(currentHash, (int)enumerationType);
			currentHash = MadHashCode.Add(currentHash, enumerationOffset);
			if (currentHash != lastHash)
			{
				lastHash = currentHash;
				return true;
			}
			return false;
		}

		private void Build()
		{
			List<MadLevelIcon> list = new List<MadLevelIcon>();
			list.AddRange(MadTransform.FindChildren<MadLevelIcon>(base.transform));
			int num = base.currentConfiguration.LevelCount(MadLevel.Type.Level, configurationGroup);
			Vector2 vector = Vector2.zero;
			MadLevelIcon madLevelIcon = null;
			iconTemplate.MinMaxDepthRecursively(out int _, out int _);
			HashSet<MadLevelIcon> hashSet = new HashSet<MadLevelIcon>();
			int levelIndex;
			for (levelIndex = 0; levelIndex < num; levelIndex++)
			{
				MadLevelIcon madLevelIcon2 = MadTransform.FindChild(draggable.transform, (MadLevelIcon ic) => ic.levelIndex == levelIndex, 0);
				bool flag = madLevelIcon2 == null;
				if (flag)
				{
					MadLevelIcon madLevelIcon3 = (iconTemplateQuantity != 0) ? iconTemplates[levelIndex] : iconTemplate;
					if (madLevelIcon3 == null)
					{
						madLevelIcon3 = iconTemplate;
					}
					madLevelIcon2 = CreateIcon(draggable.transform, $"level {levelIndex + 1:D3}", madLevelIcon3);
					madLevelIcon2.pivotPoint = MadSprite.PivotPoint.Center;
					madLevelIcon2.transform.localScale = Vector3.one;
					do
					{
						madLevelIcon2.transform.localPosition = vector;
						vector += offset;
					}
					while (Collides(madLevelIcon2, list));
					list.Add(madLevelIcon2);
				}
				if (!MadGameObject.IsActive(madLevelIcon2.gameObject))
				{
					MadGameObject.SetActive(madLevelIcon2.gameObject, active: true);
				}
				madLevelIcon2.levelGroup = configurationGroup;
				madLevelIcon2.levelIndex = levelIndex;
				madLevelIcon2.configuration = base.currentConfiguration;
				madLevelIcon2.hasLevelConfiguration = true;
				if (madLevelIcon2.levelNumber != null)
				{
					madLevelIcon2.levelNumber.text = GetEnumerationValue(levelIndex);
				}
				if (madLevelIcon != null)
				{
					if (flag)
					{
						madLevelIcon.unlockOnComplete.Add(madLevelIcon2);
					}
				}
				else
				{
					madLevelIcon2.locked = false;
				}
				if (!Application.isPlaying || !MadLevelProfile.IsLockedSet(madLevelIcon2.level.name))
				{
					madLevelIcon2.locked = madLevelIcon2.level.lockedByDefault;
				}
				madLevelIcon = madLevelIcon2;
				hashSet.Add(madLevelIcon2);
			}
			BuildBackgroundTexture();
			DeactivateAllOther(hashSet);
		}

		private bool Collides(MadLevelIcon icon, List<MadLevelIcon> iconList)
		{
			Rect transformedBounds = icon.GetTransformedBounds();
			foreach (MadLevelIcon icon2 in iconList)
			{
				Rect transformedBounds2 = icon2.GetTransformedBounds();
				if (MadMath.Overlaps(transformedBounds, transformedBounds2))
				{
					return true;
				}
			}
			return false;
		}

		private void BuildBackgroundTexture()
		{
			if (backgroundTexture != null)
			{
				MadSprite orCreateChild = MadTransform.GetOrCreateChild<MadSprite>(draggable.transform, "background");
				orCreateChild.texture = backgroundTexture;
				orCreateChild.guiDepth = -1;
				return;
			}
			MadSprite madSprite = MadTransform.FindChildWithName<MadSprite>(draggable.transform, "background");
			if (madSprite != null)
			{
				UnityEngine.Object.DestroyImmediate(madSprite.gameObject);
			}
		}

		private void DeactivateAllOther(HashSet<MadLevelIcon> activeIcons)
		{
			List<MadLevelIcon> list = MadTransform.FindChildren<MadLevelIcon>(draggable.transform);
			foreach (MadLevelIcon item in list)
			{
				if (!activeIcons.Contains(item))
				{
					MadGameObject.SetActive(item.gameObject, active: false);
				}
			}
		}
	}
}
