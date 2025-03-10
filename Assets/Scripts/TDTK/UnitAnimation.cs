using UnityEngine;

namespace TDTK
{
	public class UnitAnimation : MonoBehaviour
	{
		public Animator animator;

		[Tooltip("Check to reset the animator transform position when the unit is spawned\nDisable this if the animator is on the root-object of the prefab")]
		public bool resetAnimatorPosition = true;

		[Header("Commons")]
		public AnimationClip clipIdle;

		public AnimationClip clipHit;

		public AnimationClip clipDestroyed;

		public AnimationClip clipAttack;

		public float attackDelay;

		[Header("For Creeps")]
		public AnimationClip clipMove;

		public AnimationClip clipSpawn;

		public AnimationClip clipDestination;

		[Header("For Towers")]
		public AnimationClip clipConstruct;

		public AnimationClip clipDeconstruct;

		private Vector3 defaultPos;

		private Quaternion defaultRot;

		private void Awake()
		{
			defaultPos = animator.transform.localPosition;
			defaultRot = animator.transform.localRotation;
			if (animator == null)
			{
				UnityEngine.Debug.LogWarning("Animator component is not assigned for UnitAnimation", this);
				return;
			}
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
			animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
			animatorOverrideController["DummyIdle"] = clipIdle;
			animatorOverrideController["DummyHit"] = clipHit;
			animatorOverrideController["DummyAttack"] = clipAttack;
			animatorOverrideController["DummyMove"] = clipMove;
			animatorOverrideController["DummySpawn"] = clipSpawn;
			animatorOverrideController["DummyDestroyed"] = clipDestroyed;
			animatorOverrideController["DummyDestination"] = clipDestination;
			animatorOverrideController["DummyConstruct"] = clipConstruct;
			animatorOverrideController["DummyDeconstruct"] = clipDeconstruct;
			animator.runtimeAnimatorController = animatorOverrideController;
			Unit component = base.gameObject.GetComponent<Unit>();
			component.SetUnitAnimation(this);
		}

		private void OnEnable()
		{
			if (!(animator == null))
			{
				animator.SetBool("Destroyed", value: false);
				animator.SetBool("Destination", value: false);
				if (resetAnimatorPosition)
				{
					animator.transform.localPosition = defaultPos;
					animator.transform.localRotation = defaultRot;
				}
				animator.Rebind();
			}
		}

		public void PlayMove(float speed)
		{
			animator.SetFloat("Speed", speed);
		}

		public void PlaySpawn()
		{
			if (clipSpawn != null)
			{
				animator.SetTrigger("Spawn");
			}
		}

		public void PlayHit()
		{
			if (clipHit != null)
			{
				animator.SetTrigger("Hit");
			}
		}

		public float PlayDestroyed()
		{
			if (clipDestroyed != null)
			{
				animator.SetBool("Destroyed", value: true);
			}
			return (!(clipDestroyed != null)) ? 0f : clipDestroyed.length;
		}

		public float PlayDestination()
		{
			if (clipDestination != null)
			{
				animator.SetBool("Destination", value: true);
			}
			return (!(clipDestination != null)) ? 0f : clipDestination.length;
		}

		public void PlayConstruct()
		{
			if (clipConstruct != null)
			{
				animator.SetTrigger("Construct");
			}
		}

		public void PlayDeconstruct()
		{
			if (clipDeconstruct != null)
			{
				animator.SetTrigger("Deconstruct");
			}
		}

		public float PlayAttack()
		{
			if (clipAttack != null)
			{
				animator.SetTrigger("Attack");
			}
			return attackDelay;
		}
	}
}
