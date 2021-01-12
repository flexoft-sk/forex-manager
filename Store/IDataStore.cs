namespace Flexoft.ForexManager.Store
{
    /// <summary> Encapsulates storage related repositories</summary>
    public interface IDataStore
    {
        /// <summary>Gets the session module.</summary>
        /// <value>The session module.</value>
        IPosition Transaction { get; }
    }
}