using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SIS
{
	public class IAPContainer : MonoBehaviour
	{
		public int maxCellSizeX;

		public int maxCellSizeY;

		private IEnumerator Start()
		{
			RectTransform rectTrans = GetComponent<RectTransform>();
			GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
			if (!(rectTrans != null) || !(grid != null))
			{
				yield break;
			}
			if (base.transform.childCount <= 0)
			{
				yield return new WaitForEndOfFrame();
			}
			if (base.transform.childCount == 0)
			{
				yield break;
			}
			RectTransform child = base.transform.GetChild(0).GetComponent<RectTransform>();
			switch (grid.startAxis)
			{
			case GridLayoutGroup.Axis.Vertical:
			{
				grid.cellSize = new Vector2(rectTrans.rect.width, child.rect.height);
				float num4 = child.rect.height * (float)base.transform.childCount;
				float num5 = num4;
				float num6 = base.transform.childCount - 1;
				Vector2 spacing2 = grid.spacing;
				num4 = num5 + (num6 * spacing2.y + (float)grid.padding.top + (float)grid.padding.bottom);
				RectTransform rectTransform2 = rectTrans;
				Vector2 sizeDelta2 = rectTrans.sizeDelta;
				rectTransform2.sizeDelta = new Vector2(sizeDelta2.x, num4);
				break;
			}
			case GridLayoutGroup.Axis.Horizontal:
			{
				grid.cellSize = new Vector2(child.rect.width, rectTrans.rect.height);
				float num = child.rect.width * (float)base.transform.childCount;
				float num2 = num;
				float num3 = base.transform.childCount - 1;
				Vector2 spacing = grid.spacing;
				num = num2 + (num3 * spacing.x + (float)grid.padding.left + (float)grid.padding.right);
				RectTransform rectTransform = rectTrans;
				float x = num;
				Vector2 sizeDelta = rectTrans.sizeDelta;
				rectTransform.sizeDelta = new Vector2(x, sizeDelta.y);
				break;
			}
			}
			if (maxCellSizeX > 0)
			{
				Vector2 cellSize = grid.cellSize;
				if (cellSize.x > (float)maxCellSizeX)
				{
					GridLayoutGroup gridLayoutGroup = grid;
					float x2 = maxCellSizeX;
					Vector2 cellSize2 = grid.cellSize;
					gridLayoutGroup.cellSize = new Vector2(x2, cellSize2.y);
				}
			}
			if (maxCellSizeY > 0)
			{
				Vector2 cellSize3 = grid.cellSize;
				if (cellSize3.y > (float)maxCellSizeY)
				{
					GridLayoutGroup gridLayoutGroup2 = grid;
					Vector2 cellSize4 = grid.cellSize;
					gridLayoutGroup2.cellSize = new Vector2(cellSize4.x, maxCellSizeY);
				}
			}
			grid.enabled = true;
		}
	}
}
