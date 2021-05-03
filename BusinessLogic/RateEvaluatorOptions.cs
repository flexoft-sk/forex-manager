namespace Flexoft.ForexManager.BusinessLogic
{
	/// <summary> Options of rate evaluation</summary>
	public class RateEvaluatorOptions
	{
		/// <summary>Gets or sets the percentage threshold of a position closing evaluation.</summary>
		/// <value>The close offset percentage.</value>
		public float CloseOffsetPercentage { get; set; }

		/// <summary>Gets or sets the notification target.</summary>
		/// <value>The notification target.</value>
		public string NotificationTarget { get; set; }

		/// <summary>Gets or sets the hour of a day when position is opened.</summary>
		/// <value>The open hour.</value>
		public int OpenHour { get; set; }

		/// <summary>Gets or sets the open amount.</summary>
		/// <value>The open amount.</value>
		public int OpenAmount { get; set; }

		/// <summary>Gets or sets the url to show UI for closing.</summary>
		/// <value>The url to show UI for closing.</value>
		public string CloseUIUrl { get; set; }
	}
}