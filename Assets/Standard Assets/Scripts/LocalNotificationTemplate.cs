using System;
using System.Text;

public class LocalNotificationTemplate
{
	private int _id;

	private string _title;

	private string _message;

	private DateTime _fireDate;

	private const string DATA_SPLITTER = "|||";

	public int id => _id;

	public string title => _title;

	public string message => _message;

	public DateTime fireDate => _fireDate;

	public string SerializedString => Convert.ToBase64String(Encoding.UTF8.GetBytes(id.ToString() + "|||" + title + "|||" + message + "|||" + fireDate.Ticks.ToString()));

	public bool IsFired
	{
		get
		{
			if (DateTime.Now.Ticks > fireDate.Ticks)
			{
				return true;
			}
			return false;
		}
	}

	public LocalNotificationTemplate(string data)
	{
		string[] array = data.Split(new string[1]
		{
			"|||"
		}, StringSplitOptions.None);
		_id = Convert.ToInt32(array[0]);
		_title = array[1];
		_message = array[2];
		_fireDate = new DateTime(Convert.ToInt64(array[3]));
	}

	public LocalNotificationTemplate(int nId, string ttl, string msg, DateTime date)
	{
		_id = nId;
		_title = ttl;
		_message = msg;
		_fireDate = date;
	}
}
