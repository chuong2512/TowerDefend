using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
	public bool calulateStartOnly = true;

	public float persentsY = 100f;

	private float _scaleFactorY = 1f;

	private float _xScaleDiff;

	private void Awake()
	{
		Vector3 localScale = base.transform.localScale;
		_scaleFactorY = localScale.y;
		Vector3 localScale2 = base.transform.localScale;
		float x = localScale2.x;
		Vector3 localScale3 = base.transform.localScale;
		_xScaleDiff = x / localScale3.y;
		if (calulateStartOnly)
		{
			placementCalculation();
		}
	}

	private void Update()
	{
		if (!calulateStartOnly)
		{
			placementCalculation();
		}
	}

	public void placementCalculation()
	{
		float num = (float)Screen.height / 100f * persentsY;
		Rect objectBounds = PreviewScreenUtil.getObjectBounds(base.gameObject);
		if (objectBounds.height < num)
		{
			while (objectBounds.height < num)
			{
				objectBounds = PreviewScreenUtil.getObjectBounds(base.gameObject);
				base.transform.localScale = new Vector3(_scaleFactorY * _xScaleDiff, _scaleFactorY, 0f);
				_scaleFactorY += 0.1f;
			}
			return;
		}
		while (objectBounds.height > num)
		{
			objectBounds = PreviewScreenUtil.getObjectBounds(base.gameObject);
			Transform transform = base.transform;
			Vector3 localScale = base.transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = base.transform.localScale;
			float x2 = x - localScale2.x * 0.1f;
			Vector3 localScale3 = base.transform.localScale;
			float y = localScale3.y;
			Vector3 localScale4 = base.transform.localScale;
			float y2 = y - localScale4.y * 0.1f;
			Vector3 localScale5 = base.transform.localScale;
			transform.localScale = new Vector3(x2, y2, localScale5.z);
		}
	}
}
