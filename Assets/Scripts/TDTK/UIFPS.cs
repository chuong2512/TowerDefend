using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIFPS : MonoBehaviour
	{
		public bool touchMode;

		public GameObject buttonGroupObj;

		public GameObject buttonFireObj;

		public UIButton buttonExit;

		[Space(10f)]
		public Text lbReloading;

		public UIObject fpsItem;

		public GameObject recticleObj;

		public RectTransform recticleSpreadRectT;

		private Vector2 recticleSpreadDefaultSize;

		public Image imgReloadProgress;

		private GameObject reloadProgressObj;

		private GameObject thisObj;

		private CanvasGroup canvasGroup;

		private static UIFPS instance;

		private float scrollCD;

		private bool reloading;

		private bool touchFiring;

		private bool isOn;

		public void Awake()
		{
			instance = this;
			thisObj = base.gameObject;
			canvasGroup = thisObj.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = thisObj.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			thisObj.SetActive(value: false);
			thisObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
			fpsItem.Init();
			recticleSpreadDefaultSize = recticleSpreadRectT.sizeDelta;
			reloadProgressObj = imgReloadProgress.transform.parent.gameObject;
			reloadProgressObj.SetActive(value: false);
			UIItemCallback uIItemCallback = buttonFireObj.AddComponent<UIItemCallback>();
			uIItemCallback.SetDownCallback(OnFireButtonDown);
			uIItemCallback.SetUpCallback(OnFireButtonUp);
			buttonExit.Init();
		}

		private void Start()
		{
			buttonGroupObj.SetActive(touchMode);
		}

		private void Update()
		{
			float recoilModifier = FPSControl.GetRecoilModifier();
			recoilModifier = Mathf.Min(recoilModifier * 20f, 250f);
			recticleSpreadRectT.sizeDelta = recticleSpreadDefaultSize + new Vector2(recoilModifier, recoilModifier) * 2f;
			if (reloading)
			{
				imgReloadProgress.fillAmount = FPSControl.GetReloadProgress();
			}
			if (!FPSControl.EnableInput())
			{
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.R))
			{
				FPSControl.Reload();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
			{
				FPSControl.SelectPrevWeapon();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.E))
			{
				FPSControl.SelectNextWeapon();
			}
			if (UnityEngine.Input.GetAxisRaw("Mouse ScrollWheel") != 0f && scrollCD <= 0f)
			{
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
				{
					FPSControl.SelectPrevWeapon();
				}
				else
				{
					FPSControl.SelectNextWeapon();
				}
				scrollCD = 0.15f;
			}
			scrollCD -= Time.deltaTime;
			if (!touchMode && (UnityEngine.Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)))
			{
				FPSControl.Fire();
			}
			if (touchMode && touchFiring)
			{
				FPSControl.Fire();
			}
			if (IsOn() && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				FPSControl.Hide();
			}
		}

		private void OnEnable()
		{
			TDTK.onFPSShootE += OnShoot;
			TDTK.onFPSReloadE += OnReload;
			TDTK.onFPSSwitchWeaponE += OnSwitchWeapon;
		}

		private void OnDisable()
		{
			TDTK.onFPSShootE -= OnShoot;
			TDTK.onFPSReloadE -= OnReload;
			TDTK.onFPSSwitchWeaponE -= OnSwitchWeapon;
		}

		private void OnShoot()
		{
			UpdateAmmoCount();
		}

		private void OnReload(bool flag)
		{
			reloading = flag;
			if (reloading)
			{
				StartCoroutine(ReloadRoutine());
			}
			else
			{
				UpdateAmmoCount();
			}
		}

		private IEnumerator ReloadRoutine()
		{
			recticleObj.SetActive(value: false);
			lbReloading.text = "Reloading";
			int count = 0;
			reloadProgressObj.SetActive(value: true);
			while (reloading)
			{
				string text = string.Empty;
				for (int i = 0; i < count; i++)
				{
					text += ".";
				}
				lbReloading.text = text + "Reloading" + text;
				count++;
				if (count == 4)
				{
					count = 0;
				}
				yield return new WaitForSeconds(0.25f);
			}
			reloadProgressObj.SetActive(value: false);
			lbReloading.text = string.Empty;
			recticleObj.SetActive(value: true);
		}

		private void OnSwitchWeapon()
		{
			UpdateAmmoCount();
			reloading = false;
			fpsItem.imgRoot.sprite = FPSControl.GetCurrentWeaponIcon();
		}

		private void UpdateAmmoCount()
		{
			int totalAmmoCount = FPSControl.GetTotalAmmoCount();
			int currentAmmoCount = FPSControl.GetCurrentAmmoCount();
			fpsItem.label.text = currentAmmoCount + "/" + totalAmmoCount;
		}

		private void OnFireButtonDown(GameObject butObj, int pointerID)
		{
			OnFireButton(null);
			touchFiring = true;
		}

		private void OnFireButtonUp(GameObject butObj, int pointerID)
		{
			touchFiring = false;
		}

		public void OnFireButton(GameObject butObj)
		{
			FPSControl.Fire();
		}

		public void OnReloadButton()
		{
			FPSControl.Reload();
		}

		public void OnPrevWeaponButton()
		{
			FPSControl.SelectPrevWeapon();
		}

		public void OnNextWeaponButton()
		{
			FPSControl.SelectNextWeapon();
		}

		public void OnExitFPSButton()
		{
			FPSControl.Hide();
		}

		public static bool IsOn()
		{
			return !(instance == null) && instance.isOn;
		}

		public static void Show()
		{
			instance._Show();
		}

		public void _Show()
		{
			OnSwitchWeapon();
			lbReloading.text = string.Empty;
			isOn = true;
			buttonExit.SetActive(flag: true);
			UIMainControl.DisableInput();
			UIMainControl.FadeIn(canvasGroup, 0.25f, thisObj);
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			isOn = false;
			UIMainControl.EnableInput();
			UIMainControl.FadeOut(canvasGroup, 0.25f, thisObj);
		}
	}
}
