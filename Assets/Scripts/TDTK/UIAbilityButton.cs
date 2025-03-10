using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIAbilityButton : MonoBehaviour
	{
		public Slider energyBar;

		public Text lbEnergy;

		public GameObject lbSelectTargetObj;

		[Header("Buttons")]
		public List<UIButton> buttonList = new List<UIButton>();

		public GameObject butCancelObj;

		private CanvasGroup butCancelCanvasG;

		private BuildInfo buildInfo;

		[Header("Tooltip")]
		public GameObject tooltipObj;

		private CanvasGroup tooltipCanvasG;

		public Text lbTooltipName;

		public Text lbTooltipDesp;

		public UIObject tooltipRscItem;

		private GameObject thisObj;

		private RectTransform rectT;

		private CanvasGroup canvasGroup;

		private static UIAbilityButton instance;

		private int AtomRange;

		private int AtomDamage;

		private int AtomCooldown;

		private int AtomStun;

		private int currentButtonID = -1;

		public static UIAbilityButton GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			if (ES2.Exists("AtomRange"))
			{
				AtomRange = ES2.Load<int>("AtomRange");
				AtomDamage = ES2.Load<int>("AtomDamage");
				AtomCooldown = ES2.Load<int>("AtomCooldown");
				AtomStun = ES2.Load<int>("AtomStun");
			}
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
			rectT.localPosition = new Vector3(0f, 0f, 0f);
		}

		private void Start()
		{
			if (!AbilityManager.IsOn() || AbilityManager.GetAbilityCount() == 0)
			{
				thisObj.SetActive(value: false);
				return;
			}
			AddDamageAb();
			List<Ability> abilityList = AbilityManager.GetAbilityList();
			for (int i = 0; i < abilityList.Count; i++)
			{
				if (i == 0)
				{
					buttonList[0].Init();
				}
				else if (i > 0)
				{
					buttonList.Add(UIButton.Clone(buttonList[0].rootObj, "AbilityButton" + (i + 1)));
				}
				buttonList[i].imgIcon.sprite = abilityList[i].icon;
				buttonList[i].imgHighlight.enabled = false;
				buttonList[i].label.text = string.Empty;
				if (abilityList[i].usedRemained > 0)
				{
					buttonList[i].label.text = abilityList[i].usedRemained.ToString();
				}
				if (UIMainControl.InTouchMode())
				{
					buttonList[i].SetCallback(null, null, OnAbilityButton);
				}
				else
				{
					buttonList[i].SetCallback(OnHoverButton, OnExitButton, OnAbilityButton);
				}
			}
			tooltipRscItem.Init();
			butCancelCanvasG = butCancelObj.AddComponent<CanvasGroup>();
			OnAbilitySelectingTarget(flag: false);
			UIItemCallback uIItemCallback = butCancelObj.AddComponent<UIItemCallback>();
			uIItemCallback.SetDownCallback(OnCancelAbilityButton);
			tooltipObj.SetActive(value: false);
		}

		private void AddDamageAb()
		{
			List<Ability> abilityList = AbilityManager.GetAbilityList();
			switch (AtomRange)
			{
			case 0:
				abilityList[0].aoeRadius = 4f;
				break;
			case 1:
				abilityList[0].aoeRadius = 4.4f;
				break;
			case 2:
				abilityList[0].aoeRadius = 4.92f;
				break;
			case 3:
				abilityList[0].aoeRadius = 5.4f;
				break;
			case 4:
				abilityList[0].aoeRadius = 6f;
				break;
			case 5:
				abilityList[0].aoeRadius = 6.6f;
				break;
			}
			switch (AtomDamage)
			{
			case 0:
				abilityList[0].effect.damageMax = 110f;
				abilityList[0].effect.damageMin = 90f;
				break;
			case 1:
				abilityList[0].effect.damageMax = 350f;
				abilityList[0].effect.damageMin = 300f;
				break;
			case 2:
				abilityList[0].effect.damageMax = 590f;
				abilityList[0].effect.damageMin = 540f;
				break;
			case 3:
				abilityList[0].effect.damageMax = 1000f;
				abilityList[0].effect.damageMin = 950f;
				break;
			case 4:
				abilityList[0].effect.damageMax = 1600f;
				abilityList[0].effect.damageMin = 1500f;
				break;
			case 5:
				abilityList[0].effect.damageMax = 2300f;
				abilityList[0].effect.damageMin = 2200f;
				break;
			case 6:
				abilityList[0].effect.damageMax = 3050f;
				abilityList[0].effect.damageMin = 2950f;
				break;
			case 7:
				abilityList[0].effect.damageMax = 3900f;
				abilityList[0].effect.damageMin = 3800f;
				break;
			case 8:
				abilityList[0].effect.damageMax = 4860f;
				abilityList[0].effect.damageMin = 4760f;
				break;
			case 9:
				abilityList[0].effect.damageMax = 5900f;
				abilityList[0].effect.damageMin = 5800f;
				break;
			case 10:
				abilityList[0].effect.damageMax = 7100f;
				abilityList[0].effect.damageMin = 7000f;
				break;
			}
			switch (AtomCooldown)
			{
			case 0:
				abilityList[0].cooldown = 120f;
				break;
			case 1:
				abilityList[0].cooldown = 108f;
				break;
			case 2:
				abilityList[0].cooldown = 96f;
				break;
			case 3:
				abilityList[0].cooldown = 78f;
				break;
			case 4:
				abilityList[0].cooldown = 60f;
				break;
			case 5:
				abilityList[0].cooldown = 42f;
				break;
			}
			switch (AtomStun)
			{
			case 0:
				abilityList[0].effect.stunChance = 0f;
				break;
			case 1:
				abilityList[0].effect.stunChance = 0.1f;
				break;
			case 2:
				abilityList[0].effect.stunChance = 0.25f;
				break;
			case 3:
				abilityList[0].effect.stunChance = 0.4f;
				break;
			case 4:
				abilityList[0].effect.stunChance = 0.55f;
				break;
			case 5:
				abilityList[0].effect.stunChance = 0.7f;
				break;
			}
		}

		private void OnEnable()
		{
			TDTK.onAbilityActivatedE += OnAbilityActivated;
			TDTK.onNewAbilityE += OnNewAbility;
			TDTK.onAbilitySelectingTargetE += OnAbilitySelectingTarget;
		}

		private void OnDisable()
		{
			TDTK.onAbilityActivatedE -= OnAbilityActivated;
			TDTK.onNewAbilityE -= OnNewAbility;
			TDTK.onAbilitySelectingTargetE -= OnAbilitySelectingTarget;
		}

		private void OnAbilitySelectingTarget(bool flag)
		{
			butCancelCanvasG.alpha = (flag ? 1 : 0);
			butCancelCanvasG.interactable = flag;
			if (lbSelectTargetObj != null)
			{
				lbSelectTargetObj.SetActive(flag);
			}
		}

		private void OnNewAbility(Ability ability)
		{
			buttonList.Add(UIButton.Clone(buttonList[0].rootObj, "AbilityButton" + (buttonList.Count + 1)));
			buttonList[buttonList.Count - 1].imgIcon.sprite = ability.icon;
			buttonList[buttonList.Count - 1].label.text = string.Empty;
			buttonList[buttonList.Count - 1].SetCallback(OnHoverButton, OnExitButton, OnAbilityButton);
			if (ability.usedRemained > 0)
			{
				buttonList[buttonList.Count - 1].label.text = ability.usedRemained.ToString();
			}
		}

		private void OnAbilityActivated(Ability ability)
		{
			UnityEngine.Debug.Log("DIS TAB");
			CameraControl.instance.enableTouchPan = true;
			StartCoroutine(ButtonCDRoutine(ability));
		}

		private IEnumerator ButtonCDRoutine(Ability ability)
		{
			int ID = AbilityManager.GetAbilityIndex(ability);
			if (ability.usedRemained >= 0)
			{
				buttonList[ID].label.text = ability.usedRemained.ToString();
			}
			buttonList[ID].button.interactable = false;
			if (ability.usedRemained == 0)
			{
				yield break;
			}
			while (true)
			{
				string empty = string.Empty;
				float duration = ability.currentCD;
				if (duration <= 0f)
				{
					break;
				}
				string text = (!(duration > 60f)) ? (Mathf.Ceil(duration).ToString("F0") + "s") : (Mathf.Floor(duration / 60f).ToString("F0") + "m");
				buttonList[ID].label.text = text;
				yield return new WaitForSeconds(0.05f);
			}
			buttonList[ID].label.text = string.Empty;
			buttonList[ID].button.interactable = true;
		}

		private void Update()
		{
			energyBar.value = AbilityManager.GetEnergy() / AbilityManager.GetEnergyFull();
			lbEnergy.text = AbilityManager.GetEnergy().ToString("f0") + "/" + AbilityManager.GetEnergyFull().ToString("f0");
		}

		public void OnAbilityButton(GameObject butObj, int pointerID = -1)
		{
			int buttonID = GetButtonID(butObj);
			CameraControl.instance.enableTouchPan = false;
			UnityEngine.Debug.Log("TAP");
			if (UIMainControl.InTouchMode())
			{
				if (currentButtonID >= 0)
				{
					buttonList[currentButtonID].imgHighlight.enabled = false;
				}
				if (currentButtonID != buttonID)
				{
					butCancelCanvasG.alpha = 1f;
					butCancelCanvasG.interactable = true;
					currentButtonID = buttonID;
					buttonList[buttonID].imgHighlight.enabled = true;
					OnHoverButton(butObj);
					return;
				}
				ClearTouchModeButton();
			}
			string text = AbilityManager.SelectAbility(buttonID, pointerID);
			if (text != string.Empty)
			{
				UIMessage.DisplayMessage(text);
			}
		}

		public void ClearTouchModeButton()
		{
			OnAbilitySelectingTarget(flag: false);
			if (currentButtonID >= 0)
			{
				buttonList[currentButtonID].imgHighlight.enabled = false;
			}
			currentButtonID = -1;
			OnExitButton(null);
		}

		public void OnHoverButton(GameObject butObj)
		{
			int buttonID = GetButtonID(butObj);
			ShowTooltip(AbilityManager.GetAbilityList()[buttonID]);
		}

		public void OnExitButton(GameObject butObj)
		{
			HideTooltip();
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

		public void OnCancelAbilityButton(GameObject butObj = null, int pointerID = -1)
		{
			AbilityManager.ExitSelectingTargetMode();
			UnityEngine.Debug.Log("DIS TAB33");
			CameraControl.instance.enableTouchPan = true;
			ClearTouchModeButton();
		}

		private void ShowTooltip(Ability ability)
		{
			lbTooltipName.text = ability.name;
			lbTooltipDesp.text = ability.GetDesp();
			tooltipRscItem.label.text = ability.GetCost().ToString("f0");
			tooltipCanvasG.alpha = 1f;
			tooltipObj.SetActive(value: true);
		}

		private void HideTooltip()
		{
			tooltipObj.SetActive(value: false);
		}

		public static void Show()
		{
			instance._Show();
		}

		public void _Show()
		{
			if (thisObj.activeInHierarchy)
			{
				rectT.localPosition = new Vector3(0f, 0f, 0f);
				UIMainControl.FadeIn(canvasGroup);
			}
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			if (thisObj.activeInHierarchy)
			{
				UIMainControl.FadeOut(canvasGroup);
				StartCoroutine(DelayHide());
			}
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.25f);
			rectT.localPosition = new Vector3(-5000f, -5000f, 0f);
		}
	}
}
