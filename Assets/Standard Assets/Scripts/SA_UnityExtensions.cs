using SA.Common.Animation;
using System;
using UnityEngine;

public static class SA_UnityExtensions
{
	public static void MoveTo(this GameObject go, Vector3 position, float time, EaseType easeType = EaseType.linear, Action OnCompleteAction = null)
	{
		ValuesTween valuesTween = go.AddComponent<ValuesTween>();
		valuesTween.DestoryGameObjectOnComplete = false;
		valuesTween.VectorTo(go.transform.position, position, time, easeType);
		valuesTween.OnComplete += OnCompleteAction;
	}

	public static void ScaleTo(this GameObject go, Vector3 scale, float time, EaseType easeType = EaseType.linear, Action OnCompleteAction = null)
	{
		ValuesTween valuesTween = go.AddComponent<ValuesTween>();
		valuesTween.DestoryGameObjectOnComplete = false;
		valuesTween.ScaleTo(go.transform.localScale, scale, time, easeType);
		valuesTween.OnComplete += OnCompleteAction;
	}

	public static Sprite ToSprite(this Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}
}
