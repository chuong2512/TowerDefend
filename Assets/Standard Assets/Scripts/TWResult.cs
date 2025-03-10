public class TWResult
{
	private bool _IsSucceeded;

	private string _data = string.Empty;

	public bool IsSucceeded => _IsSucceeded;

	public string data => _data;

	public TWResult(bool IsResSucceeded, string resData)
	{
		_IsSucceeded = IsResSucceeded;
		_data = resData;
	}
}
