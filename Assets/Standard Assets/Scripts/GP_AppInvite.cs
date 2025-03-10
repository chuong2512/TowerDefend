public class GP_AppInvite
{
	public string _Id = string.Empty;

	public string _DeepLink = string.Empty;

	public bool _IsOpenedFromPlayStore;

	public string Id => _Id;

	public string DeepLink => _DeepLink;

	public bool IsOpenedFromPlayStore => _IsOpenedFromPlayStore;

	public GP_AppInvite(string id, string link = "", bool isOpenedFromPlatStore = false)
	{
		_Id = id;
		_DeepLink = link;
		_IsOpenedFromPlayStore = isOpenedFromPlatStore;
	}
}
