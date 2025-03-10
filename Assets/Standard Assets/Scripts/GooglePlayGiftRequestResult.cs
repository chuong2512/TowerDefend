using System;

public class GooglePlayGiftRequestResult
{
	private GP_GamesActivityResultCodes _code;

	public GP_GamesActivityResultCodes code => _code;

	public bool isSuccess => _code == GP_GamesActivityResultCodes.RESULT_OK;

	public bool isFailure => !isSuccess;

	public GooglePlayGiftRequestResult(string r_code)
	{
		_code = (GP_GamesActivityResultCodes)Convert.ToInt32(r_code);
	}
}
