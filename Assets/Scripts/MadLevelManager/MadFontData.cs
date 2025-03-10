using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MadLevelManager
{
	public class MadFontData
	{
		public class Char
		{
			public char c;

			public int x;

			public int y;

			public int width;

			public int height;

			public int xOffset;

			public int yOffset;

			public int xAdvance;

			public int page;

			public int chnl;
		}

		public class Kerning
		{
			public char first;

			public char second;

			public int amount;
		}

		public Dictionary<char, Char> charTable = new Dictionary<char, Char>();

		public Dictionary<char, Dictionary<char, Kerning>> kerningTable = new Dictionary<char, Dictionary<char, Kerning>>();

		public string infoFace;

		public int infoSize;

		public bool infoBold;

		public bool infoItalic;

		public string infoCharset;

		public bool infoUnicode;

		public int infoStretchH;

		public bool infoSmooth;

		public bool infoAA;

		public int[] infoPadding;

		public int[] infoSpacing;

		public int commonLineHeight;

		public int commonBase;

		public int commonScaleW;

		public int commonScaleH;

		public int commonPages;

		public bool commonPacked;

		public int pageId;

		public string pageFile;

		public int charsCount;

		public int kerningCount;

		private string lineType;

		private Dictionary<string, string> lineMap = new Dictionary<string, string>();

		private MadFontData()
		{
		}

		public static MadFontData Parse(string text, Texture2D texture)
		{
			MadFontData madFontData = new MadFontData();
			if (text.StartsWith("1"))
			{
				madFontData.DoParseLegacy(text, texture);
			}
			else
			{
				madFontData.DoParse(text);
			}
			return madFontData;
		}

		private void DoParse(string text)
		{
			string[] array = text.Split('\n');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					UseLine(text2);
					switch (lineType)
					{
					case "info":
						ParseInfo();
						break;
					case "common":
						ParseCommon();
						break;
					case "page":
						ParsePage();
						break;
					case "chars":
						ParseChars();
						break;
					case "char":
						ParseChar();
						break;
					case "kernings":
						ParseKernings();
						break;
					case "kerning":
						ParseKerning();
						break;
					default:
						throw new MadDebug.AssertException("Unknown line type: " + lineType);
					}
				}
			}
		}

		private void UseLine(string line)
		{
			lineMap.Clear();
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			line = line.TrimEnd();
			for (int i = 0; i < line.Length; i++)
			{
				switch (line[i])
				{
				case '"':
					flag2 = !flag2;
					break;
				case ' ':
					if (!flag2)
					{
						string pair = line.Substring(num, i - num);
						if (!flag)
						{
							lineType = pair;
							flag = true;
						}
						else
						{
							ReadPair(pair);
						}
						num = i + 1;
					}
					break;
				}
			}
			string pair2 = line.Substring(num);
			ReadPair(pair2);
		}

		private void ReadPair(string pair)
		{
			if (!string.IsNullOrEmpty(pair))
			{
				int num = pair.IndexOf("=");
				MadDebug.Assert(num != -1, "Delimiter not found");
				string key = pair.Substring(0, num);
				string value = pair.Substring(num + 1);
				lineMap[key] = value;
			}
		}

		private void ParseInfo()
		{
			infoFace = GetString("face");
			infoSize = GetInt("size");
			infoBold = GetBool("bold");
			infoItalic = GetBool("italic");
			infoCharset = GetString("charset");
			infoUnicode = GetBool("unicode");
			infoStretchH = GetInt("stretchH");
			infoSmooth = GetBool("smooth");
			infoAA = GetBool("aa");
			infoPadding = GetIntArray("padding");
			infoSpacing = GetIntArray("spacing");
		}

		private void ParseCommon()
		{
			commonLineHeight = GetInt("lineHeight");
			commonBase = GetInt("base");
			commonScaleW = GetInt("scaleW");
			commonScaleH = GetInt("scaleH");
			commonPages = GetInt("pages");
			commonPacked = GetBool("packed");
		}

		private void ParsePage()
		{
			pageId = GetInt("id");
			pageFile = GetString("file");
		}

		private void ParseChars()
		{
			charsCount = GetInt("count");
		}

		private void ParseChar()
		{
			Char @char = new Char();
			@char.c = (char)GetInt("id");
			@char.x = GetInt("x");
			@char.y = GetInt("y");
			@char.width = GetInt("width");
			@char.height = GetInt("height");
			@char.xOffset = GetInt("xoffset");
			@char.yOffset = GetInt("yoffset");
			@char.xAdvance = GetInt("xadvance");
			@char.page = GetInt("page");
			@char.chnl = GetInt("chnl");
			charTable.Add(@char.c, @char);
		}

		private void ParseKernings()
		{
			kerningCount = GetInt("count");
		}

		private void ParseKerning()
		{
			Kerning kerning = new Kerning();
			kerning.first = (char)GetInt("first");
			kerning.second = (char)GetInt("second");
			kerning.amount = GetInt("amount");
			Dictionary<char, Kerning> dictionary;
			if (kerningTable.ContainsKey(kerning.first))
			{
				dictionary = kerningTable[kerning.first];
			}
			else
			{
				dictionary = new Dictionary<char, Kerning>();
				kerningTable[kerning.first] = dictionary;
			}
			dictionary.Add(kerning.second, kerning);
		}

		private string GetString(string key)
		{
			string value = GetValue(key);
			MadDebug.Assert(value != null, "Key " + key + " not found");
			MadDebug.Assert(value[0] == '"' && value[value.Length - 1] == '"', "Key " + key + " not a string");
			return value.Substring(1, value.Length - 2);
		}

		private bool GetBool(string key)
		{
			return GetInt(key) > 0;
		}

		private int GetInt(string key)
		{
			string value = GetValue(key);
			MadDebug.Assert(value != null, "Key " + key + " not found");
			MadDebug.Assert(int.TryParse(value, out int result), "Key " + key + "not a int");
			return result;
		}

		private int[] GetIntArray(string key)
		{
			string value = GetValue(key);
			MadDebug.Assert(value != null, "Key " + key + " not found");
			string[] array = value.Split(',');
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = int.Parse(array[i]);
			}
			return array2;
		}

		private string GetValue(string key)
		{
			if (lineMap.ContainsKey(key))
			{
				return lineMap[key];
			}
			return null;
		}

		private void DoParseLegacy(string data, Texture2D texture)
		{
			charTable.Clear();
			int width = texture.width;
			int height = texture.height;
			commonScaleW = width;
			commonScaleH = height;
			string[] array = data.Split(new char[1]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 1; i < array.Length; i++)
			{
				string text = array[i];
				if (!string.IsNullOrEmpty(text))
				{
					string[] array2 = text.Split(' ');
					char c = array2[0][0];
					float num = float.Parse(array2[1], CultureInfo.InvariantCulture);
					float num2 = float.Parse(array2[2], CultureInfo.InvariantCulture);
					float num3 = float.Parse(array2[3], CultureInfo.InvariantCulture);
					float num4 = float.Parse(array2[4], CultureInfo.InvariantCulture);
					Char @char = new Char();
					@char.c = c;
					@char.x = (int)(num * (float)width);
					@char.y = (int)(num2 * (float)height);
					@char.width = (int)(num3 * (float)width);
					@char.height = (int)(num4 * (float)height);
					@char.xAdvance = (int)(num3 * (float)width + 3f);
					if (!charTable.ContainsKey(c))
					{
						charTable.Add(c, @char);
					}
					infoSize = @char.height;
				}
			}
		}
	}
}
