using SA.Common.Util;
using System;
using System.Threading;
using UnityEngine;

public class GP_Participant
{
	private string _id;

	private string _playerid;

	private string _HiResImageUrl;

	private string _IconImageUrl;

	private string _DisplayName;

	private GP_ParticipantResult _result;

	private GP_RTM_ParticipantStatus _status = GP_RTM_ParticipantStatus.STATUS_UNRESPONSIVE;

	private Texture2D _SmallPhoto;

	private Texture2D _BigPhoto;

	public Texture2D SmallPhoto => _SmallPhoto;

	public Texture2D BigPhoto => _BigPhoto;

	public string id => _id;

	public string playerId => _playerid;

	public string HiResImageUrl => _HiResImageUrl;

	public string IconImageUrl => _IconImageUrl;

	public string DisplayName => _DisplayName;

	public GP_RTM_ParticipantStatus Status => _status;

	public GP_ParticipantResult Result => _result;

	public event Action<Texture2D> BigPhotoLoaded;

	public event Action<Texture2D> SmallPhotoLoaded;

	public GP_Participant(string uid, string playerUid, string stat, string hiResImg, string IconImg, string Name)
	{
		this.BigPhotoLoaded = delegate
		{
		};
		this.SmallPhotoLoaded = delegate
		{
		};
		
		_id = uid;
		_playerid = playerUid;
		_status = (GP_RTM_ParticipantStatus)Convert.ToInt32(stat);
		_HiResImageUrl = hiResImg;
		_IconImageUrl = IconImg;
		_DisplayName = Name;
	}

	public void SetResult(GP_ParticipantResult r)
	{
		_result = r;
	}

	public void LoadBigPhoto()
	{
		Loader.LoadWebTexture(_HiResImageUrl, HandheBigPhotoLoaed);
	}

	public void LoadSmallPhoto()
	{
		Loader.LoadWebTexture(_IconImageUrl, HandheSmallPhotoLoaed);
	}

	private void HandheBigPhotoLoaed(Texture2D tex)
	{
		if (this != null)
		{
			_BigPhoto = tex;
			this.BigPhotoLoaded(_BigPhoto);
		}
	}

	private void HandheSmallPhotoLoaed(Texture2D tex)
	{
		if (this != null)
		{
			_SmallPhoto = tex;
			this.SmallPhotoLoaded(_SmallPhoto);
		}
	}
}
