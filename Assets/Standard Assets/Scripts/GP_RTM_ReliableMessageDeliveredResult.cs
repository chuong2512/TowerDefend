public class GP_RTM_ReliableMessageDeliveredResult : GP_RTM_Result
{
	private int _MessageTokenId;

	private byte[] _Data;

	public int MessageTokenId => _MessageTokenId;

	public byte[] Data => _Data;

	public GP_RTM_ReliableMessageDeliveredResult(string status, string roomId, int messageTokedId, byte[] data)
		: base(status, roomId)
	{
		_MessageTokenId = messageTokedId;
		_Data = data;
	}
}
