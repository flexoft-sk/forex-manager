namespace Flexoft.ForexManager.RatesFetcher
{
	/// <summary> Options for rate fetcher</summary>
	public class RatesFetcherOptions
    {
		/// <summary>Gets or sets the API key.</summary>
		/// <value>The API key.</value>
		public string ApiKey { get; set; }

		/// <summary> Alternative API key</summary>
		public string AltApiKey { get; set; }
    }
}
