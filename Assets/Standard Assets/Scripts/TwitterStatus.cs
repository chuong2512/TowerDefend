using ANMiniJSON;
using System;
using System.Collections;

public class TwitterStatus
{
	private string _rawJSON;

	private string _text;

	private string _geo;

	public string rawJSON => _rawJSON;

	public string text => _text;

	public string geo => _geo;

	public TwitterStatus(IDictionary JSON)
	{
		_rawJSON = Json.Serialize(JSON);
		_text = Convert.ToString(JSON["text"]);
		_geo = Convert.ToString(JSON["geo"]);
	}
}
