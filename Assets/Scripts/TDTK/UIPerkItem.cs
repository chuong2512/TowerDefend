using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	[Serializable]
	public class UIPerkItem : UIButton
	{
		public int perkID = -1;

		[HideInInspector]
		public GameObject selectHighlight;

		[HideInInspector]
		public GameObject purchasedHighlight;

		[HideInInspector]
		public GameObject unavailableHighlight;

		[HideInInspector]
		public GameObject connector;

		[HideInInspector]
		public GameObject connectorBG;

		public UIPerkItem()
		{
		}

		public UIPerkItem(GameObject obj)
		{
			rootObj = obj;
			Init();
		}

		public override void Init()
		{
			base.Init();
			button = rootObj.GetComponent<Button>();
			IEnumerator enumerator = rectT.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform.name == "SelectHighlight")
					{
						selectHighlight = transform.gameObject;
						selectHighlight.SetActive(value: false);
					}
					else if (transform.name == "PurchasedHighlight")
					{
						purchasedHighlight = transform.gameObject;
					}
					else if (transform.name == "UnavailableHighlight")
					{
						unavailableHighlight = transform.gameObject;
					}
					else if (transform.name == "Connector")
					{
						connector = transform.gameObject;
					}
					else if (transform.name == "ConnectorBG")
					{
						connectorBG = transform.gameObject;
					}
					if (connectorBG != null && connector != null)
					{
						connector.transform.SetParent(connectorBG.transform);
						connectorBG.transform.SetParent(rootT.parent);
						connector.SetActive(value: false);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public new static UIPerkItem Clone(GameObject srcObj, string name = "", Vector3 posOffset = default(Vector3))
		{
			GameObject obj = UI.Clone(srcObj, name, posOffset);
			return new UIPerkItem(obj);
		}
	}
}
