using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace TDTK
{
	public class CameraControl : MonoBehaviour
	{
		private float initialMousePosX;

		private float initialMousePosY;

		private float initialRotX;

		private float initialRotY;

		private Vector3 lastTouchPos = new Vector3(9999f, 9999f, 9999f);

		private Vector3 moveDir = Vector3.zero;

		private float moveMagnitude;

		private float touchZoomSpeed;

		[HideInInspector]
		public Transform camT;

		[HideInInspector]
		public BlurOptimized blurEffect;

		public float panSpeed = 5f;

		public float zoomSpeed = 5f;

		public float rotationSpeed = 1f;

		public bool enableMouseZoom = true;

		public bool enableMouseRotate = true;

		public bool enableMousePanning;

		public bool enableKeyPanning = true;

		public int mousePanningZoneWidth = 10;

		public bool enableTouchPan = true;

		public bool enableTouchZoom = true;

		public bool enableTouchRotate;

		public float minPosX = -10f;

		public float maxPosX = 10f;

		public float minPosZ = -10f;

		public float maxPosZ = 10f;

		public float minZoomDistance = 8f;

		public float maxZoomDistance = 30f;

		public float minRotateAngle = 10f;

		public float maxRotateAngle = 89f;

		private float deltaT;

		private float currentZoom;

		private Transform thisT;

		public static CameraControl instance;

		private bool fpsOn;

		public bool avoidClipping;

		private bool obstacle;

		public bool showGizmo = true;

		public static void Disable()
		{
			if (instance != null)
			{
				instance.enabled = false;
			}
		}

		public static void Enable()
		{
			if (instance != null)
			{
				instance.enabled = true;
			}
		}

		private void Awake()
		{
			thisT = base.transform;
			instance = this;
			camT = Camera.main.transform;
			blurEffect = camT.GetComponent<BlurOptimized>();
		}

		private void Start()
		{
			minRotateAngle = Mathf.Max(10f, minRotateAngle);
			maxRotateAngle = Mathf.Min(89f, maxRotateAngle);
			minZoomDistance = Mathf.Max(1f, minZoomDistance);
			Vector3 localPosition = camT.localPosition;
			currentZoom = localPosition.z;
		}

		private void OnEnable()
		{
			TDTK.onFPSModeE += OnFPSMode;
		}

		private void OnDisable()
		{
			TDTK.onFPSModeE -= OnFPSMode;
		}

		private void OnFPSMode(bool flag)
		{
			fpsOn = flag;
		}

		public static void SetPosition(Vector3 newPos)
		{
			if (!instance.fpsOn)
			{
				instance.thisT.position = newPos;
			}
		}

		private void Update()
		{
			if (fpsOn)
			{
				return;
			}
			if (Time.timeScale == 1f)
			{
				deltaT = Time.deltaTime;
			}
			else if (Time.timeScale > 1f)
			{
				deltaT = Time.deltaTime / Time.timeScale;
			}
			else
			{
				deltaT = 0.015f;
			}
			if (!UI.IsCursorOnUI(0) && !BuildManager.InDragNDrop())
			{
				if (enableTouchPan)
				{
					Vector3 eulerAngles = base.transform.eulerAngles;
					Quaternion rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
					if (UnityEngine.Input.touchCount == 1)
					{
						Touch touch = Input.touches[0];
						if (touch.phase == TouchPhase.Moved)
						{
							Vector3 a = touch.position;
							if (lastTouchPos != new Vector3(9999f, 9999f, 9999f))
							{
								a -= lastTouchPos;
								moveMagnitude = new Vector3(a.x, 0f, a.y).magnitude * 0.1f;
								moveDir = new Vector3(a.x, 0f, a.y).normalized * -1f;
							}
							lastTouchPos = touch.position;
							if (moveMagnitude > 10f)
							{
								UIMainControl.ClearSelectedTower();
							}
						}
					}
					else
					{
						lastTouchPos = new Vector3(9999f, 9999f, 9999f);
					}
					Vector3 a2 = thisT.InverseTransformDirection(rotation * moveDir) * moveMagnitude;
					thisT.Translate(a2 * panSpeed * deltaT);
					moveMagnitude *= 1f - deltaT * 10f;
				}
				if (enableTouchZoom)
				{
					if (UnityEngine.Input.touchCount == 2)
					{
						Touch touch2 = Input.touches[0];
						Touch touch3 = Input.touches[1];
						if (touch2.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
						{
							Vector3 vector = touch2.position - touch2.deltaPosition - (touch3.position - touch3.deltaPosition);
							Vector3 vector2 = touch2.position - touch3.position;
							float f = Vector3.Dot(vector.normalized, vector2.normalized);
							if (Mathf.Abs(f) > 0.7f)
							{
								touchZoomSpeed = vector2.magnitude - vector.magnitude;
							}
						}
					}
					currentZoom += Time.deltaTime * zoomSpeed * touchZoomSpeed;
					touchZoomSpeed *= 1f - Time.deltaTime * 15f;
				}
				if (enableTouchRotate && UnityEngine.Input.touchCount == 2)
				{
					Touch touch4 = Input.touches[0];
					Touch touch5 = Input.touches[1];
					Vector2 normalized = touch4.deltaPosition.normalized;
					Vector2 normalized2 = touch5.deltaPosition.normalized;
					Vector2 vector3 = (normalized + normalized2) / 2f;
					Vector3 eulerAngles2 = thisT.rotation.eulerAngles;
					float value = eulerAngles2.x - vector3.y * rotationSpeed;
					Vector3 eulerAngles3 = thisT.rotation.eulerAngles;
					float y = eulerAngles3.y + vector3.x * rotationSpeed;
					value = Mathf.Clamp(value, minRotateAngle, maxRotateAngle);
					thisT.rotation = Quaternion.Euler(value, y, 0f);
				}
			}
			if (enableMouseRotate)
			{
				if (Input.GetMouseButtonDown(1))
				{
					Vector3 mousePosition = UnityEngine.Input.mousePosition;
					initialMousePosX = mousePosition.x;
					Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
					initialMousePosY = mousePosition2.y;
					Vector3 eulerAngles4 = thisT.eulerAngles;
					initialRotX = eulerAngles4.y;
					Vector3 eulerAngles5 = thisT.eulerAngles;
					initialRotY = eulerAngles5.x;
				}
				if (Input.GetMouseButton(1))
				{
					Vector3 mousePosition3 = UnityEngine.Input.mousePosition;
					float num = mousePosition3.x - initialMousePosX;
					float num2 = 0.1f * (initialRotX / (float)Screen.width);
					float num3 = num * rotationSpeed + num2;
					float num4 = initialMousePosY;
					Vector3 mousePosition4 = UnityEngine.Input.mousePosition;
					float num5 = num4 - mousePosition4.y;
					float num6 = 0f - 0.1f * (initialRotY / (float)Screen.height);
					float num7 = num5 * rotationSpeed + num6;
					float num8 = num7 + initialRotY;
					if (num8 > maxRotateAngle)
					{
						initialRotY -= num7 + initialRotY - maxRotateAngle;
						num8 = maxRotateAngle;
					}
					else if (num8 < minRotateAngle)
					{
						initialRotY += minRotateAngle - (num7 + initialRotY);
						num8 = minRotateAngle;
					}
					thisT.rotation = Quaternion.Euler(num8, num3 + initialRotX, 0f);
				}
			}
			Vector3 eulerAngles6 = thisT.eulerAngles;
			Quaternion rotation2 = Quaternion.Euler(0f, eulerAngles6.y, 0f);
			if (enableKeyPanning)
			{
				if (Input.GetButton("Horizontal"))
				{
					Vector3 a3 = base.transform.InverseTransformDirection(rotation2 * Vector3.right);
					thisT.Translate(a3 * panSpeed * deltaT * UnityEngine.Input.GetAxisRaw("Horizontal"));
				}
				if (Input.GetButton("Vertical"))
				{
					Vector3 a4 = base.transform.InverseTransformDirection(rotation2 * Vector3.forward);
					thisT.Translate(a4 * panSpeed * deltaT * UnityEngine.Input.GetAxisRaw("Vertical"));
				}
			}
			if (enableMousePanning)
			{
				Vector3 mousePosition5 = UnityEngine.Input.mousePosition;
				Vector3 a5 = base.transform.InverseTransformDirection(rotation2 * Vector3.right);
				if (mousePosition5.x <= 0f)
				{
					thisT.Translate(a5 * panSpeed * deltaT * -3f);
				}
				else if (mousePosition5.x <= (float)mousePanningZoneWidth)
				{
					thisT.Translate(a5 * panSpeed * deltaT * -1f);
				}
				else if (mousePosition5.x >= (float)Screen.width)
				{
					thisT.Translate(a5 * panSpeed * deltaT * 3f);
				}
				else if (mousePosition5.x > (float)(Screen.width - mousePanningZoneWidth))
				{
					thisT.Translate(a5 * panSpeed * deltaT * 1f);
				}
				Vector3 a6 = base.transform.InverseTransformDirection(rotation2 * Vector3.forward);
				if (mousePosition5.y <= 0f)
				{
					thisT.Translate(a6 * panSpeed * deltaT * -3f);
				}
				else if (mousePosition5.y <= (float)mousePanningZoneWidth)
				{
					thisT.Translate(a6 * panSpeed * deltaT * -1f);
				}
				else if (mousePosition5.y >= (float)Screen.height)
				{
					thisT.Translate(a6 * panSpeed * deltaT * 3f);
				}
				else if (mousePosition5.y > (float)(Screen.height - mousePanningZoneWidth))
				{
					thisT.Translate(a6 * panSpeed * deltaT * 1f);
				}
			}
			if (enableMouseZoom)
			{
				float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f)
				{
					currentZoom += zoomSpeed * axis;
					currentZoom = Mathf.Clamp(currentZoom, 0f - maxZoomDistance, 0f - minZoomDistance);
				}
			}
			if (avoidClipping)
			{
				Vector3 a7 = thisT.TransformPoint(new Vector3(0f, 0f, currentZoom));
				Vector3 direction = a7 - thisT.position;
				float maxDistance = Vector3.Distance(a7, thisT.position);
				obstacle = Physics.Raycast(thisT.position, direction, out RaycastHit hitInfo, maxDistance);
				if (!obstacle)
				{
					currentZoom = Mathf.Clamp(currentZoom, 0f - maxZoomDistance, 0f - minZoomDistance);
					Vector3 localPosition = camT.localPosition;
					float z = Mathf.Lerp(localPosition.z, currentZoom, Time.deltaTime * 4f);
					Transform transform = camT;
					Vector3 localPosition2 = camT.localPosition;
					float x = localPosition2.x;
					Vector3 localPosition3 = camT.localPosition;
					transform.localPosition = new Vector3(x, localPosition3.y, z);
				}
				else
				{
					maxDistance = Vector3.Distance(hitInfo.point, thisT.position) * 0.85f;
					Vector3 localPosition4 = camT.localPosition;
					float z2 = Mathf.Lerp(localPosition4.z, 0f - maxDistance, Time.deltaTime * 50f);
					Transform transform2 = camT;
					Vector3 localPosition5 = camT.localPosition;
					float x2 = localPosition5.x;
					Vector3 localPosition6 = camT.localPosition;
					transform2.localPosition = new Vector3(x2, localPosition6.y, z2);
				}
			}
			else
			{
				currentZoom = Mathf.Clamp(currentZoom, 0f - maxZoomDistance, 0f - minZoomDistance);
				Vector3 localPosition7 = camT.localPosition;
				float z3 = Mathf.Lerp(localPosition7.z, currentZoom, Time.deltaTime * 4f);
				Transform transform3 = camT;
				Vector3 localPosition8 = camT.localPosition;
				float x3 = localPosition8.x;
				Vector3 localPosition9 = camT.localPosition;
				transform3.localPosition = new Vector3(x3, localPosition9.y, z3);
			}
			Vector3 position = thisT.position;
			float num9 = Mathf.Clamp(position.x, minPosX, maxPosX);
			Vector3 position2 = thisT.position;
			float z4 = Mathf.Clamp(position2.z, minPosZ, maxPosZ);
			Transform transform4 = thisT;
			float x4 = num9;
			Vector3 position3 = thisT.position;
			transform4.position = new Vector3(x4, position3.y, z4);
		}

		public static void TurnBlurOn()
		{
			if (!(instance == null) && !(instance.blurEffect == null))
			{
				instance.StartCoroutine(instance.FadeBlurRoutine(instance.blurEffect, 0f, 2f));
			}
		}

		public static void TurnBlurOff()
		{
			if (!(instance == null) && !(instance.blurEffect == null))
			{
				instance.StartCoroutine(instance.FadeBlurRoutine(instance.blurEffect, 2f));
			}
		}

		public static void FadeBlur(BlurOptimized blurEff, float startValue = 0f, float targetValue = 0f)
		{
			if (!(blurEff == null) && !(instance == null))
			{
				instance.StartCoroutine(instance.FadeBlurRoutine(blurEff, startValue, targetValue));
			}
		}

		private IEnumerator FadeBlurRoutine(BlurOptimized blurEff, float startValue = 0f, float targetValue = 0f)
		{
			blurEff.enabled = true;
			float duration = 0f;
			while (duration < 1f)
			{
				float value = blurEff.blurSize = Mathf.Lerp(startValue, targetValue, duration);
				duration += Time.unscaledDeltaTime * 4f;
				yield return null;
			}
			blurEff.blurSize = targetValue;
			if (targetValue == 0f)
			{
				blurEff.enabled = false;
			}
			if (targetValue == 1f)
			{
				blurEff.enabled = true;
			}
		}

		private void OnDrawGizmos()
		{
			if (showGizmo)
			{
				float x = minPosX;
				Vector3 position = base.transform.position;
				Vector3 vector = new Vector3(x, position.y, maxPosZ);
				float x2 = maxPosX;
				Vector3 position2 = base.transform.position;
				Vector3 vector2 = new Vector3(x2, position2.y, maxPosZ);
				float x3 = maxPosX;
				Vector3 position3 = base.transform.position;
				Vector3 vector3 = new Vector3(x3, position3.y, minPosZ);
				float x4 = minPosX;
				Vector3 position4 = base.transform.position;
				Vector3 vector4 = new Vector3(x4, position4.y, minPosZ);
				Gizmos.color = Color.green;
				Gizmos.DrawLine(vector, vector2);
				Gizmos.DrawLine(vector2, vector3);
				Gizmos.DrawLine(vector3, vector4);
				Gizmos.DrawLine(vector4, vector);
			}
		}
	}
}
