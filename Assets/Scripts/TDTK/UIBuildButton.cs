using MadLevelManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIBuildButton : MonoBehaviour
	{
		[Header("Buttons")]
		public HorizontalLayoutGroup layoutGroup;

		public List<UIButton> buttonList = new List<UIButton>();

		private List<UIButton> activeButtonList = new List<UIButton>();

		public GameObject butCancelObj;

		private CanvasGroup butCancelCanvasG;

		private BuildInfo buildInfo;

		[Header("Tooltip")]
		public GameObject tooltipObj;

		private CanvasGroup tooltipCanvasG;

		public Text lbTooltipName;

		public Text lbTooltipDesp;

		public List<UIObject> tooltipRscItemList = new List<UIObject>();

		private int numUnlock;

		private GameObject thisObj;

		private RectTransform rectT;

		private CanvasGroup canvasGroup;

		private static UIBuildButton instance;

		private List<UnitTower> TowerDbList;

		private List<UnitStat> TowerStats;

		private int GunJetRange;

		private int GunJetDamage;

		private int GunJetCooldown;

		private int GunJetCrift;

		private int GunJetStun;

		private int SlowRange;

		private int SlowCooldown;

		private int SlowDamage;

		private int SlowTime;

		private int CannonRange;

		private int CannonDamage;

		private int CannonCooldown;

		private int CannonCrift;

		private int CannonStun;

		private int LaserRange;

		private int LaserDamage;

		private int LaserCooldown;

		private int LaserCrift;

		private int LaserStun;

		private int BombRange;

		private int BombDamage;

		private int BombCooldown;

		private int BombCrift;

		private int BombStun;

		private int SunrayRange;

		private int SunrayDamage;

		private int SunrayCooldown;

		private int SunrayCrift;

		private int SunrayStun;

		private int BeamRange;

		private int BeamDamage;

		private int BeamCooldown;

		private int BeamCrift;

		private int BeamStun;

		private int currentButtonID = -1;

		private Transform piePosDummyT;

		public static UIBuildButton GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			instance = this;
			thisObj = base.gameObject;
			rectT = thisObj.GetComponent<RectTransform>();
			canvasGroup = thisObj.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = thisObj.AddComponent<CanvasGroup>();
			}
			tooltipCanvasG = tooltipObj.GetComponent<CanvasGroup>();
			tooltipCanvasG.alpha = 0f;
			rectT = thisObj.GetComponent<RectTransform>();
			if (ES2.Exists("BeamRange"))
			{
				GunJetRange = ES2.Load<int>("GunJetRange");
				GunJetDamage = ES2.Load<int>("GunJetDamage");
				GunJetCooldown = ES2.Load<int>("GunJetCooldown");
				GunJetCrift = ES2.Load<int>("GunJetCrift");
				GunJetStun = ES2.Load<int>("GunJetStun");
				SlowRange = ES2.Load<int>("SlowRange");
				SlowCooldown = ES2.Load<int>("SlowCooldown");
				SlowDamage = ES2.Load<int>("SlowDamage");
				SlowTime = ES2.Load<int>("SlowTime");
				CannonRange = ES2.Load<int>("CannonRange");
				CannonDamage = ES2.Load<int>("CannonDamage");
				CannonCooldown = ES2.Load<int>("CannonCooldown");
				CannonCrift = ES2.Load<int>("CannonCrift");
				CannonStun = ES2.Load<int>("CannonStun");
				LaserRange = ES2.Load<int>("LaserRange");
				LaserDamage = ES2.Load<int>("LaserDamage");
				LaserCooldown = ES2.Load<int>("LaserCooldown");
				LaserCrift = ES2.Load<int>("LaserCrift");
				LaserStun = ES2.Load<int>("LaserStun");
				BombRange = ES2.Load<int>("BombRange");
				BombDamage = ES2.Load<int>("BombDamage");
				BombCooldown = ES2.Load<int>("BombCooldown");
				BombCrift = ES2.Load<int>("BombCrift");
				BombStun = ES2.Load<int>("BombStun");
				SunrayRange = ES2.Load<int>("SunrayRange");
				SunrayDamage = ES2.Load<int>("SunrayDamage");
				SunrayCooldown = ES2.Load<int>("SunrayCooldown");
				SunrayCrift = ES2.Load<int>("SunrayCrift");
				SunrayStun = ES2.Load<int>("SunrayStun");
				BeamRange = ES2.Load<int>("BeamRange");
				BeamDamage = ES2.Load<int>("BeamDamage");
				BeamCooldown = ES2.Load<int>("BeamCooldown");
				BeamCrift = ES2.Load<int>("BeamCrift");
				BeamStun = ES2.Load<int>("BeamStun");
			}
		}

		private void Start()
		{
			if (BuildManager.GetInstance() == null)
			{
				return;
			}
			List<UnitTower> towerList = BuildManager.GetTowerList();
			GetTowerUpdate();
			if (MadLevel.currentLevelName == "EndLess")
			{
				if (CheckLevelComplete() >= 4 && CheckLevelComplete() < 7)
				{
					numUnlock = 3;
				}
				else if (CheckLevelComplete() >= 7 && CheckLevelComplete() < 10)
				{
					numUnlock = 2;
				}
				else if (CheckLevelComplete() >= 10 && CheckLevelComplete() < 15)
				{
					numUnlock = 1;
				}
				else if (CheckLevelComplete() >= 15 && CheckLevelComplete() < 50)
				{
					numUnlock = 0;
				}
				else if (CheckLevelComplete() >= 0 && CheckLevelComplete() < 4)
				{
					numUnlock = 4;
				}
			}
			else
			{
				numUnlock = 0;
			}
			for (int i = 0; i < towerList.Count - numUnlock; i++)
			{
				if (i == 0)
				{
					buttonList[0].Init();
				}
				else if (i > 0)
				{
					buttonList.Add(UIButton.Clone(buttonList[0].rootObj, "BuildButton" + (i + 1)));
				}
				buttonList[i].imgIcon.sprite = towerList[i].iconSprite;
				buttonList[i].imgHighlight.enabled = false;
				if (UIMainControl.InTouchMode())
				{
					buttonList[i].SetCallback(null, null, OnTowerButton);
				}
				else
				{
					buttonList[i].SetCallback(OnHoverButton, OnExitButton, OnTowerButton);
				}
			}
			if (!BuildManager.UseDragNDrop())
			{
				canvasGroup.alpha = 0f;
				rectT.localPosition = new Vector3(0f, 99999f, 0f);
			}
			if (!BuildManager.UseDragNDrop() && UIMainControl.UsePieMenu())
			{
				layoutGroup.enabled = false;
				tooltipObj.transform.localPosition -= new Vector3(0f, 60f, 0f);
			}
			else
			{
				layoutGroup.enabled = true;
			}
			List<Rsc> resourceList = ResourceManager.GetResourceList();
			for (int j = 0; j < resourceList.Count; j++)
			{
				if (j == 0)
				{
					tooltipRscItemList[0].Init();
				}
				else
				{
					tooltipRscItemList.Add(UIObject.Clone(tooltipRscItemList[0].rootObj, "Rsc" + (j + 1)));
				}
				tooltipRscItemList[j].imgRoot.sprite = resourceList[j].icon;
				tooltipRscItemList[j].label.text = resourceList[j].value.ToString();
			}
			if (!BuildManager.UseDragNDrop())
			{
				butCancelObj.SetActive(value: false);
			}
			else
			{
				butCancelCanvasG = butCancelObj.AddComponent<CanvasGroup>();
				butCancelObj.transform.SetAsLastSibling();
				OnDragNDrop(flag: false);
			}
			tooltipObj.SetActive(value: false);
		}

		private int CheckLevelComplete()
		{
			string text = MadLevel.FindLastUnlockedLevelName();
			return int.Parse(text.Substring(6));
		}

		private void OnNewBuildableTower(UnitTower tower)
		{
			buttonList.Add(UIButton.Clone(buttonList[0].rootObj, "BuildButton" + (buttonList.Count + 1)));
			buttonList[buttonList.Count - 1].imgIcon.sprite = tower.iconSprite;
			buttonList[buttonList.Count - 1].SetCallback(OnHoverButton, OnExitButton, OnTowerButton);
			butCancelObj.transform.SetAsLastSibling();
			UnityEngine.Debug.Log("11");
		}

		private void OnEnable()
		{
			TDTK.onNewBuildableTowerE += OnNewBuildableTower;
			TDTK.onDragNDropE += OnDragNDrop;
		}

		private void OnDisable()
		{
			TDTK.onNewBuildableTowerE -= OnNewBuildableTower;
			TDTK.onDragNDropE -= OnDragNDrop;
		}

		private void OnDragNDrop(bool flag)
		{
			if (BuildManager.UseDragNDrop())
			{
				butCancelCanvasG.alpha = (flag ? 1 : 0);
				butCancelCanvasG.interactable = flag;
			}
		}

		private void GetTowerUpdate()
		{
			TowerDbList = TowerDB.Load();
			List<UnitStat> stats = TowerDbList[0].stats;
			List<UnitStat> stats2 = TowerDbList[1].stats;
			List<UnitStat> stats3 = TowerDbList[2].stats;
			List<UnitStat> stats4 = TowerDbList[3].stats;
			List<UnitStat> stats5 = TowerDbList[4].stats;
			List<UnitStat> stats6 = TowerDbList[5].stats;
			List<UnitStat> stats7 = TowerDbList[6].stats;
			List<UnitStat> stats8 = TowerDbList[7].stats;
			List<UnitStat> stats9 = TowerDbList[8].stats;
			List<UnitStat> stats10 = TowerDbList[9].stats;
			List<UnitStat> stats11 = TowerDbList[10].stats;
			List<UnitStat> stats12 = TowerDbList[11].stats;
			List<UnitStat> stats13 = TowerDbList[12].stats;
			List<UnitStat> stats14 = TowerDbList[13].stats;
			List<UnitStat> stats15 = TowerDbList[14].stats;
			List<UnitStat> stats16 = TowerDbList[15].stats;
			List<UnitStat> stats17 = TowerDbList[16].stats;
			List<UnitStat> stats18 = TowerDbList[17].stats;
			List<UnitStat> stats19 = TowerDbList[18].stats;
			List<UnitStat> stats20 = TowerDbList[19].stats;
			List<UnitStat> stats21 = TowerDbList[20].stats;
			List<UnitStat> stats22 = TowerDbList[21].stats;
			List<UnitStat> stats23 = TowerDbList[22].stats;
			List<UnitStat> stats24 = TowerDbList[23].stats;
			List<UnitStat> stats25 = TowerDbList[24].stats;
			List<UnitStat> stats26 = TowerDbList[25].stats;
			List<UnitStat> stats27 = TowerDbList[26].stats;
			List<UnitStat> stats28 = TowerDbList[27].stats;
			switch (GunJetRange)
			{
			case 0:
				stats[0].range = 3.5f;
				stats2[0].range = 3.8f;
				stats3[0].range = 4.1f;
				stats4[0].range = 4.5f;
				break;
			case 1:
				stats[0].range = 3.74500012f;
				stats2[0].range = 4.066f;
				stats3[0].range = 4.387f;
				stats4[0].range = 4.815f;
				break;
			case 2:
				stats[0].range = 3.955f;
				stats2[0].range = 4.294f;
				stats3[0].range = 4.633f;
				stats4[0].range = 5.085f;
				break;
			case 3:
				stats[0].range = 4.20000029f;
				stats2[0].range = 4.56f;
				stats3[0].range = 4.92f;
				stats4[0].range = 5.4f;
				break;
			case 4:
				stats[0].range = 4.62000036f;
				stats2[0].range = 5.01600027f;
				stats3[0].range = 5.412f;
				stats4[0].range = 5.94f;
				break;
			case 5:
				stats[0].range = 5.07500029f;
				stats2[0].range = 5.51f;
				stats3[0].range = 5.945f;
				stats4[0].range = 6.525f;
				break;
			}
			switch (GunJetDamage)
			{
			case 0:
				stats[0].damageMax = 1f;
				stats[0].damageMin = 0.5f;
				stats2[0].damageMax = 2f;
				stats2[0].damageMin = 1f;
				stats3[0].damageMax = 3f;
				stats3[0].damageMin = 2f;
				stats4[0].damageMax = 6f;
				stats4[0].damageMin = 4f;
				break;
			case 1:
				stats[0].damageMax = 1.1f;
				stats[0].damageMin = 0.55f;
				stats2[0].damageMax = 2.2f;
				stats2[0].damageMin = 1.1f;
				stats3[0].damageMax = 3.30000019f;
				stats3[0].damageMin = 2.2f;
				stats4[0].damageMax = 6.60000038f;
				stats4[0].damageMin = 4.4f;
				break;
			case 2:
				stats[0].damageMax = 1.25f;
				stats[0].damageMin = 0.625f;
				stats2[0].damageMax = 2.5f;
				stats2[0].damageMin = 1.25f;
				stats3[0].damageMax = 3.75f;
				stats3[0].damageMin = 2.5f;
				stats4[0].damageMax = 7.5f;
				stats4[0].damageMin = 5f;
				break;
			case 3:
				stats[0].damageMax = 1.5f;
				stats[0].damageMin = 0.75f;
				stats2[0].damageMax = 3f;
				stats2[0].damageMin = 1.5f;
				stats3[0].damageMax = 4.5f;
				stats3[0].damageMin = 3f;
				stats4[0].damageMax = 9f;
				stats4[0].damageMin = 6f;
				break;
			case 4:
				stats[0].damageMax = 1.85f;
				stats[0].damageMin = 0.925f;
				stats2[0].damageMax = 3.7f;
				stats2[0].damageMin = 1.85f;
				stats3[0].damageMax = 5.55f;
				stats3[0].damageMin = 3.7f;
				stats4[0].damageMax = 11.1f;
				stats4[0].damageMin = 7.4f;
				break;
			case 5:
				stats[0].damageMax = 2.2f;
				stats[0].damageMin = 1.1f;
				stats2[0].damageMax = 4.4f;
				stats2[0].damageMin = 2.2f;
				stats3[0].damageMax = 6.60000038f;
				stats3[0].damageMin = 4.4f;
				stats4[0].damageMax = 13.2000008f;
				stats4[0].damageMin = 8.8f;
				break;
			}
			switch (GunJetCooldown)
			{
			case 0:
				stats[0].cooldown = 0.2f;
				stats2[0].cooldown = 0.2f;
				stats3[0].cooldown = 0.2f;
				stats4[0].cooldown = 0.2f;
				break;
			case 1:
				stats[0].cooldown = 0.93f;
				stats2[0].cooldown = 0.93f;
				stats3[0].cooldown = 0.93f;
				stats4[0].cooldown = 0.93f;
				break;
			case 2:
				stats[0].cooldown = 0.87f;
				stats2[0].cooldown = 0.87f;
				stats3[0].cooldown = 0.87f;
				stats4[0].cooldown = 0.87f;
				break;
			case 3:
				stats[0].cooldown = 0.75f;
				stats2[0].cooldown = 0.75f;
				stats3[0].cooldown = 0.75f;
				stats4[0].cooldown = 0.75f;
				break;
			case 4:
				stats[0].cooldown = 0.66f;
				stats2[0].cooldown = 0.66f;
				stats3[0].cooldown = 0.66f;
				stats4[0].cooldown = 0.66f;
				break;
			case 5:
				stats[0].cooldown = 0.55f;
				stats2[0].cooldown = 0.55f;
				stats3[0].cooldown = 0.55f;
				stats4[0].cooldown = 0.55f;
				break;
			}
			switch (GunJetCrift)
			{
			case 0:
				stats[0].crit.chance = 0f;
				stats2[0].crit.chance = 0f;
				stats3[0].crit.chance = 0f;
				stats4[0].crit.chance = 0f;
				break;
			case 1:
				stats[0].crit.chance = 0.03f;
				stats2[0].crit.chance = 0.03f;
				stats3[0].crit.chance = 0.03f;
				stats4[0].crit.chance = 0.03f;
				break;
			case 2:
				stats[0].crit.chance = 0.06f;
				stats2[0].crit.chance = 0.06f;
				stats3[0].crit.chance = 0.06f;
				stats4[0].crit.chance = 0.06f;
				break;
			case 3:
				stats[0].crit.chance = 0.13f;
				stats2[0].crit.chance = 0.13f;
				stats3[0].crit.chance = 0.13f;
				stats4[0].crit.chance = 0.13f;
				break;
			case 4:
				stats[0].crit.chance = 0.2f;
				stats2[0].crit.chance = 0.2f;
				stats3[0].crit.chance = 0.2f;
				stats4[0].crit.chance = 0.2f;
				break;
			case 5:
				stats[0].crit.chance = 0.3f;
				stats2[0].crit.chance = 0.3f;
				stats3[0].crit.chance = 0.3f;
				stats4[0].crit.chance = 0.3f;
				break;
			}
			switch (GunJetStun)
			{
			case 0:
				stats[0].stun.chance = 0f;
				stats2[0].stun.chance = 0f;
				stats3[0].stun.chance = 0f;
				stats4[0].stun.chance = 0f;
				break;
			case 1:
				stats[0].stun.chance = 0.02f;
				stats2[0].stun.chance = 0.02f;
				stats3[0].stun.chance = 0.02f;
				stats4[0].stun.chance = 0.02f;
				break;
			case 2:
				stats[0].stun.chance = 0.04f;
				stats2[0].stun.chance = 0.04f;
				stats3[0].stun.chance = 0.04f;
				stats4[0].stun.chance = 0.04f;
				break;
			case 3:
				stats[0].stun.chance = 0.06f;
				stats2[0].stun.chance = 0.06f;
				stats3[0].stun.chance = 0.06f;
				stats4[0].stun.chance = 0.06f;
				break;
			case 4:
				stats[0].stun.chance = 0.08f;
				stats2[0].stun.chance = 0.08f;
				stats3[0].stun.chance = 0.08f;
				stats4[0].stun.chance = 0.08f;
				break;
			case 5:
				stats[0].stun.chance = 0.1f;
				stats2[0].stun.chance = 0.1f;
				stats3[0].stun.chance = 0.1f;
				stats4[0].stun.chance = 0.1f;
				break;
			}
			switch (SlowRange)
			{
			case 0:
				stats5[0].range = 3.5f;
				stats6[0].range = 3.8f;
				stats7[0].range = 4.1f;
				stats8[0].range = 4.5f;
				break;
			case 1:
				stats5[0].range = 3.74500012f;
				stats6[0].range = 4.066f;
				stats7[0].range = 4.387f;
				stats8[0].range = 4.815f;
				break;
			case 2:
				stats5[0].range = 3.955f;
				stats6[0].range = 4.294f;
				stats7[0].range = 4.633f;
				stats8[0].range = 5.085f;
				break;
			case 3:
				stats5[0].range = 4.20000029f;
				stats6[0].range = 4.56f;
				stats7[0].range = 4.92f;
				stats8[0].range = 5.4f;
				break;
			case 4:
				stats5[0].range = 4.62000036f;
				stats6[0].range = 5.01600027f;
				stats7[0].range = 5.412f;
				stats8[0].range = 5.94f;
				break;
			case 5:
				stats5[0].range = 5.07500029f;
				stats6[0].range = 5.51f;
				stats7[0].range = 5.945f;
				stats8[0].range = 6.525f;
				break;
			}
			switch (SlowCooldown)
			{
			case 0:
				stats5[0].cooldown = 1f;
				stats6[0].cooldown = 1f;
				stats7[0].cooldown = 1f;
				stats8[0].cooldown = 1f;
				break;
			case 1:
				stats5[0].cooldown = 0.93f;
				stats6[0].cooldown = 0.93f;
				stats7[0].cooldown = 0.93f;
				stats8[0].cooldown = 0.93f;
				break;
			case 2:
				stats5[0].cooldown = 0.86f;
				stats6[0].cooldown = 0.86f;
				stats7[0].cooldown = 0.86f;
				stats8[0].cooldown = 0.86f;
				break;
			case 3:
				stats5[0].cooldown = 0.75f;
				stats6[0].cooldown = 0.75f;
				stats7[0].cooldown = 0.75f;
				stats8[0].cooldown = 0.75f;
				break;
			case 4:
				stats5[0].cooldown = 0.66f;
				stats6[0].cooldown = 0.66f;
				stats7[0].cooldown = 0.66f;
				stats8[0].cooldown = 0.66f;
				break;
			case 5:
				stats5[0].cooldown = 0.55f;
				stats6[0].cooldown = 0.55f;
				stats7[0].cooldown = 0.55f;
				stats8[0].cooldown = 0.55f;
				break;
			}
			switch (SlowDamage)
			{
			case 0:
				stats5[0].slow.slowMultiplier = 0.7f;
				stats6[0].slow.slowMultiplier = 0.65f;
				stats7[0].slow.slowMultiplier = 0.6f;
				stats8[0].slow.slowMultiplier = 0.55f;
				break;
			case 1:
				stats5[0].slow.slowMultiplier = 0.664999962f;
				stats6[0].slow.slowMultiplier = 0.617499948f;
				stats7[0].slow.slowMultiplier = 0.57f;
				stats8[0].slow.slowMultiplier = 0.5225f;
				break;
			case 2:
				stats5[0].slow.slowMultiplier = 0.622999966f;
				stats6[0].slow.slowMultiplier = 0.5785f;
				stats7[0].slow.slowMultiplier = 0.534000039f;
				stats8[0].slow.slowMultiplier = 0.489500016f;
				break;
			case 3:
				stats5[0].slow.slowMultiplier = 0.581f;
				stats6[0].slow.slowMultiplier = 0.5395f;
				stats7[0].slow.slowMultiplier = 0.498f;
				stats8[0].slow.slowMultiplier = 0.4565f;
				break;
			case 4:
				stats5[0].slow.slowMultiplier = 0.532f;
				stats6[0].slow.slowMultiplier = 0.494f;
				stats7[0].slow.slowMultiplier = 0.456f;
				stats8[0].slow.slowMultiplier = 0.418f;
				break;
			case 5:
				stats5[0].slow.slowMultiplier = 0.48999998f;
				stats6[0].slow.slowMultiplier = 0.454999983f;
				stats7[0].slow.slowMultiplier = 0.420000017f;
				stats8[0].slow.slowMultiplier = 0.385f;
				break;
			}
			switch (SlowTime)
			{
			case 0:
				stats5[0].slow.duration = 1f;
				stats6[0].slow.duration = 1f;
				stats7[0].slow.duration = 1f;
				stats8[0].slow.duration = 1f;
				break;
			case 1:
				stats5[0].slow.duration = 1.1f;
				stats6[0].slow.duration = 1.1f;
				stats7[0].slow.duration = 1.1f;
				stats8[0].slow.duration = 1.1f;
				break;
			case 2:
				stats5[0].slow.duration = 1.2f;
				stats6[0].slow.duration = 1.2f;
				stats7[0].slow.duration = 1.2f;
				stats8[0].slow.duration = 1.2f;
				break;
			case 3:
				stats5[0].slow.duration = 1.32f;
				stats6[0].slow.duration = 1.32f;
				stats7[0].slow.duration = 1.32f;
				stats8[0].slow.duration = 1.32f;
				break;
			case 4:
				stats5[0].slow.duration = 1.43f;
				stats6[0].slow.duration = 1.43f;
				stats7[0].slow.duration = 1.43f;
				stats8[0].slow.duration = 1.43f;
				break;
			case 5:
				stats5[0].slow.duration = 1.55f;
				stats6[0].slow.duration = 1.55f;
				stats7[0].slow.duration = 1.55f;
				stats8[0].slow.duration = 1.55f;
				break;
			}
			switch (CannonRange)
			{
			case 0:
				stats9[0].range = 4f;
				stats10[0].range = 4.4f;
				stats11[0].range = 4.8f;
				stats12[0].range = 5.2f;
				break;
			case 1:
				stats9[0].range = 4.28f;
				stats10[0].range = 4.708f;
				stats11[0].range = 5.13600063f;
				stats12[0].range = 5.564f;
				break;
			case 2:
				stats9[0].range = 4.52f;
				stats10[0].range = 4.972f;
				stats11[0].range = 5.42400026f;
				stats12[0].range = 5.876f;
				break;
			case 3:
				stats9[0].range = 4.8f;
				stats10[0].range = 5.28f;
				stats11[0].range = 5.76f;
				stats12[0].range = 6.24000025f;
				break;
			case 4:
				stats9[0].range = 5.28f;
				stats10[0].range = 5.80800056f;
				stats11[0].range = 6.33600044f;
				stats12[0].range = 6.864f;
				break;
			case 5:
				stats9[0].range = 5.8f;
				stats10[0].range = 6.38f;
				stats11[0].range = 6.96000051f;
				stats12[0].range = 7.54f;
				break;
			}
			switch (CannonDamage)
			{
			case 0:
				stats9[0].damageMax = 4f;
				stats9[0].damageMin = 3f;
				stats10[0].damageMax = 8f;
				stats10[0].damageMin = 6f;
				stats11[0].damageMax = 15f;
				stats11[0].damageMin = 11f;
				stats12[0].damageMax = 26f;
				stats12[0].damageMin = 20f;
				break;
			case 1:
				stats9[0].damageMax = 4.4f;
				stats9[0].damageMin = 3.30000019f;
				stats10[0].damageMax = 8.8f;
				stats10[0].damageMin = 6.60000038f;
				stats11[0].damageMax = 16.5f;
				stats11[0].damageMin = 12.1f;
				stats12[0].damageMax = 28.6f;
				stats12[0].damageMin = 22f;
				break;
			case 2:
				stats9[0].damageMax = 5f;
				stats9[0].damageMin = 3.75f;
				stats10[0].damageMax = 10f;
				stats10[0].damageMin = 7.5f;
				stats11[0].damageMax = 18.75f;
				stats11[0].damageMin = 13.75f;
				stats12[0].damageMax = 32.5f;
				stats12[0].damageMin = 25f;
				break;
			case 3:
				stats9[0].damageMax = 6f;
				stats9[0].damageMin = 4.5f;
				stats10[0].damageMax = 12f;
				stats10[0].damageMin = 9f;
				stats11[0].damageMax = 22.5f;
				stats11[0].damageMin = 16.5f;
				stats12[0].damageMax = 39f;
				stats12[0].damageMin = 30f;
				break;
			case 4:
				stats9[0].damageMax = 7.4f;
				stats9[0].damageMin = 5.55f;
				stats10[0].damageMax = 14.8f;
				stats10[0].damageMin = 11.1f;
				stats11[0].damageMax = 27.75f;
				stats11[0].damageMin = 20.35f;
				stats12[0].damageMax = 48.1000023f;
				stats12[0].damageMin = 37f;
				break;
			case 5:
				stats9[0].damageMax = 8.8f;
				stats9[0].damageMin = 6.60000038f;
				stats10[0].damageMax = 17.6f;
				stats10[0].damageMin = 13.2000008f;
				stats11[0].damageMax = 33f;
				stats11[0].damageMin = 24.2f;
				stats12[0].damageMax = 57.2f;
				stats12[0].damageMin = 44f;
				break;
			}
			switch (CannonCooldown)
			{
			case 0:
				stats9[0].cooldown = 1f;
				stats10[0].cooldown = 1f;
				stats11[0].cooldown = 1f;
				stats12[0].cooldown = 1f;
				break;
			case 1:
				stats9[0].cooldown = 0.93f;
				stats10[0].cooldown = 0.93f;
				stats11[0].cooldown = 0.93f;
				stats12[0].cooldown = 0.93f;
				break;
			case 2:
				stats9[0].cooldown = 0.86f;
				stats10[0].cooldown = 0.86f;
				stats11[0].cooldown = 0.86f;
				stats12[0].cooldown = 0.86f;
				break;
			case 3:
				stats9[0].cooldown = 0.75f;
				stats10[0].cooldown = 0.75f;
				stats11[0].cooldown = 0.75f;
				stats12[0].cooldown = 0.75f;
				break;
			case 4:
				stats9[0].cooldown = 0.66f;
				stats10[0].cooldown = 0.66f;
				stats11[0].cooldown = 0.66f;
				stats12[0].cooldown = 0.66f;
				break;
			case 5:
				stats9[0].cooldown = 0.55f;
				stats10[0].cooldown = 0.55f;
				stats11[0].cooldown = 0.55f;
				stats12[0].cooldown = 0.55f;
				break;
			}
			switch (CannonCrift)
			{
			case 0:
				stats9[0].crit.chance = 0f;
				stats10[0].crit.chance = 0f;
				stats11[0].crit.chance = 0f;
				stats12[0].crit.chance = 0f;
				break;
			case 1:
				stats9[0].crit.chance = 0.03f;
				stats10[0].crit.chance = 0.03f;
				stats11[0].crit.chance = 0.03f;
				stats12[0].crit.chance = 0.03f;
				break;
			case 2:
				stats9[0].crit.chance = 0.06f;
				stats10[0].crit.chance = 0.06f;
				stats11[0].crit.chance = 0.06f;
				stats12[0].crit.chance = 0.06f;
				break;
			case 3:
				stats9[0].crit.chance = 0.11f;
				stats10[0].crit.chance = 0.11f;
				stats11[0].crit.chance = 0.11f;
				stats12[0].crit.chance = 0.11f;
				break;
			case 4:
				stats9[0].crit.chance = 0.2f;
				stats10[0].crit.chance = 0.2f;
				stats11[0].crit.chance = 0.2f;
				stats12[0].crit.chance = 0.2f;
				break;
			case 5:
				stats9[0].crit.chance = 0.3f;
				stats10[0].crit.chance = 0.3f;
				stats11[0].crit.chance = 0.3f;
				stats12[0].crit.chance = 0.3f;
				break;
			}
			switch (CannonStun)
			{
			case 0:
				stats9[0].stun.chance = 0f;
				stats10[0].stun.chance = 0f;
				stats11[0].stun.chance = 0f;
				stats12[0].stun.chance = 0f;
				break;
			case 1:
				stats9[0].stun.chance = 0.03f;
				stats10[0].stun.chance = 0.03f;
				stats11[0].stun.chance = 0.03f;
				stats12[0].stun.chance = 0.03f;
				break;
			case 2:
				stats9[0].stun.chance = 0.06f;
				stats10[0].stun.chance = 0.06f;
				stats11[0].stun.chance = 0.06f;
				stats12[0].stun.chance = 0.06f;
				break;
			case 3:
				stats9[0].stun.chance = 0.11f;
				stats10[0].stun.chance = 0.11f;
				stats11[0].stun.chance = 0.11f;
				stats12[0].stun.chance = 0.11f;
				break;
			case 4:
				stats9[0].stun.chance = 0.2f;
				stats10[0].stun.chance = 0.2f;
				stats11[0].stun.chance = 0.2f;
				stats12[0].stun.chance = 0.2f;
				break;
			case 5:
				stats9[0].stun.chance = 0.3f;
				stats10[0].stun.chance = 0.3f;
				stats11[0].stun.chance = 0.3f;
				stats12[0].stun.chance = 0.3f;
				break;
			}
			switch (LaserRange)
			{
			case 0:
				stats13[0].range = 4f;
				stats14[0].range = 4.4f;
				stats15[0].range = 4.8f;
				stats16[0].range = 5.2f;
				break;
			case 1:
				stats13[0].range = 4.28f;
				stats14[0].range = 4.708f;
				stats15[0].range = 5.13600063f;
				stats16[0].range = 5.564f;
				break;
			case 2:
				stats13[0].range = 4.52f;
				stats14[0].range = 4.972f;
				stats15[0].range = 5.42400026f;
				stats16[0].range = 5.876f;
				break;
			case 3:
				stats13[0].range = 4.8f;
				stats14[0].range = 5.28f;
				stats15[0].range = 5.76f;
				stats16[0].range = 6.24000025f;
				break;
			case 4:
				stats13[0].range = 5.28f;
				stats14[0].range = 5.80800056f;
				stats15[0].range = 6.33600044f;
				stats16[0].range = 6.864f;
				break;
			case 5:
				stats13[0].range = 5.8f;
				stats14[0].range = 6.38f;
				stats15[0].range = 6.96000051f;
				stats16[0].range = 7.54f;
				break;
			}
			switch (LaserDamage)
			{
			case 0:
				stats13[0].damageMax = 8f;
				stats13[0].damageMin = 7f;
				stats14[0].damageMax = 16f;
				stats14[0].damageMin = 14f;
				stats15[0].damageMax = 32f;
				stats15[0].damageMin = 25f;
				stats16[0].damageMax = 60f;
				stats16[0].damageMin = 56f;
				break;
			case 1:
				stats13[0].damageMax = 8.8f;
				stats13[0].damageMin = 7.70000029f;
				stats14[0].damageMax = 17.6f;
				stats14[0].damageMin = 15.4000006f;
				stats15[0].damageMax = 35.2f;
				stats15[0].damageMin = 27.5f;
				stats16[0].damageMax = 66f;
				stats16[0].damageMin = 61.6000023f;
				break;
			case 2:
				stats13[0].damageMax = 10f;
				stats13[0].damageMin = 8.75f;
				stats14[0].damageMax = 20f;
				stats14[0].damageMin = 17.5f;
				stats15[0].damageMax = 40f;
				stats15[0].damageMin = 31.25f;
				stats16[0].damageMax = 75f;
				stats16[0].damageMin = 70f;
				break;
			case 3:
				stats13[0].damageMax = 12f;
				stats13[0].damageMin = 10.5f;
				stats14[0].damageMax = 24f;
				stats14[0].damageMin = 21f;
				stats15[0].damageMax = 48f;
				stats15[0].damageMin = 37.5f;
				stats16[0].damageMax = 90f;
				stats16[0].damageMin = 84f;
				break;
			case 4:
				stats13[0].damageMax = 14.8f;
				stats13[0].damageMin = 12.95f;
				stats14[0].damageMax = 29.6f;
				stats14[0].damageMin = 25.9f;
				stats15[0].damageMax = 59.2f;
				stats15[0].damageMin = 46.25f;
				stats16[0].damageMax = 111f;
				stats16[0].damageMin = 103.6f;
				break;
			case 5:
				stats13[0].damageMax = 17.6f;
				stats13[0].damageMin = 15.4000006f;
				stats14[0].damageMax = 35.2f;
				stats14[0].damageMin = 30.8000011f;
				stats15[0].damageMax = 70.4f;
				stats15[0].damageMin = 55f;
				stats16[0].damageMax = 132f;
				stats16[0].damageMin = 123.200005f;
				break;
			}
			switch (LaserCooldown)
			{
			case 0:
				stats13[0].cooldown = 1f;
				stats14[0].cooldown = 1f;
				stats15[0].cooldown = 1f;
				stats16[0].cooldown = 1f;
				break;
			case 1:
				stats13[0].cooldown = 0.93f;
				stats14[0].cooldown = 0.93f;
				stats15[0].cooldown = 0.93f;
				stats16[0].cooldown = 0.93f;
				break;
			case 2:
				stats13[0].cooldown = 0.86f;
				stats14[0].cooldown = 0.86f;
				stats15[0].cooldown = 0.86f;
				stats16[0].cooldown = 0.86f;
				break;
			case 3:
				stats13[0].cooldown = 0.75f;
				stats14[0].cooldown = 0.75f;
				stats15[0].cooldown = 0.75f;
				stats16[0].cooldown = 0.75f;
				break;
			case 4:
				stats13[0].cooldown = 0.66f;
				stats14[0].cooldown = 0.66f;
				stats15[0].cooldown = 0.66f;
				stats16[0].cooldown = 0.66f;
				break;
			case 5:
				stats13[0].cooldown = 0.55f;
				stats14[0].cooldown = 0.55f;
				stats15[0].cooldown = 0.55f;
				stats16[0].cooldown = 0.55f;
				break;
			}
			switch (LaserCrift)
			{
			case 0:
				stats13[0].crit.chance = 0f;
				stats14[0].crit.chance = 0f;
				stats15[0].crit.chance = 0f;
				stats16[0].crit.chance = 0f;
				break;
			case 1:
				stats13[0].crit.chance = 0.03f;
				stats14[0].crit.chance = 0.03f;
				stats15[0].crit.chance = 0.03f;
				stats16[0].crit.chance = 0.03f;
				break;
			case 2:
				stats13[0].crit.chance = 0.06f;
				stats14[0].crit.chance = 0.06f;
				stats15[0].crit.chance = 0.06f;
				stats16[0].crit.chance = 0.06f;
				break;
			case 3:
				stats13[0].crit.chance = 0.11f;
				stats14[0].crit.chance = 0.11f;
				stats15[0].crit.chance = 0.11f;
				stats16[0].crit.chance = 0.11f;
				break;
			case 4:
				stats13[0].crit.chance = 0.2f;
				stats14[0].crit.chance = 0.2f;
				stats15[0].crit.chance = 0.2f;
				stats16[0].crit.chance = 0.2f;
				break;
			case 5:
				stats13[0].crit.chance = 0.3f;
				stats14[0].crit.chance = 0.3f;
				stats15[0].crit.chance = 0.3f;
				stats16[0].crit.chance = 0.3f;
				break;
			}
			switch (LaserStun)
			{
			case 0:
				stats13[0].stun.chance = 0f;
				stats14[0].stun.chance = 0f;
				stats15[0].stun.chance = 0f;
				stats16[0].stun.chance = 0f;
				break;
			case 1:
				stats13[0].stun.chance = 0.03f;
				stats14[0].stun.chance = 0.03f;
				stats15[0].stun.chance = 0.03f;
				stats16[0].stun.chance = 0.03f;
				break;
			case 2:
				stats13[0].stun.chance = 0.06f;
				stats14[0].stun.chance = 0.06f;
				stats15[0].stun.chance = 0.06f;
				stats16[0].stun.chance = 0.06f;
				break;
			case 3:
				stats13[0].stun.chance = 0.11f;
				stats14[0].stun.chance = 0.11f;
				stats15[0].stun.chance = 0.11f;
				stats16[0].stun.chance = 0.11f;
				break;
			case 4:
				stats13[0].stun.chance = 0.2f;
				stats14[0].stun.chance = 0.2f;
				stats15[0].stun.chance = 0.2f;
				stats16[0].stun.chance = 0.2f;
				break;
			case 5:
				stats13[0].stun.chance = 0.3f;
				stats14[0].stun.chance = 0.3f;
				stats15[0].stun.chance = 0.3f;
				stats16[0].stun.chance = 0.3f;
				break;
			}
			switch (BombRange)
			{
			case 0:
				stats17[0].range = 4.4f;
				stats18[0].range = 4.8f;
				stats19[0].range = 5.2f;
				stats20[0].range = 5.5f;
				break;
			case 1:
				stats17[0].range = 4.708f;
				stats18[0].range = 5.13600063f;
				stats19[0].range = 5.564f;
				stats20[0].range = 5.885f;
				break;
			case 2:
				stats17[0].range = 4.972f;
				stats18[0].range = 5.42400026f;
				stats19[0].range = 5.876f;
				stats20[0].range = 6.215f;
				break;
			case 3:
				stats17[0].range = 5.28f;
				stats18[0].range = 5.76f;
				stats19[0].range = 6.24000025f;
				stats20[0].range = 6.60000038f;
				break;
			case 4:
				stats17[0].range = 5.80800056f;
				stats18[0].range = 6.33600044f;
				stats19[0].range = 6.864f;
				stats20[0].range = 7.26f;
				break;
			case 5:
				stats17[0].range = 6.38f;
				stats18[0].range = 6.96000051f;
				stats19[0].range = 7.54f;
				stats20[0].range = 7.97500038f;
				break;
			}
			switch (BombDamage)
			{
			case 0:
				stats17[0].damageMax = 9f;
				stats17[0].damageMin = 7f;
				stats18[0].damageMax = 18f;
				stats18[0].damageMin = 15f;
				stats19[0].damageMax = 35f;
				stats19[0].damageMin = 30f;
				stats20[0].damageMax = 64f;
				stats20[0].damageMin = 58f;
				break;
			case 1:
				stats17[0].damageMax = 9.900001f;
				stats17[0].damageMin = 7.70000029f;
				stats18[0].damageMax = 19.8000011f;
				stats18[0].damageMin = 16.5f;
				stats19[0].damageMax = 38.5f;
				stats19[0].damageMin = 33f;
				stats20[0].damageMax = 70.4f;
				stats20[0].damageMin = 63.8000031f;
				break;
			case 2:
				stats17[0].damageMax = 11.25f;
				stats17[0].damageMin = 8.75f;
				stats18[0].damageMax = 22.5f;
				stats18[0].damageMin = 18.75f;
				stats19[0].damageMax = 43.75f;
				stats19[0].damageMin = 37.5f;
				stats20[0].damageMax = 80f;
				stats20[0].damageMin = 72.5f;
				break;
			case 3:
				stats17[0].damageMax = 13.5f;
				stats17[0].damageMin = 10.5f;
				stats18[0].damageMax = 27f;
				stats18[0].damageMin = 22.5f;
				stats19[0].damageMax = 52.5f;
				stats19[0].damageMin = 45f;
				stats20[0].damageMax = 96f;
				stats20[0].damageMin = 87f;
				break;
			case 4:
				stats17[0].damageMax = 16.65f;
				stats17[0].damageMin = 12.95f;
				stats18[0].damageMax = 33.3f;
				stats18[0].damageMin = 27.75f;
				stats19[0].damageMax = 64.75f;
				stats19[0].damageMin = 55.5f;
				stats20[0].damageMax = 118.4f;
				stats20[0].damageMin = 107.3f;
				break;
			case 5:
				stats17[0].damageMax = 19.8000011f;
				stats17[0].damageMin = 15.4000006f;
				stats18[0].damageMax = 39.6000023f;
				stats18[0].damageMin = 33f;
				stats19[0].damageMax = 77f;
				stats19[0].damageMin = 66f;
				stats20[0].damageMax = 140.8f;
				stats20[0].damageMin = 127.600006f;
				break;
			}
			switch (BombCooldown)
			{
			case 0:
				stats17[0].cooldown = 1f;
				stats18[0].cooldown = 1f;
				stats19[0].cooldown = 1f;
				stats20[0].cooldown = 1f;
				break;
			case 1:
				stats17[0].cooldown = 0.93f;
				stats18[0].cooldown = 0.93f;
				stats19[0].cooldown = 0.93f;
				stats20[0].cooldown = 0.93f;
				break;
			case 2:
				stats17[0].cooldown = 0.86f;
				stats18[0].cooldown = 0.86f;
				stats19[0].cooldown = 0.86f;
				stats20[0].cooldown = 0.86f;
				break;
			case 3:
				stats17[0].cooldown = 0.75f;
				stats18[0].cooldown = 0.75f;
				stats19[0].cooldown = 0.75f;
				stats20[0].cooldown = 0.75f;
				break;
			case 4:
				stats17[0].cooldown = 0.66f;
				stats18[0].cooldown = 0.66f;
				stats19[0].cooldown = 0.66f;
				stats20[0].cooldown = 0.66f;
				break;
			case 5:
				stats17[0].cooldown = 0.55f;
				stats18[0].cooldown = 0.55f;
				stats19[0].cooldown = 0.55f;
				stats20[0].cooldown = 0.55f;
				break;
			}
			switch (BombCrift)
			{
			case 0:
				stats17[0].crit.chance = 0f;
				stats18[0].crit.chance = 0f;
				stats19[0].crit.chance = 0f;
				stats20[0].crit.chance = 0f;
				break;
			case 1:
				stats17[0].crit.chance = 0.03f;
				stats18[0].crit.chance = 0.03f;
				stats19[0].crit.chance = 0.03f;
				stats20[0].crit.chance = 0.03f;
				break;
			case 2:
				stats17[0].crit.chance = 0.06f;
				stats18[0].crit.chance = 0.06f;
				stats19[0].crit.chance = 0.06f;
				stats20[0].crit.chance = 0.06f;
				break;
			case 3:
				stats17[0].crit.chance = 0.11f;
				stats18[0].crit.chance = 0.11f;
				stats19[0].crit.chance = 0.11f;
				stats20[0].crit.chance = 0.11f;
				break;
			case 4:
				stats17[0].crit.chance = 0.2f;
				stats18[0].crit.chance = 0.2f;
				stats19[0].crit.chance = 0.2f;
				stats20[0].crit.chance = 0.2f;
				break;
			case 5:
				stats17[0].crit.chance = 0.3f;
				stats18[0].crit.chance = 0.3f;
				stats19[0].crit.chance = 0.3f;
				stats20[0].crit.chance = 0.3f;
				break;
			}
			switch (BombStun)
			{
			case 0:
				stats17[0].stun.chance = 0f;
				stats18[0].stun.chance = 0f;
				stats19[0].stun.chance = 0f;
				stats20[0].stun.chance = 0f;
				break;
			case 1:
				stats17[0].stun.chance = 0.03f;
				stats18[0].stun.chance = 0.03f;
				stats19[0].stun.chance = 0.03f;
				stats20[0].stun.chance = 0.03f;
				break;
			case 2:
				stats17[0].stun.chance = 0.06f;
				stats18[0].stun.chance = 0.06f;
				stats19[0].stun.chance = 0.06f;
				stats20[0].stun.chance = 0.06f;
				break;
			case 3:
				stats17[0].stun.chance = 0.11f;
				stats18[0].stun.chance = 0.11f;
				stats19[0].stun.chance = 0.11f;
				stats20[0].stun.chance = 0.11f;
				break;
			case 4:
				stats17[0].stun.chance = 0.2f;
				stats18[0].stun.chance = 0.2f;
				stats19[0].stun.chance = 0.2f;
				stats20[0].stun.chance = 0.2f;
				break;
			case 5:
				stats17[0].stun.chance = 0.3f;
				stats18[0].stun.chance = 0.3f;
				stats19[0].stun.chance = 0.3f;
				stats20[0].stun.chance = 0.3f;
				break;
			}
			switch (SunrayRange)
			{
			case 0:
				stats21[0].range = 2.5f;
				stats22[0].range = 2.8f;
				stats23[0].range = 3.1f;
				stats24[0].range = 3.5f;
				break;
			case 1:
				stats21[0].range = 2.67500019f;
				stats22[0].range = 2.996f;
				stats23[0].range = 3.31700015f;
				stats24[0].range = 3.74500012f;
				break;
			case 2:
				stats21[0].range = 2.825f;
				stats22[0].range = 3.164f;
				stats23[0].range = 3.50299978f;
				stats24[0].range = 3.955f;
				break;
			case 3:
				stats21[0].range = 3f;
				stats22[0].range = 3.36000013f;
				stats23[0].range = 3.72f;
				stats24[0].range = 4.20000029f;
				break;
			case 4:
				stats21[0].range = 3.30000019f;
				stats22[0].range = 3.696f;
				stats23[0].range = 4.092f;
				stats24[0].range = 4.62000036f;
				break;
			case 5:
				stats21[0].range = 3.625f;
				stats22[0].range = 4.06f;
				stats23[0].range = 4.495f;
				stats24[0].range = 5.07500029f;
				break;
			}
			switch (SunrayDamage)
			{
			case 0:
				stats21[0].damageMax = 18f;
				stats21[0].damageMin = 16f;
				stats22[0].damageMax = 42f;
				stats22[0].damageMin = 32f;
				stats23[0].damageMax = 74f;
				stats23[0].damageMin = 62f;
				stats24[0].damageMax = 140f;
				stats24[0].damageMin = 130f;
				break;
			case 1:
				stats21[0].damageMax = 19.8000011f;
				stats21[0].damageMin = 17.6f;
				stats22[0].damageMax = 46.2f;
				stats22[0].damageMin = 35.2f;
				stats23[0].damageMax = 81.4f;
				stats23[0].damageMin = 68.2000046f;
				stats24[0].damageMax = 154f;
				stats24[0].damageMin = 143f;
				break;
			case 2:
				stats21[0].damageMax = 22.5f;
				stats21[0].damageMin = 20f;
				stats22[0].damageMax = 52.5f;
				stats22[0].damageMin = 40f;
				stats23[0].damageMax = 92.5f;
				stats23[0].damageMin = 77.5f;
				stats24[0].damageMax = 175f;
				stats24[0].damageMin = 162.5f;
				break;
			case 3:
				stats21[0].damageMax = 27f;
				stats21[0].damageMin = 24f;
				stats22[0].damageMax = 63f;
				stats22[0].damageMin = 48f;
				stats23[0].damageMax = 111f;
				stats23[0].damageMin = 93f;
				stats24[0].damageMax = 210f;
				stats24[0].damageMin = 195f;
				break;
			case 4:
				stats21[0].damageMax = 33.3f;
				stats21[0].damageMin = 29.6f;
				stats22[0].damageMax = 77.7000046f;
				stats22[0].damageMin = 59.2f;
				stats23[0].damageMax = 136.900009f;
				stats23[0].damageMin = 114.700005f;
				stats24[0].damageMax = 259f;
				stats24[0].damageMin = 240.5f;
				break;
			case 5:
				stats21[0].damageMax = 39.6000023f;
				stats21[0].damageMin = 35.2f;
				stats22[0].damageMax = 92.4f;
				stats22[0].damageMin = 70.4f;
				stats23[0].damageMax = 162.8f;
				stats23[0].damageMin = 136.400009f;
				stats24[0].damageMax = 308f;
				stats24[0].damageMin = 286f;
				break;
			}
			switch (SunrayCooldown)
			{
			case 0:
				stats21[0].cooldown = 2f;
				stats22[0].cooldown = 2f;
				stats23[0].cooldown = 2f;
				stats24[0].cooldown = 2f;
				break;
			case 1:
				stats21[0].cooldown = 1.86f;
				stats22[0].cooldown = 1.86f;
				stats23[0].cooldown = 1.86f;
				stats24[0].cooldown = 1.86f;
				break;
			case 2:
				stats21[0].cooldown = 1.72f;
				stats22[0].cooldown = 1.72f;
				stats23[0].cooldown = 1.72f;
				stats24[0].cooldown = 1.72f;
				break;
			case 3:
				stats21[0].cooldown = 1.5f;
				stats22[0].cooldown = 1.5f;
				stats23[0].cooldown = 1.5f;
				stats24[0].cooldown = 1.5f;
				break;
			case 4:
				stats21[0].cooldown = 1.32f;
				stats22[0].cooldown = 1.32f;
				stats23[0].cooldown = 1.32f;
				stats24[0].cooldown = 1.32f;
				break;
			case 5:
				stats21[0].cooldown = 1.1f;
				stats22[0].cooldown = 1.1f;
				stats23[0].cooldown = 1.1f;
				stats24[0].cooldown = 1.1f;
				break;
			}
			switch (SunrayCrift)
			{
			case 0:
				stats21[0].crit.chance = 0f;
				stats22[0].crit.chance = 0f;
				stats23[0].crit.chance = 0f;
				stats24[0].crit.chance = 0f;
				break;
			case 1:
				stats21[0].crit.chance = 0.03f;
				stats22[0].crit.chance = 0.03f;
				stats23[0].crit.chance = 0.03f;
				stats24[0].crit.chance = 0.03f;
				break;
			case 2:
				stats21[0].crit.chance = 0.06f;
				stats22[0].crit.chance = 0.06f;
				stats23[0].crit.chance = 0.06f;
				stats24[0].crit.chance = 0.06f;
				break;
			case 3:
				stats21[0].crit.chance = 0.11f;
				stats22[0].crit.chance = 0.11f;
				stats23[0].crit.chance = 0.11f;
				stats24[0].crit.chance = 0.11f;
				break;
			case 4:
				stats21[0].crit.chance = 0.2f;
				stats22[0].crit.chance = 0.2f;
				stats23[0].crit.chance = 0.2f;
				stats24[0].crit.chance = 0.2f;
				break;
			case 5:
				stats21[0].crit.chance = 0.3f;
				stats22[0].crit.chance = 0.3f;
				stats23[0].crit.chance = 0.3f;
				stats24[0].crit.chance = 0.3f;
				break;
			}
			switch (SunrayStun)
			{
			case 0:
				stats21[0].stun.chance = 0f;
				stats22[0].stun.chance = 0f;
				stats23[0].stun.chance = 0f;
				stats24[0].stun.chance = 0f;
				break;
			case 1:
				stats21[0].stun.chance = 0.03f;
				stats22[0].stun.chance = 0.03f;
				stats23[0].stun.chance = 0.03f;
				stats24[0].stun.chance = 0.03f;
				break;
			case 2:
				stats21[0].stun.chance = 0.06f;
				stats22[0].stun.chance = 0.06f;
				stats23[0].stun.chance = 0.06f;
				stats24[0].stun.chance = 0.06f;
				break;
			case 3:
				stats21[0].stun.chance = 0.11f;
				stats22[0].stun.chance = 0.11f;
				stats23[0].stun.chance = 0.11f;
				stats24[0].stun.chance = 0.11f;
				break;
			case 4:
				stats21[0].stun.chance = 0.2f;
				stats22[0].stun.chance = 0.2f;
				stats23[0].stun.chance = 0.2f;
				stats24[0].stun.chance = 0.2f;
				break;
			case 5:
				stats21[0].stun.chance = 0.3f;
				stats22[0].stun.chance = 0.3f;
				stats23[0].stun.chance = 0.3f;
				stats24[0].stun.chance = 0.3f;
				break;
			}
			switch (BeamRange)
			{
			case 0:
				stats25[0].range = 4.8f;
				stats26[0].range = 5.2f;
				stats27[0].range = 5.6f;
				stats28[0].range = 6f;
				break;
			case 1:
				stats25[0].range = 5.13600063f;
				stats26[0].range = 5.564f;
				stats27[0].range = 5.992f;
				stats28[0].range = 6.42f;
				break;
			case 2:
				stats25[0].range = 5.42400026f;
				stats26[0].range = 5.876f;
				stats27[0].range = 6.328f;
				stats28[0].range = 6.77999973f;
				break;
			case 3:
				stats25[0].range = 5.76f;
				stats26[0].range = 6.24000025f;
				stats27[0].range = 6.72000027f;
				stats28[0].range = 7.20000029f;
				break;
			case 4:
				stats25[0].range = 6.33600044f;
				stats26[0].range = 6.864f;
				stats27[0].range = 7.392f;
				stats28[0].range = 7.92f;
				break;
			case 5:
				stats25[0].range = 6.96000051f;
				stats26[0].range = 7.54f;
				stats27[0].range = 8.12f;
				stats28[0].range = 8.700001f;
				break;
			}
			switch (BeamDamage)
			{
			case 0:
				stats25[0].damageMax = 21f;
				stats25[0].damageMin = 19f;
				stats26[0].damageMax = 44f;
				stats26[0].damageMin = 38f;
				stats27[0].damageMax = 88f;
				stats27[0].damageMin = 82f;
				stats28[0].damageMax = 171f;
				stats28[0].damageMin = 165f;
				break;
			case 1:
				stats25[0].damageMax = 23.1f;
				stats25[0].damageMin = 20.9f;
				stats26[0].damageMax = 48.4f;
				stats26[0].damageMin = 41.8f;
				stats27[0].damageMax = 96.8f;
				stats27[0].damageMin = 90.2000046f;
				stats28[0].damageMax = 188.1f;
				stats28[0].damageMin = 181.5f;
				break;
			case 2:
				stats25[0].damageMax = 26.25f;
				stats25[0].damageMin = 23.75f;
				stats26[0].damageMax = 55f;
				stats26[0].damageMin = 47.5f;
				stats27[0].damageMax = 110f;
				stats27[0].damageMin = 102.5f;
				stats28[0].damageMax = 213.75f;
				stats28[0].damageMin = 206.25f;
				break;
			case 3:
				stats25[0].damageMax = 31.5f;
				stats25[0].damageMin = 28.5f;
				stats26[0].damageMax = 66f;
				stats26[0].damageMin = 57f;
				stats27[0].damageMax = 132f;
				stats27[0].damageMin = 123f;
				stats28[0].damageMax = 256.5f;
				stats28[0].damageMin = 247.5f;
				break;
			case 4:
				stats25[0].damageMax = 38.8500023f;
				stats25[0].damageMin = 35.15f;
				stats26[0].damageMax = 81.4f;
				stats26[0].damageMin = 70.3f;
				stats27[0].damageMax = 162.8f;
				stats27[0].damageMin = 151.7f;
				stats28[0].damageMax = 316.35f;
				stats28[0].damageMin = 305.25f;
				break;
			case 5:
				stats25[0].damageMax = 46.2f;
				stats25[0].damageMin = 41.8f;
				stats26[0].damageMax = 96.8f;
				stats26[0].damageMin = 83.6f;
				stats27[0].damageMax = 193.6f;
				stats27[0].damageMin = 180.400009f;
				stats28[0].damageMax = 376.2f;
				stats28[0].damageMin = 363f;
				break;
			}
			switch (BeamCooldown)
			{
			case 0:
				stats25[0].cooldown = 1f;
				stats26[0].cooldown = 1f;
				stats27[0].cooldown = 1f;
				stats28[0].cooldown = 1f;
				break;
			case 1:
				stats25[0].cooldown = 0.93f;
				stats26[0].cooldown = 0.93f;
				stats27[0].cooldown = 0.93f;
				stats28[0].cooldown = 0.93f;
				break;
			case 2:
				stats25[0].cooldown = 0.86f;
				stats26[0].cooldown = 0.86f;
				stats27[0].cooldown = 0.86f;
				stats28[0].cooldown = 0.86f;
				break;
			case 3:
				stats25[0].cooldown = 0.75f;
				stats26[0].cooldown = 0.75f;
				stats27[0].cooldown = 0.75f;
				stats28[0].cooldown = 0.75f;
				break;
			case 4:
				stats25[0].cooldown = 0.66f;
				stats26[0].cooldown = 0.66f;
				stats27[0].cooldown = 0.66f;
				stats28[0].cooldown = 0.66f;
				break;
			case 5:
				stats25[0].cooldown = 0.55f;
				stats26[0].cooldown = 0.55f;
				stats27[0].cooldown = 0.55f;
				stats28[0].cooldown = 0.55f;
				break;
			}
			switch (BeamCrift)
			{
			case 0:
				stats25[0].crit.chance = 0f;
				stats26[0].crit.chance = 0f;
				stats27[0].crit.chance = 0f;
				stats28[0].crit.chance = 0f;
				break;
			case 1:
				stats25[0].crit.chance = 0.03f;
				stats26[0].crit.chance = 0.03f;
				stats27[0].crit.chance = 0.03f;
				stats28[0].crit.chance = 0.03f;
				break;
			case 2:
				stats25[0].crit.chance = 0.06f;
				stats26[0].crit.chance = 0.06f;
				stats27[0].crit.chance = 0.06f;
				stats28[0].crit.chance = 0.06f;
				break;
			case 3:
				stats25[0].crit.chance = 0.11f;
				stats26[0].crit.chance = 0.11f;
				stats27[0].crit.chance = 0.11f;
				stats28[0].crit.chance = 0.11f;
				break;
			case 4:
				stats25[0].crit.chance = 0.2f;
				stats26[0].crit.chance = 0.2f;
				stats27[0].crit.chance = 0.2f;
				stats28[0].crit.chance = 0.2f;
				break;
			case 5:
				stats25[0].crit.chance = 0.3f;
				stats26[0].crit.chance = 0.3f;
				stats27[0].crit.chance = 0.3f;
				stats28[0].crit.chance = 0.3f;
				break;
			}
			switch (BeamStun)
			{
			case 0:
				stats25[0].stun.chance = 0f;
				stats26[0].stun.chance = 0f;
				stats27[0].stun.chance = 0f;
				stats28[0].stun.chance = 0f;
				break;
			case 1:
				stats25[0].stun.chance = 0.03f;
				stats26[0].stun.chance = 0.03f;
				stats27[0].stun.chance = 0.03f;
				stats28[0].stun.chance = 0.03f;
				break;
			case 2:
				stats25[0].stun.chance = 0.06f;
				stats26[0].stun.chance = 0.06f;
				stats27[0].stun.chance = 0.06f;
				stats28[0].stun.chance = 0.06f;
				break;
			case 3:
				stats25[0].stun.chance = 0.11f;
				stats26[0].stun.chance = 0.11f;
				stats27[0].stun.chance = 0.11f;
				stats28[0].stun.chance = 0.11f;
				break;
			case 4:
				stats25[0].stun.chance = 0.2f;
				stats26[0].stun.chance = 0.2f;
				stats27[0].stun.chance = 0.2f;
				stats28[0].stun.chance = 0.2f;
				break;
			case 5:
				stats25[0].stun.chance = 0.3f;
				stats26[0].stun.chance = 0.3f;
				stats27[0].stun.chance = 0.3f;
				stats28[0].stun.chance = 0.3f;
				break;
			}
		}

		private void Update()
		{
			if (BuildManager.UseDragNDrop() || !UIMainControl.UsePieMenu() || buildInfo == null)
			{
				return;
			}
			Vector3 screenPos = Camera.main.WorldToScreenPoint(buildInfo.position) * UIMainControl.GetScaleFactor();
			List<Vector3> pieMenuPos = GetPieMenuPos(activeButtonList.Count, screenPos, 120f, 50);
			for (int i = 0; i < activeButtonList.Count; i++)
			{
				if (i < pieMenuPos.Count)
				{
					activeButtonList[i].rectT.localPosition = pieMenuPos[i];
				}
				else
				{
					activeButtonList[i].rectT.localPosition = new Vector3(0f, 9999f, 0f);
				}
			}
		}

		public void OnTowerButton(GameObject butObj, int pointerID = -1)
		{
			int buttonID = GetButtonID(butObj);
			if (UIMainControl.InTouchMode() && !BuildManager.UseDragNDrop())
			{
				if (currentButtonID >= 0)
				{
					buttonList[currentButtonID].imgHighlight.enabled = false;
				}
				if (currentButtonID != buttonID)
				{
					currentButtonID = buttonID;
					buttonList[buttonID].imgHighlight.enabled = true;
					OnHoverButton(butObj);
					return;
				}
				ClearTouchModeButton();
			}
			string text = BuildManager.BuildTower(buttonID, buildInfo, pointerID);
			if (text != string.Empty)
			{
				UIMessage.DisplayMessage(text);
				return;
			}
			buildInfo = null;
			if (!BuildManager.UseDragNDrop())
			{
				Hide();
			}
		}

		public void ClearTouchModeButton()
		{
			if (currentButtonID >= 0)
			{
				buttonList[currentButtonID].imgHighlight.enabled = false;
			}
			currentButtonID = -1;
			OnExitButton(null);
		}

		public void OnHoverButton(GameObject butObj)
		{
			ShowTooltip(BuildManager.GetSampleTower(GetButtonID(butObj)));
			if (!BuildManager.UseDragNDrop() && buildInfo != null)
			{
				BuildManager.ShowSampleTower(GetButtonID(butObj), buildInfo);
			}
		}

		public void OnExitButton(GameObject butObj)
		{
			HideTooltip();
			if (!BuildManager.UseDragNDrop())
			{
				BuildManager.ClearSampleTower();
			}
		}

		private int GetButtonID(GameObject butObj)
		{
			for (int i = 0; i < buttonList.Count; i++)
			{
				if (buttonList[i].rootObj == butObj)
				{
					return i;
				}
			}
			return 0;
		}

		public void OnCancelBuildButton()
		{
			BuildManager.ExitDragNDrop();
		}

		private void ShowTooltip(UnitTower tower)
		{
			lbTooltipName.text = tower.unitName;
			lbTooltipDesp.text = tower.GetDespGeneral();
			List<int> cost = tower.GetCost();
			for (int i = 0; i < tooltipRscItemList.Count; i++)
			{
				tooltipRscItemList[i].label.text = cost[i].ToString();
			}
			tooltipCanvasG.alpha = 1f;
			tooltipObj.SetActive(value: true);
		}

		private void HideTooltip()
		{
			tooltipCanvasG.alpha = 0f;
			tooltipObj.SetActive(value: false);
		}

		private void UpdateActiveBuildButtonList()
		{
			activeButtonList = new List<UIButton>();
			if (buildInfo == null)
			{
				return;
			}
			for (int i = 0; i < buildInfo.availableTowerIDList.Count; i++)
			{
				activeButtonList.Add(buttonList[buildInfo.availableTowerIDList[i]]);
			}
			for (int j = 0; j < buttonList.Count; j++)
			{
				if (activeButtonList.Contains(buttonList[j]))
				{
					buttonList[j].rootObj.SetActive(value: true);
				}
				else
				{
					buttonList[j].rootObj.SetActive(value: false);
				}
			}
			if (activeButtonList.Count == 0)
			{
				Hide();
			}
		}

		public static void Show(BuildInfo bInfo = null)
		{
			instance._Show(bInfo);
		}

		public void _Show(BuildInfo bInfo = null)
		{
			if (!BuildManager.UseDragNDrop())
			{
				if (bInfo == null)
				{
					return;
				}
				ClearTouchModeButton();
				buildInfo = bInfo;
				UpdateActiveBuildButtonList();
			}
			rectT.localPosition = new Vector3(0f, 0f, 0f);
			UIMainControl.FadeIn(canvasGroup, 0.25f, thisObj);
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			if (UIMainControl.InTouchMode())
			{
				ClearTouchModeButton();
			}
			UIMainControl.FadeOut(canvasGroup);
			StartCoroutine(DelayHide());
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.25f);
			rectT.localPosition = new Vector3(0f, 99999f, 0f);
		}

		public List<Vector3> GetPieMenuPos(float num, Vector3 screenPos, float cutoff, int size)
		{
			List<Vector3> list = new List<Vector3>();
			if (num == 1f)
			{
				list.Add(screenPos * UIMainControl.GetScaleFactor() + new Vector3(0f, 50f, 0f));
				return list;
			}
			if (num <= 2f)
			{
				list.Add(screenPos * UIMainControl.GetScaleFactor() + new Vector3(50f, 10f, 0f));
				list.Add(screenPos * UIMainControl.GetScaleFactor() + new Vector3(-50f, 10f, 0f));
				return list;
			}
			if (piePosDummyT == null)
			{
				piePosDummyT = new GameObject().transform;
				piePosDummyT.parent = base.transform;
				piePosDummyT.name = "PiePosDummy";
			}
			int num2 = (cutoff > 0f) ? 1 : 0;
			float d = (360f - cutoff) / (num - (float)num2);
			float num3 = 0.35f * num * (float)size;
			piePosDummyT.rotation = Quaternion.Euler(0f, 0f, cutoff / 2f);
			piePosDummyT.position = screenPos;
			for (int i = 0; (float)i < num; i++)
			{
				list.Add(piePosDummyT.TransformPoint(new Vector3(0f, 0f - num3, 0f)));
				piePosDummyT.Rotate(Vector3.forward * d);
			}
			return list;
		}
	}
}
