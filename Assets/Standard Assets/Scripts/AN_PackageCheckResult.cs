using SA.Common.Models;

public class AN_PackageCheckResult : Result
{
	private string _packageName;

	public string packageName => _packageName;

	public AN_PackageCheckResult(string packId)
	{
		_packageName = packId;
	}

	public AN_PackageCheckResult(string packId, Error error)
		: base(error)
	{
	}
}
