using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadFont : MonoBehaviour
	{
		public class Glyph
		{
			public float x;

			public float y;

			public float width;

			public float height;

			public int widthPx;

			public int heightPx;

			public float xAdvance;

			public float xOffset;

			public float yOffset;

			public float uMin => x;

			public float uMax => x + width;

			public float vMin => 1f - (y + height);

			public float vMax => 1f - y;

			public string ToString()
			{
				return $"[glyph x={x}, y={y}, width={width}, height={height}]";
			}
		}

		public enum CreateStatus
		{
			None,
			Ok,
			TooMuchGlypsDefined,
			TooMuchGlypsFound
		}

		public enum InputType
		{
			TextureAndGlyphList,
			Bitmap
		}

		public InputType inputType;

		public Texture2D texture;

		public bool forceWhite;

		public string glyphs;

		public int linesCount = 1;

		public float fillFactorTolerance = 0.01f;

		public TextAsset fntFile;

		public CreateStatus createStatus;

		public bool created;

		public Material material;

		public string dimensions;

		private MadFontData _data;

		public MadFontData data
		{
			get
			{
				if (_data == null || dirty)
				{
					_data = MadFontData.Parse(dimensions, texture);
					dirty = false;
				}
				return _data;
			}
		}

		public float textureAspect => (float)material.mainTexture.width / (float)material.mainTexture.height;

		public bool initialized
		{
			get;
			private set;
		}

		public bool dirty
		{
			get;
			set;
		}

		public int GetHashCode()
		{
			MadHashCode madHashCode = new MadHashCode();
			madHashCode.Add(texture);
			madHashCode.Add(glyphs);
			madHashCode.Add(linesCount);
			madHashCode.Add(fillFactorTolerance);
			madHashCode.Add(createStatus);
			madHashCode.Add(created);
			madHashCode.Add(material);
			madHashCode.Add(dimensions);
			return madHashCode.GetHashCode();
		}

		public Glyph GlyphFor(char c)
		{
			if (!data.charTable.ContainsKey(c))
			{
				if (c == ' ')
				{
					return Space();
				}
				return null;
			}
			MadFontData.Char @char = data.charTable[c];
			Glyph glyph = new Glyph();
			glyph.x = (float)@char.x / (float)data.commonScaleW;
			glyph.y = 1f * ((float)@char.y / (float)data.commonScaleH);
			glyph.width = (float)@char.width / (float)data.commonScaleW;
			glyph.height = (float)@char.height / (float)data.commonScaleH;
			glyph.xAdvance = (float)@char.xAdvance / (float)data.commonScaleW;
			glyph.xOffset = (float)@char.xOffset / (float)data.commonScaleW;
			glyph.yOffset = (float)(data.infoSize - (@char.yOffset + @char.height)) / (float)data.commonScaleH;
			glyph.widthPx = @char.width;
			glyph.heightPx = @char.height;
			return glyph;
		}

		private Glyph Space()
		{
			return GlyphFor('-') ?? GlyphFor('1');
		}
	}
}
