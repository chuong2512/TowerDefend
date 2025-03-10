using UnityEngine;

public class AndroidNotificationBuilder
{
	public class NotificationColor
	{
		private Color _value;

		public Color Value => _value;

		public NotificationColor(Color value)
		{
			_value = value;
		}
	}

	private int _id = 1;

	private string _title = string.Empty;

	private string _message = string.Empty;

	private int _time = 1;

	private string _sound = string.Empty;

	private string _smallIcon = string.Empty;

	private bool _vibration;

	private bool _showIfAppForeground = true;

	private bool _repeating;

	private int _repeatDelay = 60;

	private string _largeIcon = string.Empty;

	private Texture2D _bigPicture;

	private NotificationColor _color;

	private int _wakeLockTime = 10000;

	private const string SOUND_SILENT = "SOUND_SILENT";

	public int Id => _id;

	public string Title => _title;

	public string Message => _message;

	public int Time => _time;

	public NotificationColor Color => _color;

	public string Sound => _sound;

	public string Icon => _smallIcon;

	public bool Vibration => _vibration;

	public bool ShowIfAppForeground => _showIfAppForeground;

	public bool Repeating => _repeating;

	public int RepeatDelay => _repeatDelay;

	public string LargeIcon => _largeIcon;

	public Texture2D BigPicture => _bigPicture;

	public int WakeLockTime => _wakeLockTime;

	public AndroidNotificationBuilder(int id, string title, string message, int time)
	{
		_id = id;
		_title = title;
		_message = message;
		_time = time;
		_largeIcon = ((!(AndroidNativeSettings.Instance.LocalNotificationLargeIcon == null)) ? AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name.ToLower() : string.Empty);
		_smallIcon = ((!(AndroidNativeSettings.Instance.LocalNotificationSmallIcon == null)) ? AndroidNativeSettings.Instance.LocalNotificationSmallIcon.name.ToLower() : string.Empty);
		_sound = ((!(AndroidNativeSettings.Instance.LocalNotificationSound == null)) ? AndroidNativeSettings.Instance.LocalNotificationSound.name : string.Empty);
		_vibration = AndroidNativeSettings.Instance.EnableVibrationLocal;
		_showIfAppForeground = AndroidNativeSettings.Instance.ShowWhenAppIsForeground;
		_wakeLockTime = AndroidNativeSettings.Instance.LocalNotificationWakeLockTimer;
	}

	public void SetColor(NotificationColor color)
	{
		_color = color;
	}

	public void SetSoundName(string sound)
	{
		_sound = sound;
	}

	public void SetIconName(string icon)
	{
		_smallIcon = icon;
	}

	public void SetVibration(bool vibration)
	{
		_vibration = vibration;
	}

	public void SetSilentNotification()
	{
		_sound = "SOUND_SILENT";
	}

	public void ShowIfAppIsForeground(bool show)
	{
		_showIfAppForeground = show;
	}

	public void SetRepeating(bool repeat)
	{
		_repeating = repeat;
	}

	public void SetRepeatDelay(int delay)
	{
		_repeatDelay = delay;
	}

	public void SetLargeIcon(string icon)
	{
		_largeIcon = icon;
	}

	public void SetBigPicture(Texture2D picture)
	{
		_bigPicture = picture;
	}

	public void SetWakeLockTime(int wakeTime)
	{
		_wakeLockTime = wakeTime;
	}
}
