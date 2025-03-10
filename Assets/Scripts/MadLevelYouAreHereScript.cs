using MadLevelManager;
using UnityEngine;

public class MadLevelYouAreHereScript : MonoBehaviour
{
	public Vector3 offset = new Vector3(0.37f, 0f, 0f);

	public float animationAmplitude = 0.02f;

	public float animationSpeed = 3f;

	private Transform lastUnlockedTransform;

	private bool initialized;

	private void Update()
	{
		if (initialized)
		{
			float x = Mathf.Sin(Time.time * animationSpeed) * animationAmplitude;
			base.transform.position = lastUnlockedTransform.position + offset + new Vector3(x, 0f, 0f);
		}
	}

	private void LateUpdate()
	{
		if (!initialized)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		MadLevelAbstractLayout current = MadLevelLayout.current;
		MadLevelConfiguration activeConfiguration = MadLevel.activeConfiguration;
		MadLevelConfiguration.Group group = activeConfiguration.FindGroupById(current.configurationGroup);
		string levelName = MadLevel.FindLastUnlockedLevelName(group.name);
		MadLevelIcon icon = current.GetIcon(levelName);
		lastUnlockedTransform = icon.transform;
		initialized = true;
	}
}
