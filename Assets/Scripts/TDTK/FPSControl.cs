using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class FPSControl : MonoBehaviour
	{
		public int recoilMode = 1;

		public float aimSensitivity = 2f;

		public List<int> unavailableIDList = new List<int>();

		[HideInInspector]
		public List<int> availableIDList = new List<int>();

		public List<FPSWeapon> weaponList = new List<FPSWeapon>();

		public int currentWeaponID;

		private FPSWeapon currentWeapon;

		public Transform weaponPivot;

		public Transform cameraPivot;

		public Transform camT;

		private Camera camFPS;

		private Camera camMain;

		private bool isInFPSMode;

		private GameObject thisObj;

		private Transform thisT;

		private static FPSControl instance;

		public Vector3 recoilDir;

		private bool recoiling;

		private float recoilModifier;

		private bool shaking;

		private float shakeMagnitude;

		public bool useTowerWeapon;

		private UnitTower anchorTower;

		private bool lerping;

		public static bool IsIDAvailable(int ID)
		{
			return !instance.unavailableIDList.Contains(ID);
		}

		public static bool IsInFPSMode()
		{
			return !(instance == null) && instance.isInFPSMode;
		}

		public static bool ActiveInScene()
		{
			return (!(instance == null)) ? true : false;
		}

		public static bool EnableInput()
		{
			return (instance.camFPS.enabled && !instance.lerping) ? true : false;
		}

		public void Init()
		{
			instance = this;
			thisObj = base.gameObject;
			thisT = base.transform;
			camMain = Camera.main;
			camFPS = camT.GetComponent<Camera>();
			recoilMode = Mathf.Clamp(recoilMode, 0, 2);
			if (recoilMode == 2)
			{
				weaponPivot.parent = camT;
			}
			List<FPSWeapon> fpsWeaponDBList = TDTK.GetFpsWeaponDBList();
			availableIDList = new List<int>();
			weaponList = new List<FPSWeapon>();
			for (int i = 0; i < fpsWeaponDBList.Count; i++)
			{
				if (!fpsWeaponDBList[i].disableInFPSControl && !unavailableIDList.Contains(fpsWeaponDBList[i].prefabID))
				{
					weaponList.Add(fpsWeaponDBList[i]);
					availableIDList.Add(fpsWeaponDBList[i].prefabID);
				}
			}
			List<FPSWeapon> unlockedWeaponList = PerkManager.GetUnlockedWeaponList();
			for (int j = 0; j < unlockedWeaponList.Count; j++)
			{
				weaponList.Add(unlockedWeaponList[j]);
			}
		}

		private void Start()
		{
			if (weaponList.Count > 0)
			{
				for (int i = 0; i < weaponList.Count; i++)
				{
					Transform transform = CreateWeaponInstance(weaponList[i].transform);
					weaponList[i] = transform.GetComponent<FPSWeapon>();
				}
				currentWeapon = weaponList[currentWeaponID];
				currentWeapon.gameObject.SetActive(value: true);
			}
			thisObj.SetActive(value: false);
		}

		public static void AddNewWeapon(FPSWeapon weaponPrefab)
		{
			if (instance != null)
			{
				instance._AddNewWeapon(weaponPrefab);
			}
		}

		public void _AddNewWeapon(FPSWeapon weaponPrefab)
		{
			for (int i = 0; i < weaponList.Count; i++)
			{
				if (weaponList[i].prefabID == weaponPrefab.prefabID)
				{
					return;
				}
			}
			Transform transform = CreateWeaponInstance(weaponPrefab.transform);
			weaponList.Add(transform.GetComponent<FPSWeapon>());
		}

		private Transform CreateWeaponInstance(Transform weapPrefabT)
		{
			Transform transform = UnityEngine.Object.Instantiate(weapPrefabT);
			transform.parent = weaponPivot;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.gameObject.SetActive(value: false);
			return transform;
		}

		public static void SelectNextWeapon()
		{
			instance.SelectWeapon(1);
		}

		public static void SelectPrevWeapon()
		{
			instance.SelectWeapon(-1);
		}

		public void SelectWeapon(int val)
		{
			if (!useTowerWeapon)
			{
				currentWeapon.gameObject.SetActive(value: false);
				currentWeaponID += val;
				if (currentWeaponID < 0)
				{
					currentWeaponID = weaponList.Count - 1;
				}
				else if (currentWeaponID >= weaponList.Count)
				{
					currentWeaponID = 0;
				}
				currentWeapon = weaponList[currentWeaponID];
				currentWeapon.gameObject.SetActive(value: true);
				TDTK.OnFPSSwitchWeapon();
			}
		}

		private void Update()
		{
			if (camFPS.enabled && !lerping)
			{
				Vector3 eulerAngles = cameraPivot.rotation.eulerAngles;
				float num = eulerAngles.x;
				Vector3 eulerAngles2 = cameraPivot.rotation.eulerAngles;
				float y = eulerAngles2.y;
				if (num > 180f)
				{
					num -= 360f;
				}
				Quaternion lhs = Quaternion.Euler(0f, y, 0f) * Quaternion.Euler(0f, UnityEngine.Input.GetAxis("Mouse X") * aimSensitivity, 0f);
				Quaternion rhs = Quaternion.Euler(Mathf.Clamp(num - UnityEngine.Input.GetAxis("Mouse Y") * aimSensitivity, -70f, 70f), 0f, 0f);
				cameraPivot.rotation = lhs * rhs;
				weaponPivot.localRotation = Quaternion.Lerp(weaponPivot.localRotation, Quaternion.identity, Time.deltaTime * 3f);
				recoilModifier *= 1f - Time.deltaTime * 2f;
			}
		}

		public static void Fire()
		{
			instance._Fire();
		}

		public void _Fire()
		{
			if (currentWeapon.Shoot())
			{
				AttackInstance attackInstance = new AttackInstance();
				attackInstance.srcWeapon = currentWeapon;
				for (int i = 0; i < currentWeapon.shootPoints.Count; i++)
				{
					Transform transform = currentWeapon.shootPoints[i];
					Transform transform2 = UnityEngine.Object.Instantiate(currentWeapon.GetShootObject(), transform.position, transform.rotation);
					transform2.GetComponent<ShootObject>().ShootFPS(attackInstance, transform);
				}
				Recoil(currentWeapon.recoil);
				TDTK.OnFPSShoot();
			}
		}

		public static void Reload()
		{
			instance.currentWeapon.Reload();
		}

		public static float GetReloadProgress()
		{
			return instance.currentWeapon.GetReloadProgress();
		}

		public static void ReloadComplete(FPSWeapon weap)
		{
			if (instance.currentWeapon == weap)
			{
				TDTK.OnFPSReload(flag: false);
			}
		}

		public static void StartReload(FPSWeapon weap)
		{
			if (instance.currentWeapon == weap)
			{
				TDTK.OnFPSReload(flag: true);
			}
		}

		public static float GetRecoilModifier()
		{
			return instance.recoilModifier;
		}

		public void Recoil(float magnitude = 1f)
		{
			if (recoilMode != 1 && recoilMode != 2)
			{
				return;
			}
			shakeMagnitude = magnitude * 0.1f;
			if (!shaking)
			{
				StartCoroutine(RecoilShakeRoutine());
			}
			if (recoilMode == 1)
			{
				recoilModifier += magnitude;
				float x = UnityEngine.Random.Range(recoilModifier * 0.5f, recoilModifier) * ((!(UnityEngine.Random.Range(0f, 1f) >= 0.5f)) ? 1f : (-1f));
				float y = (0f - UnityEngine.Random.Range(recoilModifier * 0.5f, recoilModifier)) * ((!(UnityEngine.Random.Range(0f, 1f) >= 0.5f)) ? 1f : (-1f));
				recoilDir = new Vector3(x, y, 0f) * magnitude * 5f;
				if (!recoiling)
				{
					StartCoroutine(RecoilRoutine());
				}
			}
			if (recoilMode == 2)
			{
				float num = UnityEngine.Random.Range(0.3f, 1f);
				float y2 = UnityEngine.Random.Range(-0.3f, 0.3f);
				recoilDir = new Vector3(0f - num, y2, 0f) * magnitude * 30f;
				if (!recoiling)
				{
					StartCoroutine(RecoilRoutine());
				}
			}
		}

		private IEnumerator RecoilRoutine()
		{
			recoiling = true;
			while (recoilDir.magnitude > 0.01f)
			{
				yield return null;
				if (recoilMode == 1)
				{
					weaponPivot.Rotate(recoilDir * Time.deltaTime * 2f);
				}
				if (recoilMode == 2)
				{
					camT.Rotate(recoilDir * Time.deltaTime);
				}
				recoilDir *= 1f - Time.deltaTime * 20f;
			}
			recoiling = false;
		}

		private IEnumerator RecoilShakeRoutine()
		{
			shaking = true;
			while (shakeMagnitude > 0f)
			{
				float x = UnityEngine.Random.Range(0f - shakeMagnitude, shakeMagnitude);
				float y = UnityEngine.Random.Range(0f - shakeMagnitude, shakeMagnitude);
				float z = UnityEngine.Random.Range(-1.5f * shakeMagnitude, 0f);
				camT.localPosition = new Vector3(x, y, z) * 1.5f;
				shakeMagnitude -= Time.deltaTime * 0.5f;
				yield return null;
			}
			shaking = false;
		}

		public void SetAnchorTower(UnitTower tower)
		{
			if (!useTowerWeapon)
			{
				return;
			}
			anchorTower = tower;
			if (currentWeapon != null)
			{
				currentWeapon.gameObject.SetActive(value: false);
			}
			currentWeapon = null;
			if (anchorTower.FPSWeaponID < 0)
			{
				return;
			}
			for (int i = 0; i < weaponList.Count; i++)
			{
				if (weaponList[i].prefabID == tower.FPSWeaponID)
				{
					currentWeapon = weaponList[i];
					currentWeapon.gameObject.SetActive(value: true);
				}
			}
		}

		public static int GetTotalAmmoCount()
		{
			return instance.currentWeapon.GetClipSize();
		}

		public static int GetCurrentAmmoCount()
		{
			return instance.currentWeapon.GetCurrentAmmo();
		}

		public static Sprite GetCurrentWeaponIcon()
		{
			return instance.currentWeapon.icon;
		}

		public static FPSWeapon GetCurrentWeapon()
		{
			return instance.currentWeapon;
		}

		public static bool UseTowerWeapon()
		{
			return instance.useTowerWeapon;
		}

		public static void Hide()
		{
			if (instance != null)
			{
				instance._Hide();
			}
		}

		public void _Hide()
		{
			Cursor.visible = true;
			isInFPSMode = false;
			Cursor.visible = true;
			TDTK.OnFPSMode(isInFPSMode);
			StartCoroutine(_LerpToMainCam());
		}

		public static void Show(UnitTower tower)
		{
			if (instance != null)
			{
				instance._Show(tower);
			}
		}

		public void _Show(UnitTower tower)
		{
			if (weaponList.Count == 0)
			{
				TDTK.OnGameMessage("No available weapon");
				return;
			}
			if (useTowerWeapon && tower.FPSWeaponID < 0)
			{
				TDTK.OnGameMessage("Tower doesn't have a weapon");
				return;
			}
			SetAnchorTower(tower);
			thisT.position = tower.thisT.position + new Vector3(0f, 5f, 0f);
			thisT.rotation = Camera.main.transform.rotation;
			Cursor.visible = false;
			isInFPSMode = true;
			TDTK.OnFPSMode(isInFPSMode);
			thisObj.SetActive(isInFPSMode);
			StartCoroutine(_LerpToView(thisT.position));
		}

		private IEnumerator _LerpToView(Vector3 targetPos)
		{
			lerping = true;
			camFPS.enabled = true;
			camFPS.gameObject.tag = "MainCamera";
			camMain.gameObject.tag = "Untagged";
			camFPS.gameObject.GetComponent<AudioListener>().enabled = true;
			camMain.gameObject.GetComponent<AudioListener>().enabled = false;
			TDTK.OnFPSSwitchCamera();
			float targetFOV = camFPS.fieldOfView;
			cameraPivot.rotation = camMain.transform.rotation;
			camT.position = camMain.transform.position;
			camFPS.fieldOfView = camMain.fieldOfView;
			Vector3 startingPos = camT.position;
			float startingFOV = camFPS.fieldOfView;
			float duration = 0f;
			while (duration < 1f)
			{
				camT.position = Vector3.Lerp(startingPos, targetPos, duration);
				camFPS.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, duration);
				duration += Time.deltaTime * 1f;
				yield return null;
			}
			camT.position = targetPos;
			camFPS.fieldOfView = targetFOV;
			lerping = false;
		}

		private IEnumerator _LerpToMainCam()
		{
			lerping = true;
			Vector3 targetPos = camMain.transform.position;
			Quaternion targetRot = camMain.transform.rotation;
			float targetFOV = camMain.fieldOfView;
			Vector3 startingPos = camT.position;
			Quaternion startingRot = camT.rotation;
			float startingFOV = camFPS.fieldOfView;
			float duration = 0f;
			while (duration < 1f)
			{
				camT.position = Vector3.Lerp(startingPos, targetPos, duration);
				camT.rotation = Quaternion.Lerp(startingRot, targetRot, duration);
				camFPS.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, duration);
				duration += Time.deltaTime * 1f;
				yield return null;
			}
			camT.position = targetPos;
			camT.rotation = targetRot;
			camFPS.fieldOfView = targetFOV;
			camFPS.gameObject.tag = "Untagged";
			camMain.gameObject.tag = "MainCamera";
			camFPS.gameObject.GetComponent<AudioListener>().enabled = false;
			camMain.gameObject.GetComponent<AudioListener>().enabled = true;
			TDTK.OnFPSSwitchCamera();
			camFPS.enabled = false;
			lerping = false;
			camT.localRotation = Quaternion.identity;
			thisObj.SetActive(isInFPSMode);
		}
	}
}
