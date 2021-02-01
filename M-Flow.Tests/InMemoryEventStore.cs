using System;
using System.Collections.Generic;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace M_Flow.Tests
{
    /// <summary>
    /// Stores events in the memory without persisting.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        #region Fields

        readonly Dictionary<Guid, List<Event>> _Events = new();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the store for the entity with the specified identifier. 
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The found store or null.</returns>
        List<Event> GetEntityStore(Guid id)
        {
            return _Events.TryGetValue(id, out var entityStore) ? entityStore : null;
        }

        /// <summary>
        /// Creates the store for the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The created store.</returns>
        List<Event> CreateEntityStore(Guid id)
        {
            var entityStore = new List<Event>();
            _Events.Add(id, entityStore);
            return entityStore;
        }
        
        /// <summary>
        /// Adds the event into the entity store. 
        /// </summary>
        /// <param name="event">The event to add.</param>
        /// <param name="store">The entity store.</param>
        static void AddToEntityStore(Event @event, ICollection<Event> store)
        {
            store.Add(@event);
        }

        #endregion
        
        #region Implementation of IEventStore

        /// <summary>
        /// Sets the specified event into the store.
        /// </summary>
        /// <param name="event">The event to store.</param>
        public void Set(Event @event)
        {
            var entityStore = GetEntityStore(@event.EntityId) ?? CreateEntityStore(@event.EntityId);
            AddToEntityStore(@event, entityStore);
        }

        /// <summary>
        /// Gets all available identifiers in the store.
        /// </summary>
        /// <returns>The identifiers.</returns>
        public IEnumerable<Guid> GetIds()
        {
            return _Events.Keys;
        }

        /// <summary>
        /// Gets all events from the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The events.</returns>
        public IEnumerable<Event> Get(Guid id)
        {
            var entityStore = GetEntityStore(id);
            return entityStore;
        }

        #endregion
    }
}