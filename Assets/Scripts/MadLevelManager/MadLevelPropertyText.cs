using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelPropertyText : MonoBehaviour
	{
		private MadLevelIcon _icon;

		public MadLevelIcon icon
		{
			get
			{
				if (_icon == null)
				{
					_icon = MadTransform.FindParent<MadLevelIcon>(base.transform);
				}
				return _icon;
			}
		}

		private void Start()
		{
			MadText component = GetComponent<MadText>();
			component.text = MadLevelProfile.GetLevelAny(icon.level.name, base.name, component.text);
		}
	}
}
