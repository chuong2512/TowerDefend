using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelGridBullets : MonoBehaviour
	{
		public MadDragStopDraggable draggable;

		public Texture2D bulletTextureOff;

		public Texture2D bulletTextureOn;

		public Vector2 bulletDistance = new Vector2(64f, 0f);

		public int guiDepth;

		public bool hideManagedObjects = true;

		private MadSprite[] bulletSprites;

		private int currentHash;

		private void Start()
		{
			if (CanRebuild())
			{
				Rebuild();
			}
		}

		private void Update()
		{
			if (RebuildNeeded() && CanRebuild())
			{
				try
				{
					MadTransform.registerUndo = false;
					Rebuild();
				}
				finally
				{
					MadTransform.registerUndo = true;
				}
			}
			else
			{
				UpdateBullets();
			}
		}

		private void UpdateBullets()
		{
			if (bulletSprites != null)
			{
				for (int i = 0; i < bulletSprites.Length; i++)
				{
					bulletSprites[i].texture = BulletTexture(i);
				}
			}
		}

		private bool RebuildNeeded()
		{
			int num = ConfigurationHash();
			if (num != currentHash)
			{
				return true;
			}
			return false;
		}

		private int ConfigurationHash()
		{
			MadHashCode madHashCode = new MadHashCode();
			madHashCode.Add(bulletTextureOff);
			madHashCode.Add(bulletTextureOn);
			madHashCode.Add(hideManagedObjects);
			madHashCode.Add(bulletDistance);
			madHashCode.Add(guiDepth);
			if (draggable != null)
			{
				madHashCode.Add(draggable.dragStopCount);
			}
			return madHashCode.GetHashCode();
		}

		private bool CanRebuild()
		{
			return bulletTextureOff != null && bulletTextureOn != null && draggable != null;
		}

		private void Rebuild()
		{
			currentHash = ConfigurationHash();
			RebuildClean();
			RebuildConstruct();
		}

		private void RebuildClean()
		{
			List<MadSprite> list = MadTransform.FindChildren(base.transform, (MadSprite sprite) => sprite.name.StartsWith("generated_"));
			foreach (MadSprite item in list)
			{
				MadGameObject.SafeDestroy(item.gameObject);
			}
		}

		private void RebuildConstruct()
		{
			int dragStopCount = draggable.dragStopCount;
			bulletSprites = new MadSprite[dragStopCount];
			Vector3 a = -(bulletDistance * (dragStopCount - 1)) / 2f;
			for (int i = 0; i < dragStopCount; i++)
			{
				MadSprite madSprite = MadTransform.CreateChild<MadSprite>(base.transform, "generated_bullet_" + (i + 1));
				madSprite.transform.localPosition = a + (Vector3)bulletDistance * (float)i;
				madSprite.texture = BulletTexture(i);
				madSprite.guiDepth = guiDepth;
				bulletSprites[i] = madSprite;
				if (hideManagedObjects)
				{
					madSprite.gameObject.hideFlags = HideFlags.HideInHierarchy;
				}
			}
		}

		private bool IsBulletOn(int index)
		{
			return draggable.dragStopCurrentIndex == index;
		}

		private Texture2D BulletTexture(int index)
		{
			if (IsBulletOn(index))
			{
				return bulletTextureOn;
			}
			return bulletTextureOff;
		}
	}
}
