namespace Flexoft.ForexManager.Store
{
    /// <summary> Encapsulates storage related repositories</summary>
    public interface IDataStore
    {
        /// <summary>Gets the position module.</summary>
        /// <value>The position module.</value>
        IPosition Position { get; }
    }
}