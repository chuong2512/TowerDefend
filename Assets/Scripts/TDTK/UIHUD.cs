using MadLevelManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIHUD : MonoBehaviour
	{
		public Text txtLife;

		public Text txtWave;

		public Text txtLvname;

		public List<UIObject> rscItemList = new List<UIObject>();

		public Text txtTimer;

		public UIButton butSpawn;

		private Vector3 butSpawnDefaultPos;

		public UIButton butFF;

		public GameObject butPerkMenuObj;

		private static UIHUD instance;

		public static UIHUD GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			List<Rsc> resourceList = ResourceManager.GetResourceList();
			for (int i = 0; i < resourceList.Count; i++)
			{
				if (i == 0)
				{
					rscItemList[0].Init();
				}
				else
				{
					rscItemList.Add(UIObject.Clone(rscItemList[0].rootObj, "Rsc" + (i + 1)));
				}
				rscItemList[i].imgRoot.sprite = resourceList[i].icon;
				rscItemList[i].label.text = resourceList[i].value.ToString();
			}
			txtTimer.text = string.Empty;
			txtLvname.text = MadLevel.currentLevelName;
			butSpawn.Init();
			butSpawnDefaultPos = butSpawn.rectT.localPosition;
			butFF.Init();
			butPerkMenuObj.SetActive(PerkManager.IsOn());
			OnLife(0);
			OnNewWave(0);
			OnEnableSpawn();
		}

		private void OnEnable()
		{
			TDTK.onLifeE += OnLife;
			TDTK.onFastForwardE += OnFastForward;
			TDTK.onNewWaveE += OnNewWave;
			TDTK.onEnableSpawnE += OnEnableSpawn;
			TDTK.onResourceE += OnResourceChanged;
		}

		private void OnDisable()
		{
			TDTK.onLifeE -= OnLife;
			TDTK.onFastForwardE -= OnFastForward;
			TDTK.onNewWaveE -= OnNewWave;
			TDTK.onEnableSpawnE -= OnEnableSpawn;
			TDTK.onResourceE -= OnResourceChanged;
		}

		private void OnLife(int changedvalue)
		{
			int playerLifeCap = GameControl.GetPlayerLifeCap();
			string arg = (playerLifeCap <= 0) ? string.Empty : ("/" + GameControl.GetPlayerLifeCap());
			txtLife.text = GameControl.GetPlayerLife() + arg;
		}

		private void OnResourceChanged(List<int> valueChangedList)
		{
			List<Rsc> resourceList = ResourceManager.GetResourceList();
			for (int i = 0; i < rscItemList.Count; i++)
			{
				rscItemList[i].label.text = resourceList[i].value.ToString();
			}
		}

		private void OnNewWave(int waveID)
		{
			int totalWaveCount = SpawnManager.GetTotalWaveCount();
			string arg = (totalWaveCount <= 0) ? string.Empty : ("/" + totalWaveCount);
			txtWave.text = "Wave: " + waveID + arg;
			butSpawn.rectT.localPosition = new Vector3(0f, 99999f, 0f);
			if (waveID > 0)
			{
				butSpawn.label.text = "Next Wave";
			}
		}

		private void OnEnableSpawn()
		{
			butSpawn.rectT.localPosition = butSpawnDefaultPos;
			butSpawn.rootObj.SetActive(value: true);
		}

		public void OnSpawnButton()
		{
			SpawnManager.Spawn();
			butSpawn.rectT.localPosition = new Vector3(0f, 99999f, 0f);
			butSpawn.label.text = "Next Wave";
		}

		public void OnFFButton()
		{
			GameControl.FastForward(!GameControl.IsFastForwardOn());
		}

		public void OnFastForward(bool flag)
		{
			butFF.imgIcon.enabled = !flag;
		}

		public void OnPerkButton()
		{
			UIMainControl.OnPerkMenu();
		}

		public void OnMenuButton()
		{
			UIMainControl.TogglePause();
		}

		private void Update()
		{
			float timeToNextSpawn = SpawnManager.GetTimeToNextSpawn();
			if (timeToNextSpawn > 0f)
			{
				if (timeToNextSpawn < 60f)
				{
					txtTimer.text = "Next Wave in " + timeToNextSpawn.ToString("f1") + "s";
				}
				else
				{
					txtTimer.text = "Next Wave in " + Mathf.Floor(timeToNextSpawn / 60f).ToString("f0") + "m";
				}
			}
			else
			{
				txtTimer.text = string.Empty;
			}
		}
	}
}
