namespace Flexoft.ForexManager.NotificationManager
{
	/// <summary> Provides functionality to trigger notifications</summary>
	public interface INotificationManager
    {
		/// <summary>Notifies the specified receiver.</summary>
		/// <param name="title">The title.</param>
		/// <param name="content">The content.</param>
		/// <param name="receiver">The receiver.</param>
		void Notify(string title, string content, string receiver);
	}
}
