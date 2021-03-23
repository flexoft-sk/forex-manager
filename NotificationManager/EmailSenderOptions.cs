namespace Flexoft.ForexManager.NotificationManager
{
	/// <summary> Email sender options </summary>
	public class EmailSenderOptions
    {
		/// <summary>Gets or sets the server.</summary>
		/// <value>The server.</value>
		public string Server { get; set; }

		/// <summary>Gets or sets the port.</summary>
		/// <value>The port.</value>
		public int Port { get; set; }

		/// <summary>Gets or sets the user.</summary>
		/// <value>The user.</value>
		public string User { get; set; }

		/// <summary>Gets or sets the password.</summary>
		/// <value>The password.</value>
		public string Password { get; set; }

		/// <summary>Gets or sets the sender.</summary>
		/// <value>The sender.</value>
		public string Sender { get; set; }
    }
}
