using System;
using UnityEngine;

public class AndroidImagePickResult : AndroidActivityResult
{
	private Texture2D _Image;

	private string _ImagePath = string.Empty;

	[Obsolete("image is deprecated, please use Image instead.")]
	public Texture2D image => _Image;

	public Texture2D Image => _Image;

	public string ImagePath => _ImagePath;

	public AndroidImagePickResult(string codeString, string ImageData, string ImagePathInfo)
		: base("0", codeString)
	{
		if (ImageData.Length > 0)
		{
			byte[] data = Convert.FromBase64String(ImageData);
			_Image = new Texture2D(1, 1, TextureFormat.DXT5, mipChain: false);
			_Image.LoadImage(data);
		}
		_ImagePath = ImagePathInfo;
	}
}
