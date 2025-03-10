public class FB_LoginResult : FB_Result
{
	private bool _IsCanceled;

	private string _UserId;

	private string _AccessToken;

	public string UserId => _UserId;

	public string AccessToken => _AccessToken;

	public bool IsCanceled => _IsCanceled;

	public FB_LoginResult(string RawData, string Error, bool isCanceled)
		: base(RawData, Error)
	{
		_IsCanceled = isCanceled;
	}

	public void SetCredential(string userId, string accessToken)
	{
		_UserId = userId;
		_AccessToken = accessToken;
	}
}
