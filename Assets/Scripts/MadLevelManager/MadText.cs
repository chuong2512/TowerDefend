using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MadLevelManager
{
	public class MadText : MadSprite
	{
		public enum Align
		{
			Left,
			Center,
			Right
		}

		public MadFont font;

		public MadAtlas atlas;

		public string text = string.Empty;

		public Align align;

		public float scale = 24f;

		public float letterSpacing = 1f;

		public bool wordWrap;

		public float wordWrapLength = 1000f;

		private int hash;

		private int linesCount;

		private Rect bounds;

		private List<string> lines = new List<string>();

		private List<float> lineWidths = new List<float>();

		[SerializeField]
		public string fontTextureGUID;

		public override Rect GetBounds()
		{
			return bounds;
		}

		private void UpdateTextIfNeeded()
		{
			int num = 37;
			num += MadHashCode.Add(num, text);
			num += MadHashCode.Add(num, wordWrap);
			num += MadHashCode.Add(num, wordWrapLength);
			num += MadHashCode.Add(num, scale);
			if (num != hash)
			{
				UpdateText();
				hash = num;
			}
		}

		private void UpdateText()
		{
			lineWidths.Clear();
			lines.Clear();
			if (!CanDraw())
			{
				linesCount = 0;
				bounds = default(Rect);
				return;
			}
			string[] array = text.Split('\n');
			linesCount = 0;
			float num = 0f;
			foreach (string item in array)
			{
				float num2 = LineWidth(item);
				if (wordWrap && num2 > wordWrapLength)
				{
					List<string> list = WordWrap(item, num2);
					for (int j = 0; j < list.Count; j++)
					{
						lines.Add(list[j]);
						float num3 = LineWidth(list[j]);
						lineWidths.Add(num3);
						num = Mathf.Max(num, num3);
						linesCount++;
					}
				}
				else
				{
					lines.Add(item);
					num = Mathf.Max(num, num2);
					lineWidths.Add(num2);
					linesCount++;
				}
			}
			float num4 = scale * (float)linesCount;
			Rect rect = new Rect(0f, 0f, num, num4);
			UpdatePivotPoint();
			Vector3 vector = PivotPointTranslate(new Vector2(0f, 0f), rect);
			Vector3 vector2 = PivotPointTranslate(new Vector2(num, num4), rect);
			bounds = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
			UpdateCollider();
		}

		private void UpdateCollider()
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				component.center = bounds.center;
				component.size = new Vector3(bounds.width, bounds.height, 0.01f);
			}
		}

		private float LineWidth(string text)
		{
			float num = 0f;
			foreach (char c in text)
			{
				MadFont.Glyph glyph = font.GlyphFor(c);
				if (glyph == null)
				{
					UnityEngine.Debug.LogWarning("No glyph found for '" + c + "' (code " + (int)c + ")");
					continue;
				}
				GlyphBounds(glyph, out float xAdvance);
				num += xAdvance;
			}
			return num;
		}

		private List<string> WordWrap(string text, float lineWidth)
		{
			string[] array = text.Split(' ');
			float num = 0f;
			float num2 = LineWidth(" ");
			int num3 = 0;
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				float num4 = LineWidth(array[i]);
				if (num + num2 + num4 > wordWrapLength)
				{
					if (i != num3)
					{
						string item = Join(array, num3, i - num3, " ");
						list.Add(item);
						num3 = i;
						num = num4;
					}
					else
					{
						string item2 = Join(array, num3, 1, " ");
						list.Add(item2);
						num3 = i + 1;
						num = 0f;
					}
				}
				else
				{
					num += num2 + num4;
				}
			}
			if (num3 < array.Length)
			{
				string item3 = Join(array, num3, array.Length - num3, " ");
				list.Add(item3);
			}
			return list;
		}

		private string Join(string[] words, int index, int count, string glue)
		{
			if (count <= 0)
			{
				UnityEngine.Debug.LogError("Illegal argument: " + count);
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(count * 7);
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(glue);
				}
				stringBuilder.Append(words[index + i]);
			}
			return stringBuilder.ToString();
		}

		private Rect GlyphBounds(MadFont.Glyph g, out float xAdvance)
		{
			float num = scale;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = (float)font.data.infoSize / (float)font.data.commonScaleH;
			num = g.height / num4 * scale;
			num2 = g.xOffset / num4 * scale;
			num3 = g.yOffset / num4 * scale;
			xAdvance = g.xAdvance / num4 * scale * font.textureAspect;
			float num5 = num / g.height * g.width;
			return new Rect(num2, num3, num5 * font.textureAspect, num);
		}

		public override bool CanDraw()
		{
			return font != null && !string.IsNullOrEmpty(text);
		}

		public override Material GetMaterial()
		{
			if (atlas != null)
			{
				if (atlas.GetItem(fontTextureGUID) != null)
				{
					return panel.materialStore.MaterialFor(atlas.atlasTexture, "Tools/Mad Level Manager/Unlit/Transparent Tint");
				}
				return GetFontMaterial();
			}
			return GetFontMaterial();
		}

		private Material GetFontMaterial()
		{
			if (panel.renderMode == MadPanel.RenderMode.DepthBased)
			{
				string name = font.material.shader.name;
				if (name == "Tools/Mad Level Manager/Unlit/Font")
				{
					return panel.materialStore.MaterialFor(font.texture, "Tools/Mad Level Manager/Unlit/Font Depth Based");
				}
				if (name == "Tools/Mad Level Manager/Unlit/Font White")
				{
					return panel.materialStore.MaterialFor(font.texture, "Tools/Mad Level Manager/Unlit/Font White Depth Based");
				}
				UnityEngine.Debug.LogError("Unknown font shader: " + name);
				return font.material;
			}
			return font.material;
		}

		public override void DrawOn(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv, ref MadList<int> triangles, out Material material)
		{
			UpdateTextIfNeeded();
			UpdatePivotPoint();
			Matrix4x4 matrix4x = TransformMatrix();
			Rect rect = GetBounds();
			MadAtlas.Item item = null;
			if (atlas != null)
			{
				item = atlas.GetItem(fontTextureGUID);
			}
			material = GetMaterial();
			float num = 0f;
			float num2 = (float)linesCount * scale - scale;
			for (int i = 0; i < linesCount; i++)
			{
				string text = lines[i];
				float num3 = lineWidths[i];
				switch (align)
				{
				case Align.Left:
					num = 0f;
					break;
				case Align.Center:
					num = (rect.width - num3) / 2f;
					break;
				case Align.Right:
					num = rect.width - num3;
					break;
				default:
					UnityEngine.Debug.LogError("Unknown align: " + align);
					num = 0f;
					break;
				}
				foreach (char c in text)
				{
					int count = vertices.Count;
					MadFont.Glyph glyph = font.GlyphFor(c);
					if (glyph == null)
					{
						UnityEngine.Debug.LogWarning("Glyph not found: '" + c + "' (code " + (int)c + ")");
						continue;
					}
					float xAdvance;
					Rect rect2 = GlyphBounds(glyph, out xAdvance);
					if (c != ' ')
					{
						float num4 = num + rect2.x;
						float num5 = num2 + rect2.y;
						float x = num4 + rect2.width;
						float y = num5 + rect2.height;
						vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num4, num5, 0f), rect)));
						vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(num4, y, 0f), rect)));
						vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(x, y, 0f), rect)));
						vertices.Add(matrix4x.MultiplyPoint(PivotPointTranslate(new Vector3(x, num5, 0f), rect)));
						colors.Add(tint);
						colors.Add(tint);
						colors.Add(tint);
						colors.Add(tint);
						uv.Add(FixUV(new Vector2(glyph.uMin, glyph.vMin), item));
						uv.Add(FixUV(new Vector2(glyph.uMin, glyph.vMax), item));
						uv.Add(FixUV(new Vector2(glyph.uMax, glyph.vMax), item));
						uv.Add(FixUV(new Vector2(glyph.uMax, glyph.vMin), item));
						triangles.Add(count);
						triangles.Add(1 + count);
						triangles.Add(2 + count);
						triangles.Add(count);
						triangles.Add(2 + count);
						triangles.Add(3 + count);
					}
					num += xAdvance;
				}
				num2 -= scale;
			}
		}

		private Vector2 FixUV(Vector2 uv, MadAtlas.Item item)
		{
			if (item != null)
			{
				Rect region = item.region;
				return new Vector2(region.x + region.width * uv.x, region.y + region.height * uv.y);
			}
			return uv;
		}
	}
}
