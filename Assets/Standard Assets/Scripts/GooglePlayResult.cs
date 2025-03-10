using System;

public class GooglePlayResult
{
	private GP_GamesStatusCodes _response;

	private string _message;

	[Obsolete("response is deprecated, please use Response instead.")]
	public GP_GamesStatusCodes response => Response;

	public GP_GamesStatusCodes Response => _response;

	[Obsolete("message is deprecated, please use Message instead.")]
	public string message => Message;

	public string Message => _message;

	[Obsolete("isSuccess is deprecated, please use IsSucceeded instead.")]
	public bool isSuccess => IsSucceeded;

	public bool IsSucceeded => _response == GP_GamesStatusCodes.STATUS_OK;

	[Obsolete("isFailure is deprecated, please use IsFailed instead.")]
	public bool isFailure => IsFailed;

	public bool IsFailed => !IsSucceeded;

	public GooglePlayResult(GP_GamesStatusCodes code)
	{
		SetCode(code);
	}

	public GooglePlayResult(string code)
	{
		SetCode((GP_GamesStatusCodes)Convert.ToInt32(code));
	}

	private void SetCode(GP_GamesStatusCodes code)
	{
		_response = code;
		_message = _response.ToString();
	}
}
