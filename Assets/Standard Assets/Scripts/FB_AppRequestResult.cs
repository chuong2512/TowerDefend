using ANMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FB_AppRequestResult : FB_Result
{
	private string _ReuqestId = string.Empty;

	private List<string> _Recipients = new List<string>();

	public string ReuqestId => _ReuqestId;

	public List<string> Recipients => _Recipients;

	public FB_AppRequestResult(string RawData, string Error)
		: base(RawData, Error)
	{
		if (_IsSucceeded)
		{
			try
			{
				Dictionary<string, object> dictionary = Json.Deserialize(RawData) as Dictionary<string, object>;
				if (dictionary.ContainsKey("request"))
				{
					_ReuqestId = Convert.ToString(dictionary["request"]);
				}
				else
				{
					_IsSucceeded = false;
				}
				if (dictionary.ContainsKey("to"))
				{
					List<object> list = dictionary["to"] as List<object>;
					if (list != null)
					{
						foreach (object item in list)
						{
							_Recipients.Add(Convert.ToString(item));
						}
					}
					else
					{
						string text = Convert.ToString(dictionary["to"]);
						if (text != null)
						{
							_Recipients.Add(text);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_IsSucceeded = false;
				UnityEngine.Debug.Log("FB_AppRequestResult parsing failed: " + ex.Message);
			}
		}
	}

	public FB_AppRequestResult(string requestId, List<string> _recipients, string RawData)
		: base(RawData, null)
	{
		if (requestId.Length > 0)
		{
			_ReuqestId = requestId;
			_Recipients = _recipients;
			_IsSucceeded = true;
		}
		else
		{
			_IsSucceeded = false;
		}
	}
}
