using System;

public class GP_RTM_Result
{
	public GP_GamesStatusCodes _status;

	public string _roomId;

	public GP_GamesStatusCodes status => _status;

	public string roomId => _roomId;

	public GP_RTM_Result(string r_status, string r_roomId)
	{
		_status = (GP_GamesStatusCodes)Convert.ToInt32(r_status);
		_roomId = r_roomId;
	}
}
