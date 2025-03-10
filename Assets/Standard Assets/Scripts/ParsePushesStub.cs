using System;
using System.Collections.Generic;
using System.Threading;

public static class ParsePushesStub
{
	public static event Action<string, Dictionary<string, object>> OnPushReceived;

	public static void InitParse()
	{
	}

	static ParsePushesStub()
	{
		ParsePushesStub.OnPushReceived = delegate
		{
		};
	}
}
