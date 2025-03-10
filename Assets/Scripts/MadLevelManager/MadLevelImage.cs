using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[RequireComponent(typeof(MadSprite))]
	public class MadLevelImage : MonoBehaviour
	{
		[Serializable]
		public class LevelTexture
		{
			public Texture2D image;
		}

		public List<LevelTexture> levelTextures = new List<LevelTexture>();

		private MadLevelIcon icon;

		private MadSprite sprite;

		private void Start()
		{
			icon = MadTransform.FindParent<MadLevelIcon>(base.transform);
			sprite = GetComponent<MadSprite>();
			if (icon != null)
			{
				AssignTexture();
			}
			else
			{
				UnityEngine.Debug.LogError("MadLevelImage may be set only as a MadLevelIcon child");
			}
		}

		private void AssignTexture()
		{
			int levelIndex = icon.levelIndex;
			if (levelIndex < levelTextures.Count)
			{
				LevelTexture levelTexture = levelTextures[levelIndex];
				if (levelTexture.image != null)
				{
					sprite.texture = levelTexture.image;
				}
				else
				{
					UnityEngine.Debug.LogWarning("Image for level " + (levelIndex + 1) + " not assinged");
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("Image for level " + (levelIndex + 1) + " not assinged");
			}
		}
	}
}
