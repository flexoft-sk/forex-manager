namespace Flexoft.ForexManager.NotificationManager
{
    public interface INotificationManager
    {
        void Notify(string title, string content, string receiver);
		string Dump();
	}
}
