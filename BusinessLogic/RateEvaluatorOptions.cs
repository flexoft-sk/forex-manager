namespace Flexoft.ForexManager.BusinessLogic
{
	public class RateEvaluatorOptions
	{
		public float CloseOffset { get; set; }

		public string NotificationTarget { get; set; }

		public int OpenHour { get; set; }

		public int OpenAmount { get; set; }
	}
}