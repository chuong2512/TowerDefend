using System;

public class BillingResult
{
	private int _response;

	private string _message;

	private GooglePurchaseTemplate _purchase;

	[Obsolete("purchase is deprectaed, plase use Purchase instead")]
	public GooglePurchaseTemplate purchase => Purchase;

	public GooglePurchaseTemplate Purchase => _purchase;

	[Obsolete("response is deprectaed, plase use Response instead")]
	public int response => Response;

	public int Response => _response;

	[Obsolete("message is deprectaed, plase use Message instead")]
	public string message => Message;

	public string Message => _message;

	[Obsolete("isSuccess is deprectaed, plase use IsSuccess instead")]
	public bool isSuccess => IsSuccess;

	public bool IsSuccess => _response == 0;

	[Obsolete("isFailure is deprectaed, plase use IsFailure instead")]
	public bool isFailure => IsFailure;

	public bool IsFailure => !IsSuccess;

	public BillingResult(int code, string msg, GooglePurchaseTemplate p)
		: this(code, msg)
	{
		_purchase = p;
	}

	public BillingResult(int code, string msg)
	{
		_response = code;
		_message = msg;
	}
}
