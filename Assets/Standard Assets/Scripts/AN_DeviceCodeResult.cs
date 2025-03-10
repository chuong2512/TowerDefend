using SA.Common.Models;

public class AN_DeviceCodeResult : Result
{
	private string _deviceCode = string.Empty;

	private string _userCode = string.Empty;

	private string _verificationUrl = string.Empty;

	private long _expiresIn;

	private long _interval;

	public string DeviceCode => _deviceCode;

	public string UserCode => _userCode;

	public string VerificationUrl => _verificationUrl;

	public long ExpiresIn => _expiresIn;

	public long Interval => _interval;

	public AN_DeviceCodeResult(string errorMessage)
		: base(new Error(0, errorMessage))
	{
	}

	public AN_DeviceCodeResult(string deviceCode, string userCode, string verificationUrl, long expiresIn, long interval)
	{
		_deviceCode = deviceCode;
		_userCode = userCode;
		_verificationUrl = verificationUrl;
		_expiresIn = expiresIn;
		_interval = interval;
	}
}
