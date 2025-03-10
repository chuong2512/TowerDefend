namespace SIS
{
	public class IAPArticle
	{
		public string id;

		public string title;

		public string description;

		public string price;

		public IAPArticle(GoogleProductTemplate prod)
		{
			id = prod.SKU;
			title = prod.Title;
			description = prod.Description;
			price = prod.LocalizedPrice;
		}
	}
}
