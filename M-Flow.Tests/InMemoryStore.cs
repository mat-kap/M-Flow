using System;
using System.Collections.Generic;
using MFlow.Operation.Adapters.Providers;

namespace M_Flow.Tests
{
    /// <summary>
    /// Implementation of an item store which stores the items in the memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryStore<T> : IItemStore<T>
    {
        #region Fields

        readonly Dictionary<Guid, T> _Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="InMemoryStore{T}" />
        /// </summary>
        /// <param name="items">The items which should be in the store.</param>
        public InMemoryStore(params Tuple<Guid,T>[] items)
        {
            _Items = new Dictionary<Guid, T>();

            foreach (var (id, item) in items)
                _Items.Add(id, item);
        }

        #endregion
        
        #region Implementation of IItemStore<T>

        /// <summary>
        /// Gets the item identifiers.
        /// </summary>
        /// <returns>The item identifiers.</returns>
        public IEnumerable<Guid> GetIds()
        {
            return _Items.Keys;
        }

        /// <summary>
        /// Writes the specified item into the store.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <param name="item">The item.</param>
        public void Write(Guid id, T item)
        {
            if (_Items.ContainsKey(id))
                _Items[id] = item;
            else
                _Items.Add(id, item);
        }

        /// <summary>
        /// Reads the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns>The read item.</returns>
        public T Read(Guid id)
        {
            return _Items.TryGetValue(id, out var item) ? item : default;
        }

        /// <summary>
        /// Deletes the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        public void Delete(Guid id)
        {
            if (_Items.ContainsKey(id))
                _Items.Remove(id);
        }

        #endregion
    }
}