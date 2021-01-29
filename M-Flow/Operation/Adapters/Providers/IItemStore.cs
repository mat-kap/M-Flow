using System;
using System.Collections.Generic;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Definition of an item store.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    public interface IItemStore<T>
    {
        /// <summary>
        /// Gets the item identifiers.
        /// </summary>
        /// <returns>The item identifiers.</returns>
        IEnumerable<Guid> GetIds();

        /// <summary>
        /// Writes the specified item into the store.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <param name="item">The item.</param>
        void Write(Guid id, T item);

        /// <summary>
        /// Reads the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns>The read item.</returns>
        T Read(Guid id);

        /// <summary>
        /// Deletes the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        void Delete(Guid id);
    }
}