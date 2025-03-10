using UnityEngine;

namespace SIS
{
	public class ReceiptValidator : MonoBehaviour
	{
		public VerificationType verificationType;

		public virtual bool shouldValidate(VerificationType verificationType)
		{
			return false;
		}

		public virtual void Validate()
		{
		}

		public virtual void Validate(string id, string receipt)
		{
		}
	}
}
