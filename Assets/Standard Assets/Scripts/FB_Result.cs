public class FB_Result
{
	private string _RawData = string.Empty;

	private string _Error = string.Empty;

	protected bool _IsSucceeded;

	public bool IsSucceeded => _IsSucceeded;

	public bool IsFailed => !IsSucceeded;

	public string RawData => _RawData;

	public string Error => _Error;

	public FB_Result(string RawData, string Error)
	{
		if (string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(RawData))
		{
			_IsSucceeded = true;
		}
		_RawData = RawData;
		_Error = Error;
	}
}
